using ValueInsight.Backend.Models;

namespace ValueInsight.Backend.Helpers;

public static class CultureAnalysisHelper
{
    private static readonly Dictionary<ValueCategory, double> CategoryWeights = new()
    {
        [ValueCategory.RelationAndTrust] = 1.00,
        [ValueCategory.ResultAndPerformance] = 1.00,
        [ValueCategory.StructureAndStability] = 1.00,
        [ValueCategory.AutonomyAndFreedom] = 1.00,
        [ValueCategory.DevelopmentAndInnovation] = 1.00,
        [ValueCategory.MeaningAndPurpose] = 1.00,
    };

    // MODIFICADO: Lista ampliada de pares en tensión/conflicto
    private static readonly (ValueCategory Left, ValueCategory Right)[] TensionPairs =
    {
        // Originales
        (ValueCategory.AutonomyAndFreedom, ValueCategory.StructureAndStability),
        (ValueCategory.ResultAndPerformance, ValueCategory.RelationAndTrust),
        (ValueCategory.StructureAndStability, ValueCategory.DevelopmentAndInnovation),
        (ValueCategory.StructureAndStability, ValueCategory.RelationAndTrust),

        // Ampliados
        (ValueCategory.MeaningAndPurpose, ValueCategory.ResultAndPerformance),
        (ValueCategory.DevelopmentAndInnovation, ValueCategory.MeaningAndPurpose),
        (ValueCategory.AutonomyAndFreedom, ValueCategory.RelationAndTrust),
        (ValueCategory.AutonomyAndFreedom, ValueCategory.ResultAndPerformance),
        (ValueCategory.MeaningAndPurpose, ValueCategory.StructureAndStability),
        (ValueCategory.DevelopmentAndInnovation, ValueCategory.RelationAndTrust),
    };

    public static Dictionary<ValueCategory, double> BuildNormalizedCategoryProfile(IEnumerable<UserValue> values)
    {
        var rankedValues = values.OrderBy(v => v.Rank).ToList();
        var weighted = Enum.GetValues<ValueCategory>().ToDictionary(c => c, _ => 0.0);

        if (!rankedValues.Any())
            return weighted;

        foreach (var item in rankedValues)
        {
            var rankWeight = Math.Max(1, 6 - item.Rank);
            weighted[item.Value.Category] += rankWeight * CategoryWeights[item.Value.Category];
        }

        var total = weighted.Values.Sum();
        if (total <= 0)
            return weighted;

        return weighted.ToDictionary(kvp => kvp.Key, kvp => Math.Round(kvp.Value / total, 4));
    }

    public static double CategoryAlignment(Dictionary<ValueCategory, double> user, Dictionary<ValueCategory, double> team)
    {
        var diffSum = Enum.GetValues<ValueCategory>().Sum(category => Math.Abs(user.GetValueOrDefault(category) - team.GetValueOrDefault(category)));
        var alignment = 1 - (diffSum / 2.0);
        return Clamp01(alignment);
    }

    public static double TopOverlap(IEnumerable<ValueCategory> userTop3, IEnumerable<ValueCategory> teamTop3)
    {
        var overlap = userTop3.Distinct().Intersect(teamTop3.Distinct()).Count();
        return Clamp01(overlap / 3.0);
    }

    public static double TensionScore(Dictionary<ValueCategory, double> user, Dictionary<ValueCategory, double> team)
    {
        if (TensionPairs.Length == 0)
            return 1;

        var pairScores = new List<double>();
        foreach (var (left, right) in TensionPairs)
        {
            var userDelta = user.GetValueOrDefault(left) - user.GetValueOrDefault(right);
            var teamDelta = team.GetValueOrDefault(left) - team.GetValueOrDefault(right);
            var difference = Math.Abs(userDelta - teamDelta);
            pairScores.Add(Clamp01(1 - difference));
        }

        return Math.Round(pairScores.Average(), 4);
    }

    public static List<ValueCategory> TopCategories(Dictionary<ValueCategory, double> profile, int take = 3)
        => profile.OrderByDescending(x => x.Value).Take(take).Select(x => x.Key).ToList();

    public static string ClassifyCultureType(Dictionary<ValueCategory, double> profile)
    {
        var topTwo = TopCategories(profile, 2).ToHashSet();

        if (topTwo.SetEquals(new[] { ValueCategory.ResultAndPerformance, ValueCategory.StructureAndStability })) return "Performance culture";
        if (topTwo.SetEquals(new[] { ValueCategory.RelationAndTrust, ValueCategory.MeaningAndPurpose })) return "Relationship culture";
        if (topTwo.SetEquals(new[] { ValueCategory.DevelopmentAndInnovation, ValueCategory.AutonomyAndFreedom })) return "Innovation culture";
        if (topTwo.SetEquals(new[] { ValueCategory.StructureAndStability, ValueCategory.RelationAndTrust })) return "Stability culture";
        if (topTwo.SetEquals(new[] { ValueCategory.ResultAndPerformance, ValueCategory.AutonomyAndFreedom })) return "Entrepreneurial results culture";
        if (topTwo.SetEquals(new[] { ValueCategory.MeaningAndPurpose, ValueCategory.DevelopmentAndInnovation })) return "Purpose-driven development culture";

        return "Balanced culture";
    }

    public static double CalculateAlignmentScore(IEnumerable<List<int>> teamTop5Lists)
    {
        var topLists = teamTop5Lists.Where(x => x.Count == 5).ToList();
        if (!topLists.Any()) return 0;

        var allPairs = new List<double>();
        for (var i = 0; i < topLists.Count; i++)
        {
            for (var j = i + 1; j < topLists.Count; j++)
            {
                var overlap = topLists[i].Intersect(topLists[j]).Count() / 5.0;
                allPairs.Add(overlap);
            }
        }

        if (!allPairs.Any()) return 1;
        return Math.Round(allPairs.Average(), 4);
    }

    public static double CalculatePolarization(Dictionary<ValueCategory, double> profile)
    {
        var values = profile.Values.ToList();
        if (!values.Any()) return 0;

        var average = values.Average();
        var variance = values.Sum(v => Math.Pow(v - average, 2)) / values.Count;
        return Math.Round(Math.Sqrt(variance), 4);
    }

    public static double CalculateMaturityIndex(double alignment, double polarization, Dictionary<ValueCategory, double> profile)
    {
        var dominance = profile.Values.Max();
        var balancePenalty = profile.Values.Min();
        var score = (0.45 * alignment) + (0.30 * (1 - polarization)) + (0.20 * dominance) + (0.05 * balancePenalty);
        return Math.Round(Math.Clamp(score, 0, 1) * 100, 1);
    }

    public static string InterpretFit(double score)
    {
        if (score >= 80) return "Strong cultural match";
        if (score >= 60) return "Good match";
        if (score >= 40) return "Moderate tension";
        if (score >= 20) return "Significant values tension";
        return "High risk of values stress";
    }

    public static string ToDisplayName(ValueCategory category) => category switch
    {
        ValueCategory.RelationAndTrust => "Relation & Trust",
        ValueCategory.ResultAndPerformance => "Result & Performance",
        ValueCategory.StructureAndStability => "Structure & Stability",
        ValueCategory.AutonomyAndFreedom => "Autonomy & Freedom",
        ValueCategory.DevelopmentAndInnovation => "Development & Innovation",
        ValueCategory.MeaningAndPurpose => "Meaning & Purpose",
        _ => category.ToString()
    };

    private static double Clamp01(double value) => Math.Clamp(value, 0, 1);
}