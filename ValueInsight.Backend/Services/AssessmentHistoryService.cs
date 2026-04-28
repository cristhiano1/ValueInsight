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

    public async Task<AssessmentRun> EnsureActiveRunAsync(int userId)
    {
        var activeRun = await _context.AssessmentRuns
            .Where(x => x.UserId == userId && x.Status == "InProgress")
            .OrderByDescending(x => x.CreatedAtUtc)
            .FirstOrDefaultAsync();

        if (activeRun != null)
            return activeRun;

        var run = new AssessmentRun
        {
            UserId = userId,
            CreatedAtUtc = DateTime.UtcNow,
            UpdatedAtUtc = DateTime.UtcNow,
            CompletedAtUtc = null,
            Status = "InProgress"
        };

        _context.AssessmentRuns.Add(run);
        await _context.SaveChangesAsync();

        return run;
    }

    public async Task SyncLatestRunAsync(int userId)
    {
        await SyncActiveRunAsync(userId);
    }

    public async Task SyncActiveRunAsync(int userId)
    {
        var run = await EnsureActiveRunAsync(userId);
        await SyncRunWithCurrentStateAsync(userId, run.Id);
    }

    public async Task CompleteActiveRunAsync(int userId)
    {
        var run = await EnsureActiveRunAsync(userId);

        await SyncRunWithCurrentStateAsync(userId, run.Id);

        run.Status = "Completed";
        run.CompletedAtUtc = DateTime.UtcNow;
        run.UpdatedAtUtc = DateTime.UtcNow;

        await _context.SaveChangesAsync();
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
        await _context.SaveChangesAsync();
    }
}