using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Authorization;
using Microsoft.EntityFrameworkCore;
using System.Security.Claims;
using ValueInsight.Backend.Data;
using ValueInsight.Backend.Dtos;
using ValueInsight.Backend.Models;
using ValueInsight.Backend.Services;

namespace ValueInsight.Backend.Controllers
{
    [ApiController]
    [Route("api/auth")]
    public class AuthController : ControllerBase
    {
        private readonly ValueInsightDbContext _db;
        private readonly JwtService _jwt;
        private readonly PasswordService _passwordService;
        private readonly TeamAccessService _teamAccessService;

        public AuthController(ValueInsightDbContext db, JwtService jwt, PasswordService passwordService, TeamAccessService teamAccessService)
        {
            _db = db;
            _jwt = jwt;
            _passwordService = passwordService;
            _teamAccessService = teamAccessService;
        }

        [HttpPost("register")]
        [AllowAnonymous]
        public async Task<IActionResult> Register(RegisterRequest request)
        {
            var exists = await _db.Users.AnyAsync(u => u.Email == request.Email);
            if (exists)
                return BadRequest("User already exists");

            var user = new User
            {
                Name = request.Name,
                Email = request.Email,
                IsAdmin = false
            };

            user.Password = _passwordService.HashPassword(user, request.Password);

            _db.Users.Add(user);
            await _db.SaveChangesAsync();

            var token = _jwt.GenerateToken(user.Id, user.Email, user.IsAdmin);
            var teamId = await _teamAccessService.GetCurrentTeamIdAsync(user.Id);

            return Ok(new
            {
                token,
                user = new
                {
                    user.Id,
                    user.Name,
                    user.Email,
                    user.IsAdmin,
                    TeamId = teamId
                }
            });
        }

        [HttpPost("login")]
        [AllowAnonymous]
        public async Task<IActionResult> Login(LoginRequest request)
        {
            var user = await _db.Users.FirstOrDefaultAsync(u => u.Email == request.Email);

            if (user == null)
                return Unauthorized("Invalid credentials");

            var isValid = _passwordService.VerifyPassword(user, user.Password, request.Password);
            if (!isValid)
                return Unauthorized("Invalid credentials");

            var token = _jwt.GenerateToken(user.Id, user.Email, user.IsAdmin);
            var teamId = await _teamAccessService.GetCurrentTeamIdAsync(user.Id);

            return Ok(new
            {
                token,
                user = new
                {
                    user.Id,
                    user.Name,
                    user.Email,
                    user.IsAdmin,
                    TeamId = teamId
                }
            });
        }

        [HttpGet("me")]
        [Authorize]
        public async Task<IActionResult> Me()
        {
            var userIdClaim = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
            if (!int.TryParse(userIdClaim, out var userId))
                return Unauthorized();

            var user = await _db.Users.FirstOrDefaultAsync(x => x.Id == userId);
            if (user == null)
                return NotFound();

            var teamId = await _teamAccessService.GetCurrentTeamIdAsync(user.Id);
            var teamName = teamId.HasValue ? await _db.Teams.Where(t => t.Id == teamId.Value).Select(t => t.Name).FirstOrDefaultAsync() : null;

            return Ok(new
            {
                user.Id,
                user.Name,
                user.Email,
                user.IsAdmin,
                TeamId = teamId,
                TeamName = teamName
            });
        }
    }
}
