using System.Security.Claims;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using ValueInsight.Backend.Data;
using ValueInsight.Backend.Models;

namespace ValueInsight.Backend.Controllers;

[ApiController]
[Route("api/assessment")]
[Authorize]
public class AssessmentController : ControllerBase
{
    private readonly ValueInsightDbContext _context;

    public AssessmentController(ValueInsightDbContext context)
    {
        _context = context;
    }

    // 🧑‍💼 COACH inicia evaluación
    [Authorize(Roles = "Coach")]
    [HttpPost("start/{teamId}")]
    public async Task<IActionResult> Start(int teamId)
    {
        var coachId = int.Parse(User.FindFirst(ClaimTypes.NameIdentifier)!.Value);

        var usersInTeam = await _context.Users
            .Where(u => u.TeamId == teamId)
            .Select(u => u.Id)
            .ToListAsync();

        if (!usersInTeam.Any())
            return BadRequest("Team has no users");

        var run = new AssessmentRun
        {
            UserId = coachId,     // 🔥 quién inicia
            TeamId = teamId,      // 🔥 CLAVE
            CreatedAtUtc = DateTime.UtcNow,
            UpdatedAtUtc = DateTime.UtcNow,
            Status = "Active"
        };

        _context.AssessmentRuns.Add(run);
        await _context.SaveChangesAsync();

        return Ok(run);
    }

    // 👤 USER responde
    [HttpPost("submit/{runId}")]
    public async Task<IActionResult> Submit(int runId)
    {
        var userId = int.Parse(User.FindFirst(ClaimTypes.NameIdentifier)!.Value);

        var run = await _context.AssessmentRuns
            .Include(r => r.ValueSelections)
            .FirstOrDefaultAsync(r => r.Id == runId);

        if (run == null) return NotFound();

        var already = run.ValueSelections.Any(x => x.UserId == userId);
        if (!already)
            return BadRequest("User has not submitted values yet");

        run.UpdatedAtUtc = DateTime.UtcNow;

        await _context.SaveChangesAsync();

        return Ok();
    }

    // 🧑‍💼 COACH finaliza
    [Authorize(Roles = "Coach")]
    [HttpPost("finish/{runId}")]
    public async Task<IActionResult> Finish(int runId)
    {
        var run = await _context.AssessmentRuns
            .Include(r => r.ValueSelections)
            .FirstOrDefaultAsync(r => r.Id == runId);

        if (run == null) return NotFound();

        run.Status = "Completed";
        run.UpdatedAtUtc = DateTime.UtcNow;

        await _context.SaveChangesAsync();

        return Ok("Assessment completed");
    }

    // 🧑‍💼 COACH cancela
    [Authorize(Roles = "Coach")]
    [HttpPost("cancel/{runId}")]
    public async Task<IActionResult> Cancel(int runId)
    {
        var run = await _context.AssessmentRuns.FindAsync(runId);
        if (run == null) return NotFound();

        run.Status = "Cancelled";
        run.UpdatedAtUtc = DateTime.UtcNow;

        await _context.SaveChangesAsync();

        return Ok("Assessment cancelled");
    }

    // 📊 estado actual
    [HttpGet("status/{runId}")]
    public async Task<IActionResult> Status(int runId)
    {
        var run = await _context.AssessmentRuns
            .Include(r => r.ValueSelections)
            .FirstOrDefaultAsync(r => r.Id == runId);

        if (run == null) return NotFound();

        var respondedUsers = run.ValueSelections
            .Select(x => x.UserId)
            .Distinct()
            .Count();

        var totalUsers = await _context.Users
            .Where(u => u.TeamId == run.TeamId)
            .CountAsync();

        return Ok(new
        {
            run.Id,
            run.Status,
            RespondedUsers = respondedUsers,
            TotalUsers = totalUsers
        });
    }
}