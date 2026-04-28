using System.Security.Claims;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using ValueInsight.Backend.Data;
using ValueInsight.Backend.Dtos;
using ValueInsight.Backend.Models;
using ValueInsight.Backend.Services;

namespace ValueInsight.Backend.Controllers
{
    [ApiController]
    [Route("api/user-values")]
    [Authorize]
    public class UserValuesController : ControllerBase
    {
        private readonly ValueInsightDbContext _context;
        private readonly AssessmentHistoryService _assessmentHistoryService;

        public UserValuesController(
            ValueInsightDbContext context,
            AssessmentHistoryService assessmentHistoryService)
        {
            _context = context;
            _assessmentHistoryService = assessmentHistoryService;
        }

        [HttpPost("save-top5")]
        public async Task<IActionResult> SaveTop5(SaveUserValuesRequest request)
        {
            if (request.OrderedValueIds == null || request.OrderedValueIds.Count != 5)
                return BadRequest("Exactly 5 ordered value ids are required.");

            if (request.OrderedValueIds.Distinct().Count() != 5)
                return BadRequest("Value ids must be unique.");

            var userIdClaim = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
            if (!int.TryParse(userIdClaim, out var userId))
                return Unauthorized();

            var validIds = await _context.Values
                .Where(v => request.OrderedValueIds.Contains(v.Id))
                .Select(v => v.Id)
                .ToListAsync();

            if (validIds.Count != 5)
                return BadRequest("One or more value ids do not exist.");

            var existing = await _context.UserValues
                .Where(uv => uv.UserId == userId)
                .ToListAsync();

            _context.UserValues.RemoveRange(existing);

            for (int i = 0; i < request.OrderedValueIds.Count; i++)
            {
                _context.UserValues.Add(new UserValue
                {
                    UserId = userId,
                    ValueId = request.OrderedValueIds[i],
                    Rank = i + 1
                });
            }

            await _context.SaveChangesAsync();

            var run = await _assessmentHistoryService.EnsureActiveRunAsync(userId);
            await _assessmentHistoryService.SyncActiveRunAsync(userId);

            return Ok(new
            {
                message = "Top 5 saved successfully.",
                assessmentRunId = run.Id
            });
        }

        [HttpGet("mine")]
        public async Task<IActionResult> GetMine()
        {
            var userIdClaim = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
            if (!int.TryParse(userIdClaim, out var userId))
                return Unauthorized();

            var items = await _context.UserValues
                .Where(uv => uv.UserId == userId)
                .Include(uv => uv.Value)
                .OrderBy(uv => uv.Rank)
                .Select(uv => new
                {
                    uv.ValueId,
                    uv.Rank,
                    ValueName = uv.Value.Name,
                    Category = uv.Value.Category.ToString()
                })
                .ToListAsync();

            return Ok(items);
        }
    }
}