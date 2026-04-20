using System.Security.Claims;
using Microsoft.EntityFrameworkCore;
using ValueInsight.Backend.Data;
using ValueInsight.Backend.Models;

namespace ValueInsight.Backend.Services;

public class TeamAccessService
{
    private readonly ValueInsightDbContext _context;

    public TeamAccessService(ValueInsightDbContext context)
    {
        _context = context;
    }

    public int? GetCurrentUserId(ClaimsPrincipal principal)
    {
        var claim = principal.FindFirst(ClaimTypes.NameIdentifier)?.Value;
        return int.TryParse(claim, out var userId) ? userId : null;
    }

    public async Task<User?> GetCurrentUserAsync(ClaimsPrincipal principal)
    {
        var userId = GetCurrentUserId(principal);
        if (!userId.HasValue)
            return null;

        return await _context.Users.FirstOrDefaultAsync(x => x.Id == userId.Value);
    }

    public bool IsAdmin(User? user) => user?.IsAdmin == true;

    public bool IsTeamLeader(User? user, Team? team) => user != null && team != null && team.LeaderId == user.Id;

    public bool CanManageTeam(User? user, Team? team) => IsAdmin(user) || IsTeamLeader(user, team);

    public async Task<bool> HasCompletedAssessmentAsync(int userId)
    {
        return await _context.AssessmentRuns.AnyAsync(x => x.UserId == userId && x.Status == "Completed");
    }

    public async Task<int?> GetCurrentTeamIdAsync(int userId)
    {
        return await _context.TeamMembers
            .Where(x => x.UserId == userId)
            .Select(x => (int?)x.TeamId)
            .FirstOrDefaultAsync();
    }

    public async Task<Team?> GetCurrentTeamAsync(int userId)
    {
        var teamId = await GetCurrentTeamIdAsync(userId);
        if (!teamId.HasValue)
            return null;

        return await _context.Teams
            .Include(x => x.Leader)
            .FirstOrDefaultAsync(x => x.Id == teamId.Value);
    }

    public async Task<bool> IsMemberOfTeamAsync(int userId, int teamId)
    {
        return await _context.TeamMembers.AnyAsync(x => x.UserId == userId && x.TeamId == teamId);
    }

    public async Task<int> CompletedMembersCountAsync(int teamId)
    {
        return await _context.TeamMembers
            .Where(x => x.TeamId == teamId && x.HasCompletedAssessment)
            .CountAsync();
    }
}
