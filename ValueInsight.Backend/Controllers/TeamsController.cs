using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Authorization;
using Microsoft.EntityFrameworkCore;
using System.Security.Claims;
using ValueInsight.Backend.Data;
using ValueInsight.Backend.Models;
using ValueInsight.Backend.Services;

namespace ValueInsight.Backend.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    [Authorize]
    public class TeamsController : ControllerBase
    {
        private readonly ValueInsightDbContext _context;
        private readonly TeamAccessService _teamAccessService;

        public TeamsController(ValueInsightDbContext context, TeamAccessService teamAccessService)
        {
            _context = context;
            _teamAccessService = teamAccessService;
        }

        private async Task<User?> GetCurrentUserAsync()
        {
            var userIdClaim = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
            return int.TryParse(userIdClaim, out var userId)
                ? await _context.Users.FirstOrDefaultAsync(x => x.Id == userId)
                : null;
        }

        [HttpGet]
        [AllowAnonymous]
        public async Task<IActionResult> GetTeams()
        {
            var teams = await _context.Teams
                .Include(t => t.Leader)
                .OrderBy(t => t.Name)
                .Select(t => new
                {
                    t.Id,
                    t.Name,
                    t.InviteCode,
                    LeaderName = t.Leader != null ? t.Leader.Name : null
                })
                .ToListAsync();

            return Ok(teams);
        }

        [HttpGet("workspace")]
        public async Task<IActionResult> GetWorkspace()
        {
            var currentUser = await GetCurrentUserAsync();
            if (currentUser == null)
                return Unauthorized();

            var currentTeamId = await _teamAccessService.GetCurrentTeamIdAsync(currentUser.Id);
            var pendingRequest = await _context.TeamJoinRequests
                .Include(x => x.Team)
                .Where(x => x.UserId == currentUser.Id && x.Status == "Pending")
                .OrderByDescending(x => x.RequestedAtUtc)
                .Select(x => new
                {
                    x.Id,
                    x.TeamId,
                    TeamName = x.Team.Name,
                    x.Status,
                    x.RequestedAtUtc
                })
                .FirstOrDefaultAsync();

            IQueryable<Team> manageableTeamsQuery = _context.Teams.Include(t => t.Leader);
            if (!currentUser.IsAdmin)
                manageableTeamsQuery = manageableTeamsQuery.Where(t => t.LeaderId == currentUser.Id);

            var manageableTeams = await manageableTeamsQuery
                .OrderBy(t => t.Name)
                .Select(t => new
                {
                    t.Id,
                    t.Name,
                    t.InviteCode,
                    LeaderName = t.Leader != null ? t.Leader.Name : null,
                    MembersCount = t.Members.Count(),
                    CompletedMembers = t.Members.Count(m => m.HasCompletedAssessment),
                    PendingJoinRequests = t.JoinRequests.Count(r => r.Status == "Pending"),
                    t.AllowPartialReport,
                    TeamReportReady = t.AllowPartialReport || (t.Members.Count() > 0 && t.Members.All(m => m.HasCompletedAssessment))
                })
                .ToListAsync();

            return Ok(new
            {
                currentUser.IsAdmin,
                CurrentTeamId = currentTeamId,
                JoinRequestStatus = pendingRequest?.Status,
                JoinRequestTeamName = pendingRequest?.TeamName,
                JoinRequestedAtUtc = pendingRequest?.RequestedAtUtc,
                ManageableTeams = manageableTeams
            });
        }

        [HttpGet("{id}")]
        public async Task<ActionResult<object>> GetTeam(int id)
        {
            var currentUser = await GetCurrentUserAsync();
            if (currentUser == null)
                return Unauthorized();

            var isMember = await _teamAccessService.IsMemberOfTeamAsync(currentUser.Id, id);
            if (!(currentUser.IsAdmin || isMember))
                return Forbid();

            var team = await _context.Teams
                .Include(t => t.Leader)
                .Include(t => t.Members)
                    .ThenInclude(m => m.User)
                        .ThenInclude(u => u.AssessmentRuns)
                .Include(t => t.JoinRequests.Where(x => x.Status == "Pending"))
                    .ThenInclude(x => x.User)
                .FirstOrDefaultAsync(t => t.Id == id);

            if (team == null)
                return NotFound();

            var completedMembers = team.Members.Count(m => m.HasCompletedAssessment || m.User.AssessmentRuns.Any(r => r.Status == "Completed"));
            var totalMembers = team.Members.Count;

            var result = new
            {
                team.Id,
                team.Name,
                team.InviteCode,
                team.AllowPartialReport,
                team.LeaderId,
                LeaderName = team.Leader?.Name,
                CanManage = _teamAccessService.CanManageTeam(currentUser, team),
                TeamReportReady = team.AllowPartialReport || (totalMembers > 0 && completedMembers == totalMembers),
                CompletedMembers = completedMembers,
                TotalMembers = totalMembers,
                Members = team.Members.OrderBy(x => x.User.Name).Select(m => new
                {
                    Id = m.UserId,
                    Name = m.User.Name,
                    Email = m.User.Email,
                    IsLeader = team.LeaderId == m.UserId,
                    HasCompletedAssessment = m.HasCompletedAssessment || m.User.AssessmentRuns.Any(r => r.Status == "Completed")
                }),
                PendingJoinRequests = team.JoinRequests.Select(r => new
                {
                    r.Id,
                    r.UserId,
                    UserName = r.User.Name,
                    UserEmail = r.User.Email,
                    r.Status,
                    r.RequestedAtUtc
                })
            };

            return Ok(result);
        }

        [HttpPost]
        public async Task<ActionResult<object>> CreateTeam([FromBody] Team team)
        {
            var currentUser = await GetCurrentUserAsync();
            if (currentUser?.IsAdmin != true)
                return Forbid();

            team.InviteCode = string.IsNullOrWhiteSpace(team.InviteCode)
                ? $"TEAM-{Guid.NewGuid().ToString("N")[..6].ToUpperInvariant()}"
                : team.InviteCode.Trim().ToUpperInvariant();

            _context.Teams.Add(team);
            await _context.SaveChangesAsync();

            return CreatedAtAction(nameof(GetTeam), new { id = team.Id }, new
            {
                team.Id,
                team.Name,
                team.InviteCode
            });
        }

        [HttpPost("{teamId:int}/assign-leader")]
        public async Task<IActionResult> AssignLeader(int teamId, [FromBody] AssignLeaderRequest request)
        {
            var currentUser = await GetCurrentUserAsync();
            if (currentUser?.IsAdmin != true)
                return Forbid();

            var team = await _context.Teams.FirstOrDefaultAsync(t => t.Id == teamId);
            if (team == null)
                return NotFound();

            var user = await _context.Users.FirstOrDefaultAsync(u => u.Id == request.UserId);
            if (user == null)
                return NotFound("Selected user was not found.");

            if (!await _teamAccessService.IsMemberOfTeamAsync(user.Id, teamId))
                return BadRequest("Only a member of the selected team can become the team leader.");

            team.LeaderId = user.Id;
            await _context.SaveChangesAsync();

            return Ok(new { message = "Team leader assigned successfully." });
        }

        [HttpPost("{teamId:int}/allow-partial-report")]
        public async Task<IActionResult> SetAllowPartialReport(int teamId, [FromBody] AllowPartialReportRequest request)
        {
            var currentUser = await GetCurrentUserAsync();
            var team = await _context.Teams.FirstOrDefaultAsync(t => t.Id == teamId);
            if (team == null)
                return NotFound();

            if (!_teamAccessService.CanManageTeam(currentUser, team))
                return Forbid();

            team.AllowPartialReport = request.AllowPartialReport;
            await _context.SaveChangesAsync();

            return Ok(new { message = "Report generation policy updated.", team.AllowPartialReport });
        }

        [HttpPost("join-request")]
        public async Task<IActionResult> CreateJoinRequest([FromBody] JoinTeamRequest request)
        {
            var currentUser = await GetCurrentUserAsync();
            if (currentUser == null)
                return Unauthorized();

            var currentTeamId = await _teamAccessService.GetCurrentTeamIdAsync(currentUser.Id);
            if (currentTeamId.HasValue)
                return BadRequest("You already belong to a team.");

            Team? team = null;
            if (request.TeamId.HasValue)
                team = await _context.Teams.FirstOrDefaultAsync(t => t.Id == request.TeamId.Value);

            if (team == null && !string.IsNullOrWhiteSpace(request.InviteCode))
                team = await _context.Teams.FirstOrDefaultAsync(t => t.InviteCode == request.InviteCode.Trim().ToUpperInvariant());

            if (team == null)
                return BadRequest("No matching team was found.");

            var existingPending = await _context.TeamJoinRequests.AnyAsync(x => x.UserId == currentUser.Id && x.Status == "Pending");
            if (existingPending)
                return BadRequest("You already have a pending join request.");

            _context.TeamJoinRequests.Add(new TeamJoinRequest
            {
                UserId = currentUser.Id,
                TeamId = team.Id,
                Status = "Pending",
                RequestedAtUtc = DateTime.UtcNow
            });

            await _context.SaveChangesAsync();
            return Ok(new { message = $"Join request sent to team {team.Name}." });
        }

        [HttpPost("join-request/{requestId:int}/review")]
        public async Task<IActionResult> ReviewJoinRequest(int requestId, [FromBody] ReviewJoinRequest request)
        {
            var currentUser = await GetCurrentUserAsync();
            if (currentUser == null)
                return Unauthorized();

            var joinRequest = await _context.TeamJoinRequests
                .Include(x => x.Team)
                .Include(x => x.User)
                .FirstOrDefaultAsync(x => x.Id == requestId);

            if (joinRequest == null)
                return NotFound();

            if (!_teamAccessService.CanManageTeam(currentUser, joinRequest.Team))
                return Forbid();

            if (joinRequest.Status != "Pending")
                return BadRequest("This request has already been reviewed.");

            joinRequest.Status = request.Approve ? "Approved" : "Rejected";
            joinRequest.ReviewedByUserId = currentUser.Id;
            joinRequest.ReviewedAtUtc = DateTime.UtcNow;

            if (request.Approve)
            {
                var existingTeamId = await _teamAccessService.GetCurrentTeamIdAsync(joinRequest.UserId);
                if (existingTeamId.HasValue)
                    return BadRequest("This user already belongs to another team.");

                _context.TeamMembers.Add(new TeamMember
                {
                    TeamId = joinRequest.TeamId,
                    UserId = joinRequest.UserId,
                    HasCompletedAssessment = await _teamAccessService.HasCompletedAssessmentAsync(joinRequest.UserId),
                    JoinedAtUtc = DateTime.UtcNow
                });
            }

            await _context.SaveChangesAsync();
            return Ok(new { message = request.Approve ? "Join request approved." : "Join request rejected." });
        }
    }

    public class AssignLeaderRequest
    {
        public int UserId { get; set; }
    }

    public class AllowPartialReportRequest
    {
        public bool AllowPartialReport { get; set; }
    }

    public class JoinTeamRequest
    {
        public int? TeamId { get; set; }
        public string? InviteCode { get; set; }
    }

    public class ReviewJoinRequest
    {
        public bool Approve { get; set; }
    }
}
