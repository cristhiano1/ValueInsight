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

    [HttpPost("generate-team")]
    public async Task<ActionResult<TeamCoachingResponseDtos>> GenerateTeamCoaching([FromBody] TeamCoachingRequestDtos request)
    {
        if (!ModelState.IsValid)
        {
            _logger.LogWarning("Invalid team coaching request received.");
            return BadRequest(ModelState);
        }

        try
        {
            _logger.LogInformation("Generating team coaching plan for team {TeamId}", request.TeamId);
            var result = await _aiCoachService.GenerateTeamCoachingAsync(request);
            return Ok(result);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error generating team coaching plan.");
            return StatusCode(500, "Internal server error");
        }
    }

    // New endpoint to explicitly request deterministic fallback coaching (no AI)
    [HttpPost("fallback")]
    public async Task<ActionResult<CoachingResponseDtos>> GenerateFallbackCoaching([FromBody] CoachingRequestDtos request)
    {
        if (!ModelState.IsValid)
        {
            _logger.LogWarning("Invalid fallback coaching request received.");
            return BadRequest(ModelState);
        }

        try
        {
            _logger.LogInformation("Generating fallback coaching for user {UserId} in team {TeamId}", request.UserId, request.TeamId);
            var result = await _aiCoachService.GenerateFallbackCoachingAsync(request);
            return Ok(result);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error generating fallback coaching plan.");
            return StatusCode(500, "Internal server error");
        }
    }
}
