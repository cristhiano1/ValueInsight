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
            .Include(t => t.Users)
                .ThenInclude(u => u.UserValues)
                    .ThenInclude(uv => uv.Value)
            .FirstOrDefaultAsync(t => t.Id == teamId);

        if (team == null)
            return null;

        var usersWithValues = team.Users.Where(u => u.UserValues.Any()).ToList();
        var allUserValues = usersWithValues.SelectMany(u => u.UserValues).ToList();

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

        var dispersion = teamProfile.Values.Any()
            ? teamProfile.Values.Max() - teamProfile.Values.Min()
            : 0;

        var totalUsers = team.Users.Count;
        var completedUsers = usersWithValues.Count;

        var completionRate = totalUsers == 0
            ? 0
            : (double)completedUsers / totalUsers * 100;

        var sharedValues = valueFrequency
            .Where(v => v.Rank == usersWithValues.Count && usersWithValues.Count > 0)
            .Select(v => v.Name)
            .ToList();

        var tensionFields = new List<string>();

        if (teamProfile.GetValueOrDefault(ValueCategory.AutonomyAndFreedom) >= 0.22 &&
            teamProfile.GetValueOrDefault(ValueCategory.StructureAndStability) >= 0.22)
            tensionFields.Add("Autonomy vs structure");

        if (teamProfile.GetValueOrDefault(ValueCategory.ResultAndPerformance) >= 0.22 &&
            teamProfile.GetValueOrDefault(ValueCategory.RelationAndTrust) >= 0.22)
            tensionFields.Add("Results vs relationships");

        if (teamProfile.GetValueOrDefault(ValueCategory.DevelopmentAndInnovation) >= 0.20 &&
            teamProfile.GetValueOrDefault(ValueCategory.StructureAndStability) >= 0.20)
            tensionFields.Add("Innovation vs stability");

        if (!tensionFields.Any())
            tensionFields.Add("No major tension field detected yet");

        return new TeamReportDto
        {
            TeamId = team.Id,
            TeamName = team.Name,
            TeamSize = totalUsers,
            CultureType = CultureAnalysisHelper.ClassifyCultureType(teamProfile),
            AlignmentScore = Math.Round(alignment * 100, 1),
            PolarizationScore = Math.Round(polarization * 100, 1),
            MaturityIndex = maturity,
            DispersionScore = Math.Round(dispersion * 100, 1),
            CompletionRate = Math.Round(completionRate, 1),
            CategoryProfile = teamProfile.Select(kvp => new CategoryScoreDto
            {
                Category = CultureAnalysisHelper.ToDisplayName(kvp.Key),
                Percentage = Math.Round(kvp.Value * 100, 1)
            }).OrderByDescending(x => x.Percentage).ToList(),
            TopValues = valueFrequency.Take(5).ToList(),
            LowestValues = valueFrequency
                .OrderBy(v => v.Rank)
                .ThenBy(v => v.Name)
                .Take(5)
                .ToList(),
            SharedCoreValues = sharedValues,
            TensionFields = tensionFields
        };
    }
}