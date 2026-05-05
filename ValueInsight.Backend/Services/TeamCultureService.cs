using Microsoft.EntityFrameworkCore;
using ValueInsight.Backend.Data;
using ValueInsight.Backend.Dtos;
using ValueInsight.Backend.Helpers;
using ValueInsight.Backend.Models;

namespace ValueInsight.Backend.Services;

public class TeamCultureService
{
    private readonly ValueInsightDbContext _context;

    public TeamCultureService(ValueInsightDbContext context)
    {
        _context = context;
    }

    public async Task<TeamCultureResponseDtos?> AnalyzeTeamCulture(int teamId)
    {
        var report = await BuildTeamReport(teamId);
        if (report == null)
            return null;

        return new TeamCultureResponseDtos
        {
            TeamId = report.TeamId,
            TeamName = report.TeamName,
            DominantValues = report.TopValues.Take(3).Select(v => v.Name).ToList()
        };
    }

    public async Task<TeamReportDto?> BuildTeamReport(int teamId)
    {
        var team = await _context.Teams
            .Include(t => t.Leader)
            .Include(t => t.Members)
                .ThenInclude(tm => tm.User)
                    .ThenInclude(u => u.UserValues)
                        .ThenInclude(uv => uv.Value)
            .Include(t => t.Members)
                .ThenInclude(tm => tm.User)
                    .ThenInclude(u => u.AssessmentRuns)
                        .ThenInclude(r => r.ValueSelections)
                            .ThenInclude(vs => vs.Value)
            .FirstOrDefaultAsync(t => t.Id == teamId);

        if (team == null)
            return null;

        foreach (var member in team.Members)
        {
            member.HasCompletedAssessment = member.User.AssessmentRuns.Any(r => r.Status == "Completed");
        }

        var memberProgress = team.Members
            .OrderBy(m => m.User.Name)
            .Select(m => new TeamMemberProgressDto
            {
                UserId = m.UserId,
                UserName = m.User.Name,
                HasCompletedAssessment = m.HasCompletedAssessment,
                IsLeader = team.LeaderId == m.UserId
            })
            .ToList();

        var completedMembers = memberProgress.Count(x => x.HasCompletedAssessment);
        var totalMembers = memberProgress.Count;
        var reportReady = totalMembers > 0 && (completedMembers == totalMembers || team.AllowPartialReport);

        var usersWithValues = team.Members.Where(m => m.User.UserValues.Any()).Select(m => m.User).ToList();
        var allUserValues = usersWithValues.SelectMany(u => u.UserValues).ToList();

        var teamMetrics = BuildMetricsFromUserValues(usersWithValues, allUserValues);

        var history = BuildTeamHistory(team.Members.Select(m => m.User).ToList());
        var historySummary = BuildHistorySummary(history, teamMetrics.TopValues, teamMetrics.AlignmentScore, teamMetrics.PolarizationScore, teamMetrics.MaturityIndex);

        var tensionFields = BuildTensionFields(teamMetrics.TeamProfile);

        return new TeamReportDto
        {
            TeamId = team.Id,
            TeamName = team.Name,
            TeamSize = usersWithValues.Count,
            CultureType = teamMetrics.CultureType,
            AlignmentScore = teamMetrics.AlignmentScore,
            PolarizationScore = teamMetrics.PolarizationScore,
            MaturityIndex = teamMetrics.MaturityIndex,
            IsReady = reportReady,
            AllowPartialReport = team.AllowPartialReport,
            CompletedMembers = completedMembers,
            TotalMembers = totalMembers,
            LeaderName = team.Leader?.Name ?? string.Empty,
            CategoryProfile = teamMetrics.CategoryProfile,
            TopValues = teamMetrics.TopValues,
            LowestValues = teamMetrics.LowestValues,
            ValueFrequency = teamMetrics.ValueFrequency,
            SharedCoreValues = teamMetrics.SharedCoreValues,
            TensionFields = tensionFields,
            History = history,
            HistorySummary = historySummary,
            Members = memberProgress
        };
    }

    private static TeamMetrics BuildMetricsFromUserValues(List<User> usersWithValues, List<UserValue> allUserValues)
    {
        var emptyProfile = Enum.GetValues<ValueCategory>()
            .ToDictionary(c => c, _ => 0.0);

        var teamProfile = allUserValues.Any()
            ? CultureAnalysisHelper.BuildNormalizedCategoryProfile(allUserValues)
            : emptyProfile;

        var valueFrequency = allUserValues
            .GroupBy(v => new { v.ValueId, v.Value.Name, Category = CultureAnalysisHelper.ToDisplayName(v.Value.Category) })
            .Select(g => new RankedValueDto
            {
                ValueId = g.Key.ValueId,
                Name = g.Key.Name,
                Category = g.Key.Category,
                Rank = g.Count()
            })
            .OrderByDescending(v => v.Rank)
            .ThenBy(v => v.Name)
            .ToList();

        var top5Lists = usersWithValues
            .Select(u => u.UserValues.OrderBy(uv => uv.Rank).Take(5).Select(uv => uv.ValueId).ToList())
            .Where(x => x.Count == 5)
            .ToList();

        var alignment = CultureAnalysisHelper.CalculateAlignmentScore(top5Lists);
        var polarization = CultureAnalysisHelper.CalculatePolarization(teamProfile);
        var maturity = CultureAnalysisHelper.CalculateMaturityIndex(alignment, polarization, teamProfile);
        var sharedValues = valueFrequency.Where(v => v.Rank == usersWithValues.Count && usersWithValues.Count > 0)
            .Select(v => v.Name)
            .ToList();

        return new TeamMetrics
        {
            TeamProfile = teamProfile,
            CultureType = CultureAnalysisHelper.ClassifyCultureType(teamProfile),
            AlignmentScore = Math.Round(alignment * 100, 1),
            PolarizationScore = Math.Round(polarization * 100, 1),
            MaturityIndex = maturity,
            CategoryProfile = teamProfile.Select(kvp => new CategoryScoreDto
            {
                Category = CultureAnalysisHelper.ToDisplayName(kvp.Key),
                Percentage = Math.Round(kvp.Value * 100, 1)
            }).OrderByDescending(x => x.Percentage).ToList(),
            TopValues = valueFrequency.Take(5).ToList(),
            LowestValues = valueFrequency.OrderBy(v => v.Rank).ThenBy(v => v.Name).Take(5).ToList(),
            ValueFrequency = valueFrequency,
            SharedCoreValues = sharedValues
        };
    }

    private static List<string> BuildTensionFields(Dictionary<ValueCategory, double> teamProfile)
    {
        var tensionFields = new List<string>();

        if (teamProfile.GetValueOrDefault(ValueCategory.AutonomyAndFreedom) >= 0.22 && teamProfile.GetValueOrDefault(ValueCategory.StructureAndStability) >= 0.22)
            tensionFields.Add("Autonomy vs structure – this may create different expectations on how work should be done.");
        if (teamProfile.GetValueOrDefault(ValueCategory.ResultAndPerformance) >= 0.22 && teamProfile.GetValueOrDefault(ValueCategory.RelationAndTrust) >= 0.22)
            tensionFields.Add("Results vs relationships – balancing performance and collaboration may be a challenge.");
        if (teamProfile.GetValueOrDefault(ValueCategory.DevelopmentAndInnovation) >= 0.20 && teamProfile.GetValueOrDefault(ValueCategory.StructureAndStability) >= 0.20)
            tensionFields.Add("Innovation vs stability – this may create tension between change and consistency.");
        if (teamProfile.GetValueOrDefault(ValueCategory.StructureAndStability) >= 0.20 && teamProfile.GetValueOrDefault(ValueCategory.RelationAndTrust) >= 0.20)
            tensionFields.Add("Control vs Trust - this can create mixed expectations.");
        if (teamProfile.GetValueOrDefault(ValueCategory.AutonomyAndFreedom) >= 0.18 &&
            teamProfile.GetValueOrDefault(ValueCategory.StructureAndStability) >= 0.18)
        {
            tensionFields.Add("Freedom vs Stability – preferences for structure vs adaptability may vary.");
        }

        if (teamProfile.GetValueOrDefault(ValueCategory.DevelopmentAndInnovation) >= 0.18 &&
            teamProfile.GetValueOrDefault(ValueCategory.StructureAndStability) >= 0.18)
        {
            tensionFields.Add("Innovation vs Stability – trying new things vs following routines may create friction.");
        }

        if (teamProfile.GetValueOrDefault(ValueCategory.ResultAndPerformance) >= 0.18 &&
            teamProfile.GetValueOrDefault(ValueCategory.MeaningAndPurpose) >= 0.18)
        {
            tensionFields.Add("Performance vs Purpose – short-term results and long-term meaning may compete.");
        }
        if (!tensionFields.Any())
            tensionFields.Add("No Value conflicts detected yet");

        return tensionFields;
    }

    private static List<TeamHistoryItemDto> BuildTeamHistory(List<User> teamUsers)
    {
        var completedRuns = teamUsers
            .SelectMany(u => u.AssessmentRuns)
            .Where(r => r.Status == "Completed" && r.ValueSelections.Any())
            .OrderBy(r => r.CompletedAtUtc ?? r.UpdatedAtUtc)
            .ToList();

        if (!completedRuns.Any())
            return new List<TeamHistoryItemDto>();

        var snapshotDates = completedRuns
            .Select(r => r.CompletedAtUtc ?? r.UpdatedAtUtc)
            .Distinct()
            .OrderByDescending(d => d)
            .Take(8)
            .OrderBy(d => d)
            .ToList();

        var history = new List<TeamHistoryItemDto>();

        foreach (var snapshotDate in snapshotDates)
        {
            var latestRunsAtDate = teamUsers
                .Select(user => user.AssessmentRuns
                    .Where(r => r.Status == "Completed" && r.ValueSelections.Any() && (r.CompletedAtUtc ?? r.UpdatedAtUtc) <= snapshotDate)
                    .OrderByDescending(r => r.CompletedAtUtc ?? r.UpdatedAtUtc)
                    .FirstOrDefault())
                .Where(r => r != null)
                .Cast<AssessmentRun>()
                .ToList();

            if (!latestRunsAtDate.Any())
                continue;

            var usersAtDate = latestRunsAtDate
                .Select(run => new User
                {
                    Id = run.UserId,
                    UserValues = run.ValueSelections
                        .OrderBy(v => v.Rank)
                        .Select(v => new UserValue
                        {
                            UserId = run.UserId,
                            ValueId = v.ValueId,
                            Rank = v.Rank,
                            Value = v.Value
                        })
                        .ToList()
                })
                .Where(u => u.UserValues.Any())
                .ToList();

            var allValuesAtDate = usersAtDate.SelectMany(u => u.UserValues).ToList();
            if (!allValuesAtDate.Any())
                continue;

            var metrics = BuildMetricsFromUserValues(usersAtDate, allValuesAtDate);

            history.Add(new TeamHistoryItemDto
            {
                SnapshotDateUtc = snapshotDate,
                TopValues = metrics.TopValues.Select(x => x.Name).ToList(),
                AlignmentScore = metrics.AlignmentScore,
                PolarizationScore = metrics.PolarizationScore,
                MaturityIndex = metrics.MaturityIndex,
                CultureType = metrics.CultureType,
                TeamSize = usersAtDate.Count
            });
        }

        return history.OrderByDescending(x => x.SnapshotDateUtc).ToList();
    }

    private static TeamHistorySummaryDto BuildHistorySummary(
        List<TeamHistoryItemDto> history,
        List<RankedValueDto> currentTopValues,
        double currentAlignment,
        double currentPolarization,
        double currentMaturity)
    {
        var previous = history.Skip(1).FirstOrDefault() ?? history.FirstOrDefault();
        if (previous == null)
        {
            return new TeamHistorySummaryDto
            {
                HasHistory = false,
                CurrentTopValues = currentTopValues.Select(x => x.Name).ToList()
            };
        }

        return new TeamHistorySummaryDto
        {
            HasHistory = history.Count > 1,
            CurrentTopValues = currentTopValues.Select(x => x.Name).ToList(),
            PreviousTopValues = previous.TopValues,
            AlignmentChange = Math.Round(currentAlignment - previous.AlignmentScore, 1),
            PolarizationChange = Math.Round(currentPolarization - previous.PolarizationScore, 1),
            MaturityChange = Math.Round(currentMaturity - previous.MaturityIndex, 1)
        };
    }

    private sealed class TeamMetrics
    {
        public Dictionary<ValueCategory, double> TeamProfile { get; set; } = new();
        public string CultureType { get; set; } = string.Empty;
        public double AlignmentScore { get; set; }
        public double PolarizationScore { get; set; }
        public double MaturityIndex { get; set; }
        public List<CategoryScoreDto> CategoryProfile { get; set; } = new();
        public List<RankedValueDto> TopValues { get; set; } = new();
        public List<RankedValueDto> LowestValues { get; set; } = new();
        public List<RankedValueDto> ValueFrequency { get; set; } = new();
        public List<string> SharedCoreValues { get; set; } = new();
    }
}
