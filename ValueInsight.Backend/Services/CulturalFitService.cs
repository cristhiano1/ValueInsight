using Microsoft.EntityFrameworkCore;
using ValueInsight.Backend.Data;
using ValueInsight.Backend.Dtos;
using ValueInsight.Backend.Helpers;
using ValueInsight.Backend.Models;

namespace ValueInsight.Backend.Services;

public class CulturalFitService
{
    private readonly ValueInsightDbContext _context;

    public CulturalFitService(ValueInsightDbContext context)
    {
        _context = context;
    }

    public async Task<CulturalFitResponseDtos?> CalculateTeamAlignment(int teamId)
    {
        var team = await _context.Teams
            .Include(t => t.Users)
                .ThenInclude(u => u.UserValues)
            .FirstOrDefaultAsync(t => t.Id == teamId);

        if (team == null)
            return null;

        var top5Lists = team.Users
            .Select(u => u.UserValues.OrderBy(uv => uv.Rank).Take(5).Select(uv => uv.ValueId).ToList())
            .Where(x => x.Count == 5)
            .ToList();

        return new CulturalFitResponseDtos
        {
            TeamId = teamId,
            AlignmentScore = Math.Round(CultureAnalysisHelper.CalculateAlignmentScore(top5Lists) * 100, 1)
        };
    }

    public async Task<(double score, string label, double categoryAlignment, double topOverlap, double dominanceMatch, double tensionScore)?> CalculateForUser(int userId)
    {
        var user = await _context.Users
            .Include(u => u.Team)
            .Include(u => u.UserValues)
                .ThenInclude(uv => uv.Value)
            .FirstOrDefaultAsync(u => u.Id == userId);

        if (user?.TeamId == null)
            return null;

        var teammates = await _context.Users
            .Where(u => u.TeamId == user.TeamId && u.Id != user.Id)
            .Include(u => u.UserValues)
                .ThenInclude(uv => uv.Value)
            .ToListAsync();

        if (!user.UserValues.Any() || !teammates.Any())
            return null;

        var teamValues = teammates.SelectMany(t => t.UserValues).ToList();
        if (!teamValues.Any())
            return null;

        var userProfile = CultureAnalysisHelper.BuildNormalizedCategoryProfile(user.UserValues);
        var teamProfile = CultureAnalysisHelper.BuildNormalizedCategoryProfile(teamValues);

        var categoryAlignment = CultureAnalysisHelper.CategoryAlignment(userProfile, teamProfile);
        var userTop3 = CultureAnalysisHelper.TopCategories(userProfile);
        var teamTop3 = CultureAnalysisHelper.TopCategories(teamProfile);
        var topOverlap = CultureAnalysisHelper.TopOverlap(userTop3, teamTop3);
        var dominantCategory = CultureAnalysisHelper.TopCategories(teamProfile, 1).FirstOrDefault();
        var dominanceMatch = userTop3.Contains(dominantCategory) ? 1.0 : 0.0;
        var tensionScore = CultureAnalysisHelper.TensionScore(userProfile, teamProfile);

        var finalScore = (0.40 * categoryAlignment) + (0.30 * topOverlap) + (0.15 * dominanceMatch) + (0.15 * tensionScore);
        finalScore = Math.Round(finalScore * 100, 1);

        return (finalScore, CultureAnalysisHelper.InterpretFit(finalScore), categoryAlignment, topOverlap, dominanceMatch, tensionScore);
    }
}
