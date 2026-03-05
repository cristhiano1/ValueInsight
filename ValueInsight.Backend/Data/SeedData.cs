using ValueInsight.Backend.Models;

namespace ValueInsight.Backend.Data;

public static class SeedData
{
    public static readonly Value[] Values =
    {
        new Value { Id = 1, Name = "Trust", Category = ValueCategory.RelationAndTrust },
        new Value { Id = 2, Name = "Respect", Category = ValueCategory.RelationAndTrust },
        new Value { Id = 3, Name = "Transparency", Category = ValueCategory.RelationAndTrust },

        new Value { Id = 4, Name = "Results", Category = ValueCategory.ResultAndPerformance },
        new Value { Id = 5, Name = "Efficiency", Category = ValueCategory.ResultAndPerformance },

        new Value { Id = 6, Name = "Structure", Category = ValueCategory.StructureAndStability },

        new Value { Id = 7, Name = "Freedom", Category = ValueCategory.AutonomyAndFreedom },

        new Value { Id = 8, Name = "Innovation", Category = ValueCategory.DevelopmentAndInnovation },

        new Value { Id = 9, Name = "Purpose", Category = ValueCategory.MeaningAndPurpose }
    };
}