using System.Security.Claims;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using ValueInsight.Backend.Data;
using ValueInsight.Backend.Dtos;
using ValueInsight.Backend.Helpers;
using ValueInsight.Backend.Models;
using ValueInsight.Backend.Services;

namespace ValueInsight.Backend.Controllers;

[ApiController]
[Route("api/reports")]
[Authorize]
public class ReportsController : ControllerBase
{
    private readonly ValueInsightDbContext _context;
    private readonly TeamCultureService _teamCultureService;
    private readonly CulturalFitService _culturalFitService;
    private readonly TeamAccessService _teamAccessService;

    public ReportsController(
        ValueInsightDbContext context,
        TeamCultureService teamCultureService,
        CulturalFitService culturalFitService,
        TeamAccessService teamAccessService)
    {
        _context = context;
        _teamCultureService = teamCultureService;
        _culturalFitService = culturalFitService;
        _teamAccessService = teamAccessService;
    }

    [HttpGet("me")]
    public async Task<IActionResult> GetMyReport()
    {
        var userIdClaim = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
        if (!int.TryParse(userIdClaim, out var userId))
            return Unauthorized();

        var user = await _context.Users
            .Include(u => u.UserValues)
                .ThenInclude(uv => uv.Value)
            .Include(u => u.AssessmentRuns)
                .ThenInclude(r => r.ValueSelections)
                    .ThenInclude(vs => vs.Value)
            .Include(u => u.AssessmentRuns)
                .ThenInclude(r => r.ReflectionAnswers)
            .FirstOrDefaultAsync(u => u.Id == userId);

        if (user == null)
            return NotFound();

        var teamId = await _teamAccessService.GetCurrentTeamIdAsync(userId);
        var team = teamId.HasValue
            ? await _context.Teams
                .Include(t => t.Leader)
                .FirstOrDefaultAsync(t => t.Id == teamId.Value)
            : null;

        var userValues = user.UserValues
            .OrderBy(uv => uv.Rank)
            .ToList();

        var categoryProfile = CultureAnalysisHelper.BuildNormalizedCategoryProfile(userValues);

        var report = new PersonalReportDto
        {
            UserId = user.Id,
            UserName = user.Name,
            TeamId = teamId,
            TeamName = team?.Name,
            IsAdmin = user.IsAdmin,
            IsTeamLeader = team?.LeaderId == user.Id,
            TopValues = userValues.Select(uv => new RankedValueDto
            {
                ValueId = uv.ValueId,
                Name = uv.Value.Name,
                Category = CultureAnalysisHelper.ToDisplayName(uv.Value.Category),
                Rank = uv.Rank
            }).ToList(),
            CategoryProfile = categoryProfile
                .Select(kvp => new CategoryScoreDto
                {
                    Category = CultureAnalysisHelper.ToDisplayName(kvp.Key),
                    Percentage = Math.Round(kvp.Value * 100, 1)
                })
                .OrderByDescending(x => x.Percentage)
                .ToList(),
            ValueConflicts = BuildIndividualValueConflicts(categoryProfile)
        };

        var reflectionAnswers = await _context.ReflectionAnswers
            .Where(x => x.UserId == userId)
            .ToListAsync();

        report.ReflectionInsights = BuildReflectionInsightsFromCurrentAnswers(userValues, reflectionAnswers);

        var completedRuns = user.AssessmentRuns
            .Where(x => x.Status == "Completed")
            .OrderByDescending(x => x.CompletedAtUtc ?? x.UpdatedAtUtc)
            .ToList();

        report.AssessmentHistory = completedRuns
            .Take(10)
            .Select(run => new AssessmentHistoryItemDto
            {
                AssessmentRunId = run.Id,
                CompletedAtUtc = run.CompletedAtUtc ?? run.UpdatedAtUtc,
                TopValues = run.ValueSelections
                    .OrderBy(v => v.Rank)
                    .Take(5)
                    .Select(v => v.Value.Name)
                    .ToList(),
                PrimaryCategory = run.ValueSelections
                    .OrderBy(v => v.Rank)
                    .Select(v => CultureAnalysisHelper.ToDisplayName(v.Value.Category))
                    .FirstOrDefault() ?? "Not available",
                CategoryProfile = BuildCategoryProfileFromSelections(run.ValueSelections.OrderBy(v => v.Rank).ToList()),
                ReflectionInsights = BuildReflectionInsightsFromAssessmentRun(
                    run.ValueSelections.OrderBy(v => v.Rank).ToList(),
                    run.ReflectionAnswers.ToList()),
                ReflectionQuestions = BuildReflectionQuestionsFromAnswers(run.ReflectionAnswers.ToList())
            })
            .ToList();

        var fit = await _culturalFitService.CalculateForUser(user.Id);
        if (fit.HasValue)
        {
            report.CulturalFitScore = fit.Value.score;
            report.CulturalFitLabel = fit.Value.label;
        }

        if (teamId.HasValue)
        {
            var teamReport = await _teamCultureService.BuildTeamReport(teamId.Value);
            if (teamReport != null)
            {
                report.TeamCultureType = teamReport.CultureType;
                report.TeamReportReady = teamReport.IsReady;
                report.AllowPartialReport = teamReport.AllowPartialReport;
                report.TeamMembersCompleted = teamReport.CompletedMembers;
                report.TeamMembersTotal = teamReport.TotalMembers;

                var userTop5 = report.TopValues.Take(5).Select(v => v.ValueId).ToHashSet();
                var teamTop5 = teamReport.TopValues.Take(5).Select(v => v.ValueId).ToHashSet();

                report.AlignmentWithTeamTop5 = userTop5.Count == 0 || teamTop5.Count == 0
                    ? 0
                    : Math.Round((double)userTop5.Intersect(teamTop5).Count() / 5 * 100, 1);
            }
        }

        return Ok(report);
    }

    private static List<CategoryScoreDto> BuildCategoryProfileFromSelections(List<AssessmentValueSelection> selections)
    {
        if (!selections.Any())
            return new List<CategoryScoreDto>();

        var total = selections.Count;
        return selections
            .GroupBy(x => x.Value.Category)
            .Select(group => new CategoryScoreDto
            {
                Category = CultureAnalysisHelper.ToDisplayName(group.Key),
                Percentage = Math.Round((double)group.Count() / total * 100, 1)
            })
            .OrderByDescending(x => x.Percentage)
            .ToList();
    }

    private static List<ReflectionInsightDto> BuildReflectionInsightsFromCurrentAnswers(List<UserValue> selections, List<ReflectionAnswer> answers)
    {
        var insights = new List<ReflectionInsightDto>();

        foreach (var topValue in selections.OrderBy(v => v.Rank).Take(3))
        {
            var valueId = topValue.ValueId;
            var valueName = topValue.Value?.Name ?? $"Value {valueId}";

            var meaning = answers.FirstOrDefault(x => x.QuestionId == $"topvalue-{valueId}-meaning")?.ResponseText ?? string.Empty;
            var behavior = answers.FirstOrDefault(x => x.QuestionId == $"topvalue-{valueId}-behavior")?.ResponseText ?? string.Empty;
            var friction = answers.FirstOrDefault(x => x.QuestionId == $"topvalue-{valueId}-friction")?.ResponseText ?? string.Empty;

            if (!string.IsNullOrWhiteSpace(meaning) || !string.IsNullOrWhiteSpace(behavior) || !string.IsNullOrWhiteSpace(friction))
            {
                insights.Add(new ReflectionInsightDto
                {
                    ValueName = valueName,
                    Meaning = meaning,
                    Behavior = behavior,
                    Friction = friction
                });
            }
        }

        return insights;
    }

    private static List<ReflectionInsightDto> BuildReflectionInsightsFromAssessmentRun(List<AssessmentValueSelection> selections, List<AssessmentReflectionAnswer> answers)
    {
        var insights = new List<ReflectionInsightDto>();

        foreach (var topValue in selections.OrderBy(v => v.Rank).Take(3))
        {
            var valueId = topValue.ValueId;
            var valueName = topValue.Value?.Name ?? $"Value {valueId}";

            var meaning = answers.FirstOrDefault(x => x.QuestionId == $"topvalue-{valueId}-meaning")?.ResponseText ?? string.Empty;
            var behavior = answers.FirstOrDefault(x => x.QuestionId == $"topvalue-{valueId}-behavior")?.ResponseText ?? string.Empty;
            var friction = answers.FirstOrDefault(x => x.QuestionId == $"topvalue-{valueId}-friction")?.ResponseText ?? string.Empty;

            if (!string.IsNullOrWhiteSpace(meaning) || !string.IsNullOrWhiteSpace(behavior) || !string.IsNullOrWhiteSpace(friction))
            {
                insights.Add(new ReflectionInsightDto
                {
                    ValueName = valueName,
                    Meaning = meaning,
                    Behavior = behavior,
                    Friction = friction
                });
            }
        }

        return insights;
    }

    private static List<AssessmentQuestionAnswerDto> BuildReflectionQuestionsFromAnswers(List<AssessmentReflectionAnswer> answers)
    {
        return answers
            .Where(x => !string.IsNullOrWhiteSpace(x.ResponseText)
                && !x.QuestionId.StartsWith("topvalue-", StringComparison.OrdinalIgnoreCase))
            .OrderBy(x => x.QuestionId)
            .Select(x => new AssessmentQuestionAnswerDto
            {
                QuestionId = x.QuestionId,
                QuestionText = string.IsNullOrWhiteSpace(x.QuestionText) ? x.QuestionId : x.QuestionText,
                ResponseText = x.ResponseText
            })
            .ToList();
    }

    [HttpGet("dashboard")]
    public async Task<IActionResult> GetDashboard()
    {
        var userIdClaim = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
        if (!int.TryParse(userIdClaim, out var userId))
            return Unauthorized();

        var user = await _context.Users
            .Include(u => u.AssessmentRuns)
                .ThenInclude(r => r.ValueSelections)
                    .ThenInclude(vs => vs.Value)
            .Include(u => u.AssessmentRuns)
                .ThenInclude(r => r.ReflectionAnswers)
            .FirstOrDefaultAsync(u => u.Id == userId);

        if (user == null)
            return NotFound();

        var teamId = await _teamAccessService.GetCurrentTeamIdAsync(userId);
        var team = teamId.HasValue
            ? await _context.Teams.Include(t => t.Leader).FirstOrDefaultAsync(t => t.Id == teamId.Value)
            : null;

        var fit = await _culturalFitService.CalculateForUser(userId);

        var allowPartialReport = false;
        var teamMembersCompleted = 0;
        var teamMembersTotal = 0;
        var teamReportReady = false;
        var teamCultureType = string.Empty;
        var pendingJoinRequests = 0;

        if (teamId.HasValue)
        {
            var teamReport = await _teamCultureService.BuildTeamReport(teamId.Value);
            if (teamReport != null)
            {
                allowPartialReport = teamReport.AllowPartialReport;
                teamMembersCompleted = teamReport.CompletedMembers;
                teamMembersTotal = teamReport.TotalMembers;
                teamReportReady = teamReport.IsReady;
                teamCultureType = teamReport.CultureType;
            }

            if (user.IsAdmin || team?.LeaderId == user.Id)
            {
                pendingJoinRequests = await _context.TeamJoinRequests
                    .CountAsync(x => x.TeamId == teamId.Value && x.Status == "Pending");
            }
        }

        var completedRuns = user.AssessmentRuns
            .Where(x => x.Status == "Completed")
            .OrderByDescending(x => x.CompletedAtUtc ?? x.UpdatedAtUtc)
            .ToList();

        var dashboard = new DashboardDto
        {
            UserName = user.Name,
            AssessmentsCompleted = completedRuns.Count,
            LatestAssessmentCompletedAtUtc = completedRuns
                .Select(x => x.CompletedAtUtc ?? x.UpdatedAtUtc)
                .FirstOrDefault(),
            TeamId = teamId,
            TeamName = team?.Name,
            TeamCultureType = teamCultureType,
            CulturalFitScore = fit?.score,
            IsAdmin = user.IsAdmin,
            IsTeamLeader = team?.LeaderId == user.Id,
            TeamReportReady = teamReportReady,
            AllowPartialReport = allowPartialReport,
            TeamMembersCompleted = teamMembersCompleted,
            TeamMembersTotal = teamMembersTotal,
            TeamLeaderName = team?.Leader?.Name,
            InviteCode = team?.InviteCode,
            PendingJoinRequests = pendingJoinRequests,
            AssessmentHistory = completedRuns
                .Take(10)
                .Select(run => new AssessmentHistoryItemDto
                {
                    AssessmentRunId = run.Id,
                    CompletedAtUtc = run.CompletedAtUtc ?? run.UpdatedAtUtc,
                    TopValues = run.ValueSelections
                        .OrderBy(v => v.Rank)
                        .Take(5)
                        .Select(v => v.Value.Name)
                        .ToList(),
                    PrimaryCategory = run.ValueSelections
                        .OrderBy(v => v.Rank)
                        .Select(v => CultureAnalysisHelper.ToDisplayName(v.Value.Category))
                        .FirstOrDefault() ?? "Not available"
                })
                .ToList()
        };

        return Ok(dashboard);
    }

    [HttpGet("team/{teamId:int}")]
    public async Task<IActionResult> GetTeamReport(int teamId)
    {
        var currentUser = await _teamAccessService.GetCurrentUserAsync(User);
        if (currentUser == null)
            return Unauthorized();

        var isMember = await _teamAccessService.IsMemberOfTeamAsync(currentUser.Id, teamId);
        if (!(currentUser.IsAdmin || isMember))
            return Forbid();

        var team = await _context.Teams.Include(t => t.Leader).FirstOrDefaultAsync(t => t.Id == teamId);
        if (team == null)
            return NotFound();

        var report = await _teamCultureService.BuildTeamReport(teamId);
        if (report == null)
            return NotFound();

        if (!report.IsReady && !(currentUser.IsAdmin || team.LeaderId == currentUser.Id))
            return BadRequest("Team report is not ready yet.");

        return Ok(report);
    }

    private static List<string> BuildIndividualValueConflicts(Dictionary<ValueCategory, double> categoryProfile)
    {
        var conflicts = new List<string>();

        if (categoryProfile.GetValueOrDefault(ValueCategory.AutonomyAndFreedom) >= 0.20 &&
            categoryProfile.GetValueOrDefault(ValueCategory.StructureAndStability) >= 0.20)
        {
            conflicts.Add("conflicts between: Freedom vs structure. You value both freedom and structure – this may create inner tension.");
        }

        if (categoryProfile.GetValueOrDefault(ValueCategory.ResultAndPerformance) >= 0.20 &&
            categoryProfile.GetValueOrDefault(ValueCategory.RelationAndTrust) >= 0.20)
        {
            conflicts.Add("conflicts between: Result vs Relation. You value both results and relationships – balancing them can be challenging.");
        }

        if (categoryProfile.GetValueOrDefault(ValueCategory.DevelopmentAndInnovation) >= 0.18 &&
            categoryProfile.GetValueOrDefault(ValueCategory.StructureAndStability) >= 0.18)
        {
            conflicts.Add("conflicts between: Innovation vs stability. You value both innovation and stability – this may pull you in different directions.");
        }
        if (categoryProfile.GetValueOrDefault(ValueCategory.StructureAndStability) >= 0.18 &&
            categoryProfile.GetValueOrDefault(ValueCategory.RelationAndTrust) >= 0.18)
        {
            conflicts.Add("conflicts between: Control vs Trust. You value both control and trust – this can create mixed expectations.");
                
        }
        if (categoryProfile.GetValueOrDefault(ValueCategory.AutonomyAndFreedom) >= 0.18 &&
            categoryProfile.GetValueOrDefault(ValueCategory.StructureAndStability) >= 0.18)
        {
            conflicts.Add("conflicts between: Freedom and Stability. you may sometimes feel pulled between adapting freely and following a clear plan.");
        }

        if (categoryProfile.GetValueOrDefault(ValueCategory.DevelopmentAndInnovation) >= 0.18 &&
            categoryProfile.GetValueOrDefault(ValueCategory.StructureAndStability) >= 0.18)
        {
            conflicts.Add("conflicts between: Development and Stability. This may create tension between trying new things and keeping reliable routines.");
        }

        if (categoryProfile.GetValueOrDefault(ValueCategory.ResultAndPerformance) >= 0.18 &&
            categoryProfile.GetValueOrDefault(ValueCategory.MeaningAndPurpose) >= 0.18)
        {
            conflicts.Add("conflicts between: Performance and Purpose. This may create tension between short-term results and long-term meaning.");
        }
        if (!conflicts.Any())
        {
            conflicts.Add("No conflicts detected yet.");
        }

        return conflicts;
    }
}
