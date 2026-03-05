using Microsoft.AspNetCore.Mvc;
using ValueInsight.Backend.Services;
using ValueInsight.Backend.Dtos;

namespace ValueInsight.Backend.Controllers;

[ApiController]
[Route("api/[controller]")]
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

    // POST: api/AiCoach/generate
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
}