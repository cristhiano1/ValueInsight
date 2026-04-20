using ValueInsight.Backend.Models;

namespace ValueInsight.Backend.Data
{
    public static class TeamSeedData
    {
        public static readonly Team[] Teams =
        {
            new Team { Id = 1, Name = "Engineering", InviteCode = "ENG001", AllowPartialReport = false },
            new Team { Id = 2, Name = "Product", InviteCode = "PROD001", AllowPartialReport = false },
            new Team { Id = 3, Name = "Design", InviteCode = "DES001", AllowPartialReport = false },
            new Team { Id = 4, Name = "Marketing", InviteCode = "MKT001", AllowPartialReport = false },
            new Team { Id = 5, Name = "Operations", InviteCode = "OPS001", AllowPartialReport = false }
        };
    }
}
