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

        public AuthController(ValueInsightDbContext db, JwtService jwt)
        {
            _db = db;
            _jwt = jwt;
        }

        //[HttpPost("register")]
        //public async Task<IActionResult> Register(RegisterRequest request)
        //{
        //    var exists = await _db.Users
        //        .AnyAsync(u => u.Email == request.Email);

        //    if (exists)
        //        return BadRequest("User already exists");

        //    var user = new User
        //    {
        //        Name = request.Name,
        //        Email = request.Email,
        //        Password = request.Password
        //    };

        //    _db.Users.Add(user);
        //    await _db.SaveChangesAsync();

        //    var token = _jwt.GenerateToken(user.Id, user.Email);

        //    return Ok(new { token });
        //}

        [HttpPost("register")]
        public async Task<IActionResult> Register(RegisterRequest request)
        {
            var exists = await _db.Users.AnyAsync(u => u.Email == request.Email);
            if (exists)
                return BadRequest("User already exists");

            var teamExists = await _db.Teams.AnyAsync(t => t.Id == request.TeamId);
            if (!teamExists)
                return BadRequest($"Team with id {request.TeamId} does not exist");

            var user = new User
            {
                Name = request.Name,
                Email = request.Email,
                Password = request.Password,
                TeamId = request.TeamId
            };

            _db.Users.Add(user);
            await _db.SaveChangesAsync();

            var token = _jwt.GenerateToken(user.Id, user.Email);

            return Ok(new { token });
        }

        [HttpPost("login")]
        public async Task<IActionResult> Login(LoginRequest request)
        {
            var user = await _db.Users
                .FirstOrDefaultAsync(u => u.Email == request.Email);

            if (user == null || user.Password != request.Password)
                return Unauthorized("Invalid credentials");

            var token = _jwt.GenerateToken(user.Id, user.Email);

            return Ok(new { token });
        }
    }
}