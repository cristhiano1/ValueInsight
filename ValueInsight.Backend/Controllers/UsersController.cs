using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Authorization;
using Microsoft.EntityFrameworkCore;
using System.Security.Claims;
using ValueInsight.Backend.Data;
using ValueInsight.Backend.Models;

namespace ValueInsight.Backend.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    [Authorize]
    public class UsersController : ControllerBase
    {
        private readonly ValueInsightDbContext _context;

        public UsersController(ValueInsightDbContext context)
        {
            _context = context;
        }

        // 🔥 SOLO COACH
        [Authorize(Roles = "Coach")]
        [HttpGet]
        public async Task<ActionResult<IEnumerable<User>>> GetUsers()
        {
            return await _context.Users
                .Include(u => u.UserValues)
                .ToListAsync();
        }

        // 👤 USER (su propio perfil) + COACH
        [HttpGet("{id}")]
        public async Task<ActionResult<User>> GetUser(int id)
        {
            var currentUserId = int.Parse(User.FindFirst(ClaimTypes.NameIdentifier)!.Value);

            // ❌ si no es coach y no es su propio id → bloqueado
            if (currentUserId != id && !User.IsInRole("Coach"))
                return Forbid();

            var user = await _context.Users
                .Include(u => u.UserValues)
                .FirstOrDefaultAsync(x => x.Id == id);

            if (user == null)
                return NotFound();

            return user;
        }

        // 🔥 SOLO COACH
        [Authorize(Roles = "Coach")]
        [HttpPost]
        public async Task<ActionResult<User>> CreateUser(User user)
        {
            _context.Users.Add(user);
            await _context.SaveChangesAsync();

            return CreatedAtAction(nameof(GetUser), new { id = user.Id }, user);
        }

        // 🔥 SOLO COACH
        [Authorize(Roles = "Coach")]
        [HttpPut("{id}")]
        public async Task<IActionResult> UpdateUser(int id, User user)
        {
            if (id != user.Id)
                return BadRequest();

            _context.Entry(user).State = EntityState.Modified;
            await _context.SaveChangesAsync();

            return NoContent();
        }

        // 🔥 SOLO COACH
        [Authorize(Roles = "Coach")]
        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteUser(int id)
        {
            var user = await _context.Users.FindAsync(id);

            if (user == null)
                return NotFound();

            _context.Users.Remove(user);
            await _context.SaveChangesAsync();

            return NoContent();
        }
    }
}