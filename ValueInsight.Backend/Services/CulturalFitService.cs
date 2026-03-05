using Microsoft.EntityFrameworkCore;
using ValueInsight.Backend.Data;
using ValueInsight.Backend.Dtos;

namespace ValueInsight.Backend.Services
{
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

            var alignment = new CulturalFitResponseDtos
            {
                TeamId = teamId,
                AlignmentScore = 75
            };

            return alignment;
        }
    }
}