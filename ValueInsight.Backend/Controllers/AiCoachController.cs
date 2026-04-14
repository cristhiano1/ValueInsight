using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Authorization;
using ValueInsight.Backend.Services;
using ValueInsight.Backend.Dtos;

namespace ValueInsight.Backend.Controllers;

[ApiController]
[Route("api/[controller]")]
[Authorize]
public class AiCoachController : ControllerBase
{
    private readonly AiCoachService _aiCoachService;
    private readonly ILogger<AiCoachController> _logger;

    public AiCoachController(
        AiCoachService aiCoachService,
        ILogger<AiCoachController> logger)
    {
        _aiCoachService = aiCoachService;
        _logger = logger;
    }

    // =========================
    // EXISTENTE (NO TOCAR)
    // =========================
    [HttpPost("generate")]
    public async Task<ActionResult<CoachingResponseDtos>> GenerateCoaching([FromBody] CoachingRequestDtos request)
    {
        if (!ModelState.IsValid)
        {
            _logger.LogWarning("Invalid coaching request received.");
            return BadRequest(ModelState);
        }

        try
        {
            _logger.LogInformation(
                "Generating coaching plan for user {UserId} in team {TeamId}",
                request.UserId,
                request.TeamId);

            var result = await _aiCoachService.GenerateCoachingAsync(request);

            return Ok(result);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error generating coaching plan.");
            return StatusCode(500, "Internal server error");
        }
    }

    // =========================
    // 🔥 NUEVO: TEAM INSIGHT
    // =========================
    [HttpPost("team-insight")]
    public async Task<IActionResult> GenerateTeamInsight([FromBody] TeamInsightDtos request)
    {
        try
        {
            _logger.LogInformation("Generating team insight");

            var prompt = $@"
You are an organizational psychologist.

Analyze this team:

- Culture type: {request.CultureType}
- Alignment: {request.AlignmentScore}
- Polarization: {request.PolarizationScore}
- Maturity: {request.MaturityIndex}
- Top categories: {string.Join(", ", request.TopCategories)}
- Weak categories: {string.Join(", ", request.LowCategories)}

Give:
1. Short interpretation (2–3 sentences)
2. One risk
3. One recommendation

Be concise.
";

            // reutilizamos tu servicio existente
            var result = await _aiCoachService.GenerateRawAsync(prompt);

            return Ok(new { insight = result });
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error generating team insight");
            return StatusCode(500, "Error generating team insight");
        }
    }
}