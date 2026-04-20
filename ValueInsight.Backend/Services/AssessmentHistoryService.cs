using Microsoft.EntityFrameworkCore;
using ValueInsight.Backend.Data;
using ValueInsight.Backend.Models;

namespace ValueInsight.Backend.Services;

public class AssessmentHistoryService
{
    private readonly ValueInsightDbContext _context;

    public AssessmentHistoryService(ValueInsightDbContext context)
    {
        _context = context;
    }

    public async Task<AssessmentRun> CreateRunFromCurrentStateAsync(int userId)
    {
        var run = new AssessmentRun
        {
            UserId = userId,
            CreatedAtUtc = DateTime.UtcNow,
            UpdatedAtUtc = DateTime.UtcNow,
            CompletedAtUtc = DateTime.UtcNow,
            Status = "Completed"
        };

        _context.AssessmentRuns.Add(run);
        await _context.SaveChangesAsync();

        await SyncRunWithCurrentStateAsync(userId, run.Id);
        return run;
    }

    public async Task SyncLatestRunAsync(int userId)
    {
        var latestRunId = await _context.AssessmentRuns
            .Where(x => x.UserId == userId)
            .OrderByDescending(x => x.CreatedAtUtc)
            .Select(x => (int?)x.Id)
            .FirstOrDefaultAsync();

        if (latestRunId.HasValue)
            await SyncRunWithCurrentStateAsync(userId, latestRunId.Value);
    }

    private async Task SyncRunWithCurrentStateAsync(int userId, int runId)
    {
        var run = await _context.AssessmentRuns
            .Include(x => x.ValueSelections)
            .Include(x => x.ReflectionAnswers)
            .FirstAsync(x => x.Id == runId);

        var currentValues = await _context.UserValues
            .Where(x => x.UserId == userId)
            .OrderBy(x => x.Rank)
            .ToListAsync();

        var currentReflections = await _context.ReflectionAnswers
            .Where(x => x.UserId == userId)
            .ToListAsync();

        _context.AssessmentValueSelections.RemoveRange(run.ValueSelections);
        _context.AssessmentReflectionAnswers.RemoveRange(run.ReflectionAnswers);
        await _context.SaveChangesAsync();

        foreach (var value in currentValues)
        {
            _context.AssessmentValueSelections.Add(new AssessmentValueSelection
            {
                AssessmentRunId = runId,
                ValueId = value.ValueId,
                Rank = value.Rank
            });
        }

        foreach (var answer in currentReflections)
        {
            _context.AssessmentReflectionAnswers.Add(new AssessmentReflectionAnswer
            {
                AssessmentRunId = runId,
                QuestionId = answer.QuestionId,
                QuestionText = answer.QuestionText,
                ResponseText = answer.ResponseText
            });
        }

        run.UpdatedAtUtc = DateTime.UtcNow;
        if (run.Status == "Completed")
            run.CompletedAtUtc ??= DateTime.UtcNow;

        var membership = await _context.TeamMembers.FirstOrDefaultAsync(x => x.UserId == userId);
        if (membership != null)
            membership.HasCompletedAssessment = true;

        await _context.SaveChangesAsync();
    }
}

