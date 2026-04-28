using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Authorization;
using Microsoft.EntityFrameworkCore;
using ValueInsight.Backend.Data;
using ValueInsight.Backend.Models;

namespace ValueInsight.Backend.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    [Authorize]
    public class TeamsController : ControllerBase
    {
        private readonly ValueInsightDbContext _context;

        public TeamsController(ValueInsightDbContext context)
        {
            _context = context;
        }

        [HttpGet]
        [AllowAnonymous]
        public async Task<IActionResult> GetTeams()
        {
            var teams = await _context.Teams
                .OrderBy(t => t.Name)
                .Select(t => new
                {
                    t.Id,
                    t.Name
                })
                .ToListAsync();

            return Ok(teams);
        }

        [HttpGet("{id}")]
        [Authorize(Roles = "Coach")]
        public async Task<ActionResult<Team>> GetTeam(int id)
        {
            var team = await _context.Teams
                .Include(t => t.Users)
                .FirstOrDefaultAsync(t => t.Id == id);

            if (team == null)
                return NotFound();

            return team;
        }

        [HttpPost]
        [Authorize(Roles = "Coach")]
        public async Task<ActionResult<Team>> CreateTeam(Team team)
        {
            _context.Teams.Add(team);
            await _context.SaveChangesAsync();

            return CreatedAtAction(nameof(GetTeam), new { id = team.Id }, team);
        }

        [HttpPut("{id}")]
        [Authorize(Roles = "Coach")]
        public async Task<IActionResult> UpdateTeam(int id, Team team)
        {
            if (id != team.Id)
                return BadRequest();

            _context.Entry(team).State = EntityState.Modified;
            await _context.SaveChangesAsync();

            return NoContent();
        }

        [HttpDelete("{id}")]
        [Authorize(Roles = "Coach")]
        public async Task<IActionResult> DeleteTeam(int id)
        {
            var team = await _context.Teams.FindAsync(id);

            if (team == null)
                return NotFound();

            _context.Teams.Remove(team);
            await _context.SaveChangesAsync();

            return NoContent();
        }

        [HttpGet("{id}/overview")]
        [Authorize]
        public async Task<IActionResult> GetTeamOverview(int id)
        {
            var team = await _context.Teams
                .Include(t => t.Users)
                .FirstOrDefaultAsync(t => t.Id == id);

            if (team == null)
                return NotFound();

            var teamSize = team.Users?.Count ?? 0;

            return Ok(new
            {
                teamId = team.Id,
                teamName = team.Name,
                teamSize = teamSize,
                completedAssessments = 0,
                pendingAssessments = teamSize,
                teamCultureType = "No data",
                culturalFitScore = (double?)null,
                latestAssessmentCompletedAtUtc = (DateTime?)null
            });
        }
    }
}