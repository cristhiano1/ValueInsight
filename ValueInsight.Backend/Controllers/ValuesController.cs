using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Authorization;
using Microsoft.EntityFrameworkCore;
using ValueInsight.Backend.Data;
using ValueInsight.Backend.Models;

namespace ValueInsight.Backend.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    [Authorize]
    public class ValuesController : ControllerBase
    {
        private readonly ValueInsightDbContext _context;

        public ValuesController(ValueInsightDbContext context)
        {
            _context = context;
        }

        [HttpGet]
        public async Task<ActionResult<IEnumerable<Value>>> GetValues()
        {
            return await _context.Values.ToListAsync();
        }

        [HttpGet("{id}")]
        public async Task<ActionResult<Value>> GetValue(int id)
        {
            var value = await _context.Values.FindAsync(id);

            if (value == null)
                return NotFound();

            return value;
        }

        [HttpPost]
        public async Task<ActionResult<Value>> CreateValue(Value value)
        {
            _context.Values.Add(value);
            await _context.SaveChangesAsync();

            return CreatedAtAction(nameof(GetValue), new { id = value.Id }, value);
        }

        [HttpPut("{id}")]
        public async Task<IActionResult> UpdateValue(int id, Value value)
        {
            if (id != value.Id)
                return BadRequest();

            _context.Entry(value).State = EntityState.Modified;
            await _context.SaveChangesAsync();

            return NoContent();
        }

        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteValue(int id)
        {
            var value = await _context.Values.FindAsync(id);

            if (value == null)
                return NotFound();

            _context.Values.Remove(value);
            await _context.SaveChangesAsync();

            return NoContent();
        }
    }
}