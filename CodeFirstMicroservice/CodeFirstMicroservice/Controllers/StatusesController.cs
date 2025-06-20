using CodeFirstMicroservice.Models;
using CodeFirstMicroservice.Models.Dtos;
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

        // Manual Mapping (DTO <-> Entity)
        private static StatusDto ToDto(Status status) => new()
        {
            Id = status.Id,
            Name = status.Name
        };

        private static Status ToEntity(StatusDto dto) => new()
        {
            Id = dto.Id,
            Name = dto.Name
        };

        // GET all
        [HttpGet]
        [ProducesResponseType(typeof(IEnumerable<StatusDto>), StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        public async Task<ActionResult<IEnumerable<StatusDto>>> GetAllAsync()
        {
            var statuses = await _context.Statuses.ToListAsync();

            if (!statuses.Any())
            {
                _logger.LogInformation("No statuses found.");
                return NotFound("No statuses found.");
            }

            return Ok(statuses.Select(ToDto));
        }

        // GET by ID
        [HttpGet("{id:int}")]
        public async Task<ActionResult<StatusDto>> GetByIdAsync(int id)
        {
            var status = await _context.Statuses.FindAsync(id);

            if (status == null)
            {
                _logger.LogWarning("Status with ID {Id} not found.", id);
                return NotFound(new { message = $"Status with ID {id} not found." });
            }

            return Ok(ToDto(status));
        }

        // POST
        [HttpPost]
        public async Task<ActionResult<StatusDto>> PostAsync([FromBody] StatusDto dto)
        {
            if (dto == null || string.IsNullOrWhiteSpace(dto.Name))
            {
                return BadRequest("Invalid status data.");
            }

            var entity = ToEntity(dto);
            await _context.Statuses.AddAsync(entity);
            await _context.SaveChangesAsync();

            return CreatedAtAction(nameof(GetByIdAsync), new { id = entity.Id }, ToDto(entity));
        }

        // PUT
        [HttpPut("{id:int}")]
        public async Task<IActionResult> UpdateAsync(int id, [FromBody] StatusDto dto)
        {
            if (id != dto.Id)
                return BadRequest(new { message = "Route ID does not match Status ID." });

            var entity = await _context.Statuses.FindAsync(id);
            if (entity == null)
                return NotFound(new { message = $"Status with ID {id} does not exist." });

            entity.Name = dto.Name;

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

        // DELETE
        [HttpDelete("{id:int}")]
        public async Task<IActionResult> DeleteAsync(int id)
        {
            var entity = await _context.Statuses.FindAsync(id);
            if (entity == null)
                return NotFound(new { message = $"Status with ID {id} not found." });

            _context.Statuses.Remove(entity);
            await _context.SaveChangesAsync();

            return NoContent();
        }
    }
}
