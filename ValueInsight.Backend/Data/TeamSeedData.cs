using ValueInsight.Backend.Models;

namespace ValueInsight.Backend.Data
{
    public static class TeamSeedData
    {
        public static readonly Team[] Teams =
        {
            new Team { Id = 1, Name = "Engineering" },
            new Team { Id = 2, Name = "Product" },
            new Team { Id = 3, Name = "Design" },
            new Team { Id = 4, Name = "Marketing" },
            new Team { Id = 5, Name = "Operations" }
        };
    }
}