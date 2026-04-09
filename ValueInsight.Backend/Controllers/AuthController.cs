using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Authorization;
using Microsoft.EntityFrameworkCore;
using ValueInsight.Backend.Data;
using ValueInsight.Backend.Dtos;
using ValueInsight.Backend.Models;
using ValueInsight.Backend.Services;

namespace ValueInsight.Backend.Controllers
{
    [ApiController]
    [Route("api/auth")]
    [AllowAnonymous]
    public class AuthController : ControllerBase
    {
        private readonly ValueInsightDbContext _db;
        private readonly JwtService _jwt;
        private readonly PasswordService _passwordService;

        public AuthController(ValueInsightDbContext db, JwtService jwt, PasswordService passwordService)
        {
            _db = db;
            _jwt = jwt;
            _passwordService = passwordService;
        }

        // ✅ REGISTER
        [HttpPost("register")]
        public async Task<IActionResult> Register(RegisterRequest request)
        {
            var exists = await _db.Users.AnyAsync(u => u.Email == request.Email);

            if (exists)
                return BadRequest("User already exists");

            if (request.TeamId.HasValue)
            {
                var teamExists = await _db.Teams.AnyAsync(t => t.Id == request.TeamId.Value);
                if (!teamExists)
                    return BadRequest($"Team with id {request.TeamId.Value} does not exist");
            }

            var user = new User
            {
                Name = request.Name,
                Email = request.Email,
                TeamId = request.TeamId,
                Role = request.Role ?? "User"
            };

            // 🔥 HASH CORRECTO
            user.PasswordHash = _passwordService.HashPassword(user, request.Password);

            _db.Users.Add(user);
            await _db.SaveChangesAsync();

            var token = _jwt.GenerateToken(user.Id, user.Email, user.Role);

            return Ok(new { token });
        }

        // ✅ LOGIN
        [HttpPost("login")]
        public async Task<IActionResult> Login(LoginRequest request)
        {
            var user = await _db.Users
                .FirstOrDefaultAsync(u => u.Email == request.Email);

            if (user == null)
                return Unauthorized("Invalid credentials");

            // 🔥 VERIFY CORRECTO
            var isValid = _passwordService.VerifyPassword(user, user.PasswordHash, request.Password);

            if (!isValid)
                return Unauthorized("Invalid credentials");

            var token = _jwt.GenerateToken(user.Id, user.Email, user.Role);

            return Ok(new { token });
        }
    }
}