using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Authorization;
using Microsoft.EntityFrameworkCore;
using ValueInsight.Backend.Data;
using ValueInsight.Backend.Models;

namespace ValueInsight.Backend.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    [Authorize] // ✔ requiere login por defecto
    public class TeamsController : ControllerBase
    {
        private readonly ValueInsightDbContext _context;

        public TeamsController(ValueInsightDbContext context)
        {
            _context = context;
        }

        // 👤 Público (solo lectura básica)
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

        // 🔥 SOLO COACH
        [Authorize(Roles = "Coach")]
        [HttpGet("{id}")]
        public async Task<ActionResult<Team>> GetTeam(int id)
        {
            var team = await _context.Teams
                .Include(t => t.Users)
                .FirstOrDefaultAsync(t => t.Id == id);

            if (team == null)
                return NotFound();

            return team;
        }

        // 🔥 SOLO COACH
        [Authorize(Roles = "Coach")]
        [HttpPost]
        public async Task<ActionResult<Team>> CreateTeam(Team team)
        {
            _context.Teams.Add(team);
            await _context.SaveChangesAsync();

            return CreatedAtAction(nameof(GetTeam), new { id = team.Id }, team);
        }

        // 🔥 SOLO COACH
        [Authorize(Roles = "Coach")]
        [HttpPut("{id}")]
        public async Task<IActionResult> UpdateTeam(int id, Team team)
        {
            if (id != team.Id)
                return BadRequest();

            _context.Entry(team).State = EntityState.Modified;
            await _context.SaveChangesAsync();

            return NoContent();
        }

        // 🔥 SOLO COACH
        [Authorize(Roles = "Coach")]
        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteTeam(int id)
        {
            var team = await _context.Teams.FindAsync(id);

            if (team == null)
                return NotFound();

            _context.Teams.Remove(team);
            await _context.SaveChangesAsync();

            return NoContent();
        }
    }
}