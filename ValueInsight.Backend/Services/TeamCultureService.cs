using Microsoft.EntityFrameworkCore;
using ValueInsight.Backend.Data;
using ValueInsight.Backend.Dtos;

namespace ValueInsight.Backend.Services
{
    public class TeamCultureService
    {
        private readonly ValueInsightDbContext _context;

        public TeamCultureService(ValueInsightDbContext context)
        {
            _context = context;
        }

        public async Task<TeamCultureResponseDtos?> AnalyzeTeamCulture(int teamId)
        {
            var team = await _context.Teams
                .Include(t => t.Users)
                    .ThenInclude(u => u.UserValues)
                        .ThenInclude(uv => uv.Value)
                .FirstOrDefaultAsync(t => t.Id == teamId);

            if (team == null)
                return null;

            var valueCounts = new Dictionary<string, int>();

            foreach (var user in team.Users)
            {
                foreach (var userValue in user.UserValues)
                {
                    var valueName = userValue.Value.Name;

                    if (!valueCounts.ContainsKey(valueName))
                        valueCounts[valueName] = 0;

                    valueCounts[valueName]++;
                }
            }

            var dominantValues = valueCounts
                .OrderByDescending(v => v.Value)
                .Take(3)
                .Select(v => v.Key)
                .ToList();

            return new TeamCultureResponseDtos
            {
                TeamId = team.Id,
                TeamName = team.Name,
                DominantValues = dominantValues
            };
        }
    }
}