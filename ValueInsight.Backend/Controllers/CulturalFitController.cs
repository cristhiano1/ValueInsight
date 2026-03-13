using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Authorization;
using ValueInsight.Backend.Services;
using ValueInsight.Backend.Dtos;

namespace ValueInsight.Backend.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    [Authorize]
    public class CulturalFitController : ControllerBase
    {
        private readonly CulturalFitService _service;

        public CulturalFitController(CulturalFitService service)
        {
            _service = service;
        }

        // GET: api/CulturalFit/{teamId}
        [HttpGet("{teamId}")]
        public async Task<ActionResult<CulturalFitResponseDtos>> GetTeamAlignment(int teamId)
        {
            var result = await _service.CalculateTeamAlignment(teamId);

            if (result == null)
                return NotFound("Team not found");

            return Ok(result);
        }

        // POST: api/CulturalFit/calculate
        [HttpPost("calculate")]
        public async Task<ActionResult<CulturalFitResponseDtos>> CalculateAlignment([FromBody] TeamCultureResponseDtos request)
        {
            var result = await _service.CalculateTeamAlignment(request.TeamId);

            return Ok(result);
        }
    }
}