using CodeFirstMicroservice.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace CodeFirstMicroservice.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class StatusesController : ControllerBase
    {
        private readonly TaskManagementContext _context;
        private readonly ILogger<StatusesController> _logger;

        public StatusesController(TaskManagementContext context, ILogger<StatusesController> logger)
        {
            _context = context;
            _logger = logger;
        }

        // GET: api/Statuses
        [HttpGet]
        public async Task<ActionResult<IEnumerable<Status>>> GetAllAsync()
        {
            var statuses = await _context.Statuses.ToListAsync();

            if (!statuses.Any())
            {
                _logger.LogInformation("No statuses found.");
                return NotFound("No statuses found.");
            }

            return Ok(statuses);
        }

        // GET: api/Statuses/5
        [HttpGet("{id}")]
        public async Task<ActionResult<Status>> GetByIdAsync(int id)
        {
            var status = await _context.Statuses.FindAsync(id);

            if (status == null)
            {
                _logger.LogWarning("Status with ID {Id} not found.", id);
                return NotFound(new { message = $"Status with ID {id} not found." });
            }

            return Ok(status);
        }

        // POST: api/Statuses
        [HttpPost]
        public async Task<ActionResult<Status>> Post([FromBody] Status status)
        {
            if (status == null || string.IsNullOrWhiteSpace(status.Name))
            {
                return BadRequest("Invalid status data.");
            }

            await _context.Statuses.AddAsync(status);
            await _context.SaveChangesAsync();

            return CreatedAtAction(nameof(GetByIdAsync), new { id = status.Id }, status);
        }

        // PUT: api/Statuses/5
        [HttpPut("{id:int}")]
        public async Task<IActionResult> UpdateAsync(int id, [FromBody] Status status)
        {
            if (id != status.Id)
            {
                return BadRequest(new { message = "Route ID does not match Status ID." });
            }

            var existing = await _context.Statuses.FindAsync(id);
            if (existing == null)
            {
                return NotFound(new { message = $"Status with ID {id} does not exist." });
            }

            existing.Name = status.Name;
            _context.Entry(existing).State = EntityState.Modified;

            try
            {
                await _context.SaveChangesAsync();
                return NoContent();
            }
            catch (DbUpdateException ex)
            {
                _logger.LogError(ex, "Error updating Status with ID {Id}", id);
                return StatusCode(500, new { message = "An error occurred while updating the status." });
            }
        }

        // DELETE: api/Statuses/5
        [HttpDelete("{id:int}")]
        public async Task<IActionResult> DeleteAsync(int id)
        {
            var status = await _context.Statuses.FindAsync(id);
            if (status == null)
            {
                return NotFound(new { message = $"Status with ID {id} not found." });
            }

            _context.Statuses.Remove(status);
            await _context.SaveChangesAsync();

            return NoContent();
        }
    }
}
