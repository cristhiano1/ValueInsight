using System.Security.Claims;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using ValueInsight.Backend.Data;
using ValueInsight.Backend.Dtos;
using ValueInsight.Backend.Helpers;
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

    public ReportsController(
        ValueInsightDbContext context,
        TeamCultureService teamCultureService,
        CulturalFitService culturalFitService)
    {
        _context = context;
        _teamCultureService = teamCultureService;
        _culturalFitService = culturalFitService;
    }

    [HttpGet("me")]
    public async Task<IActionResult> GetMyReport()
    {
        var userIdClaim = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
        if (!int.TryParse(userIdClaim, out var userId))
            return Unauthorized();

        var user = await _context.Users
            .Include(u => u.Team)
            .Include(u => u.UserValues)
                .ThenInclude(uv => uv.Value)
            .Include(u => u.Team)
                .ThenInclude(t => t!.Users)
            .FirstOrDefaultAsync(u => u.Id == userId);

        if (user == null)
            return NotFound();

        var userValues = user.UserValues.OrderBy(uv => uv.Rank).ToList();
        var categoryProfile = CultureAnalysisHelper.BuildNormalizedCategoryProfile(userValues);
        var report = new PersonalReportDto
        {
            UserId = user.Id,
            UserName = user.Name,
            TeamId = user.TeamId,
            TeamName = user.Team?.Name,
            TopValues = userValues.Select(uv => new RankedValueDto
            {
                ValueId = uv.ValueId,
                Name = uv.Value.Name,
                Category = CultureAnalysisHelper.ToDisplayName(uv.Value.Category),
                Rank = uv.Rank
            }).ToList(),
            CategoryProfile = categoryProfile.Select(kvp => new CategoryScoreDto
            {
                Category = CultureAnalysisHelper.ToDisplayName(kvp.Key),
                Percentage = Math.Round(kvp.Value * 100, 1)
            }).OrderByDescending(x => x.Percentage).ToList(),
        };

        if (userValues.Any())
        {
            foreach (var value in userValues.Take(3))
            {
                var prefix = $"topvalue-{value.ValueId}";
                var answers = await _context.ReflectionAnswers
                    .Where(x => x.UserId == userId && x.QuestionId.StartsWith(prefix))
                    .ToListAsync();

                report.ReflectionInsights.Add(new ReflectionInsightDto
                {
                    ValueName = value.Value.Name,
                    Meaning = answers.FirstOrDefault(x => x.QuestionId.EndsWith("-meaning"))?.ResponseText ?? string.Empty,
                    Behavior = answers.FirstOrDefault(x => x.QuestionId.EndsWith("-behavior"))?.ResponseText ?? string.Empty,
                    Friction = answers.FirstOrDefault(x => x.QuestionId.EndsWith("-friction"))?.ResponseText ?? string.Empty,
                });
            }

            report.CoachingQuestions = userValues.Take(3)
                .Select(v => $"How can you express {v.Value.Name.ToLower()} more clearly in your leadership this week?")
                .Concat(new[]
                {
                    "Where do your current goals support your top values — and where do they conflict?",
                    "What conversation would reduce the biggest value tension you are experiencing right now?"
                })
                .ToList();
        }

        if (user.TeamId.HasValue)
        {
            var teamReport = await _teamCultureService.BuildTeamReport(user.TeamId.Value);
            var fit = await _culturalFitService.CalculateForUser(userId);
            if (teamReport != null)
            {
                report.TeamCultureType = teamReport.CultureType;
                report.AlignmentWithTeamTop5 = teamReport.AlignmentScore;
            }

            if (fit.HasValue)
            {
                report.CulturalFitScore = fit.Value.score;
                report.CulturalFitLabel = fit.Value.label;
            }
        }

        return Ok(report);
    }


    [HttpGet("dashboard")]
    public async Task<IActionResult> GetDashboard()
    {
        var userIdClaim = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
        if (!int.TryParse(userIdClaim, out var userId))
            return Unauthorized();

        var user = await _context.Users
            .Include(u => u.Team)
            .FirstOrDefaultAsync(u => u.Id == userId);

        if (user == null)
            return NotFound();

        var runs = await _context.AssessmentRuns
            .Where(x => x.UserId == userId)
            .Include(x => x.ValueSelections)
                .ThenInclude(x => x.Value)
            .OrderByDescending(x => x.CreatedAtUtc)
            .ToListAsync();

        var history = runs.Select(run => new AssessmentHistoryItemDto
        {
            AssessmentRunId = run.Id,
            CompletedAtUtc = run.CreatedAtUtc,
            TopValues = run.ValueSelections
                .OrderBy(x => x.Rank)
                .Take(3)
                .Select(x => x.Value.Name)
                .ToList(),
            PrimaryCategory = run.ValueSelections
                .OrderBy(x => x.Rank)
                .Select(x => CultureAnalysisHelper.ToDisplayName(x.Value.Category))
                .FirstOrDefault() ?? string.Empty
        }).ToList();

        double? fitScore = null;
        string teamCulture = string.Empty;

        if (user.TeamId.HasValue)
        {
            var fit = await _culturalFitService.CalculateForUser(userId);
            if (fit.HasValue)
                fitScore = fit.Value.score;

            var teamReport = await _teamCultureService.BuildTeamReport(user.TeamId.Value);
            if (teamReport != null)
                teamCulture = teamReport.CultureType;
        }


        var dto = new DashboardDto
        {
            UserName = user.Name,
            AssessmentsCompleted = history.Count,
            LatestAssessmentCompletedAtUtc = history.FirstOrDefault()?.CompletedAtUtc,
            TeamId = user.TeamId,
            TeamName = user.Team?.Name,
            TeamCultureType = teamCulture,
            CulturalFitScore = fitScore,
            AssessmentHistory = history
        };

        return Ok(dto);
    }

    [HttpGet("team/{teamId:int}")]
    public async Task<IActionResult> GetTeamReport(int teamId)
    {
        var report = await _teamCultureService.BuildTeamReport(teamId);
        if (report == null)
            return NotFound();

        return Ok(report);
    }
}
