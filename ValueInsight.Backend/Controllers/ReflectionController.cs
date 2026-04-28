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
    [Route("api/reflection")]
    [Authorize]
    public class ReflectionController : ControllerBase
    {
        private readonly ValueInsightDbContext _context;
        private readonly AssessmentHistoryService _assessmentHistoryService;

        public ReflectionController(
            ValueInsightDbContext context,
            AssessmentHistoryService assessmentHistoryService)
        {
            _context = context;
            _assessmentHistoryService = assessmentHistoryService;
        }

        [HttpGet("mine")]
        public async Task<IActionResult> GetMine()
        {
            var userIdClaim = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
            if (!int.TryParse(userIdClaim, out var userId))
                return Unauthorized();

            var answers = await _context.ReflectionAnswers
                .Where(x => x.UserId == userId)
                .OrderBy(x => x.QuestionId)
                .Select(x => new
                {
                    x.QuestionId,
                    x.QuestionText,
                    x.ResponseText
                })
                .ToListAsync();

            return Ok(answers);
        }

        [HttpPost("save")]
        public async Task<IActionResult> Save(SaveReflectionRequest request)
        {
            var userIdClaim = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
            if (!int.TryParse(userIdClaim, out var userId))
                return Unauthorized();

            if (request.Answers == null || request.Answers.Count == 0)
                return BadRequest("No answers provided.");

            foreach (var item in request.Answers)
            {
                var existing = await _context.ReflectionAnswers
                    .FirstOrDefaultAsync(x =>
                        x.UserId == userId &&
                        x.QuestionId == item.QuestionId);

                if (existing == null)
                {
                    _context.ReflectionAnswers.Add(new ReflectionAnswer
                    {
                        UserId = userId,
                        QuestionId = item.QuestionId,
                        QuestionText = item.QuestionText,
                        ResponseText = item.ResponseText
                    });
                }
                else
                {
                    existing.QuestionText = item.QuestionText;
                    existing.ResponseText = item.ResponseText;
                }
            }

            await _context.SaveChangesAsync();

            var containsTopValueAnswers = request.Answers.Any(x =>
                !string.IsNullOrWhiteSpace(x.QuestionId) &&
                x.QuestionId.StartsWith("topvalue-"));

            if (containsTopValueAnswers)
            {
                await _assessmentHistoryService.CompleteActiveRunAsync(userId);
            }
            else
            {
                await _assessmentHistoryService.EnsureActiveRunAsync(userId);
                await _assessmentHistoryService.SyncActiveRunAsync(userId);
            }

            return Ok(new
            {
                message = "Reflection answers saved.",
                completedAssessment = containsTopValueAnswers
            });
        }
    }
}