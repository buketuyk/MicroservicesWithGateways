using AutoMapper;
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
        private readonly IMapper _mapper;

        public StatusesController(TaskManagementContext context, ILogger<StatusesController> logger, IMapper mapper)
        {
            _context = context;
            _logger = logger;
            _mapper = mapper;
        }

        // noteb: Manual Mapping (DTO <-> Entity) AutoMapper kullanildiginda kaldirildi.
        //private static StatusDto ToDto(Status status) => new()
        //{
        //    Id = status.Id,
        //    Name = status.Name
        //};

        //private static Status ToEntity(StatusDto dto) => new()
        //{
        //    Id = dto.Id,
        //    Name = dto.Name
        //};

        // GET ALL
        [HttpGet]
        [ProducesResponseType(typeof(IEnumerable<StatusDto>), StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        public async Task<ActionResult<IEnumerable<StatusDto>>> GetAllAsync()
        {
            var statuses = await _context.Statuses.ToListAsync();

            if (!statuses.Any())
            {
                _logger.LogInformation("GET /api/statuses - No statuses found.");
                return NotFound("No statuses found.");
            }

            _logger.LogInformation("GET /api/statuses - {Count} statuses retrieved", statuses.Count);

            var dtoList = _mapper.Map<IEnumerable<StatusDto>>(statuses);
            return Ok(dtoList); // noteb: dto olarak donuyoruz objeleri, veritabani objelerini donmuyoruz
        }

        // GET BY ID
        [HttpGet("{id:int}", Name = "GetByIdAsync")]
        [ProducesResponseType(typeof(StatusDto), StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        public async Task<ActionResult<StatusDto>> GetByIdAsync(int id)
        {
            var status = await _context.Statuses.FindAsync(id);

            if (status == null)
            {
                _logger.LogWarning("GET /api/statuses/{Id} - Status not found.", id);
                return NotFound(new { message = $"Status with ID {id} not found." });
            }

            _logger.LogInformation("GET /api/statuses/{Id} - Status retrieved successfully.", id);

            var statusDto = _mapper.Map<StatusDto>(status);

            return Ok(statusDto);
        }

        // POST
        [HttpPost]
        [ProducesResponseType(typeof(StatusDto), StatusCodes.Status201Created)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        public async Task<ActionResult<StatusDto>> PostAsync([FromBody] StatusDto dto) // noteb: dto alip dto donuyoruz, veritabi objesi donmuyoruz (Güvenlik, Ayrışma(decoupling), Geliştirilebilirlik, Versiyonlama kolaylığı)
        {
            if (!ModelState.IsValid)
            {
                _logger.LogWarning("POST /api/statuses - Attempted to create status with invalid data.");

                var errors = ModelState
                    .Where(x => x.Value?.Errors.Count > 0)
                    .ToDictionary(
                        kvp => kvp.Key,
                        kvp => kvp.Value?.Errors.Select(e => e.ErrorMessage).ToList());
                
                return BadRequest( new { message = "Validation failed", errors = errors } );
            }

            var entity = _mapper.Map<Status>(dto);
            await _context.Statuses.AddAsync(entity);
            await _context.SaveChangesAsync();

            var resultDto = _mapper.Map<StatusDto>(entity);
            _logger.LogInformation("POST /api/statuses - Status with ID {Id} created successfully.", entity.Id);

            return CreatedAtRoute(nameof(GetByIdAsync), new { id = entity.Id }, resultDto);
        }

        // PUT
        [HttpPut("{id:int}")]
        [ProducesResponseType(StatusCodes.Status204NoContent)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        public async Task<IActionResult> UpdateAsync(int id, [FromBody] StatusDto dto)
        {
            if (!ModelState.IsValid)
            {
                _logger.LogWarning("PUT api/status/{Id} - Attempted to update with invalid data.", id);

                var errors = ModelState
                    .Where(x => x.Value?.Errors.Count > 0)
                    .ToDictionary(
                        kvp => kvp.Key,
                        kvp => kvp.Value?.Errors.Select(e => e.ErrorMessage).ToList());

                return BadRequest(new { message = "Validation failed", errors = errors });
            }

            if (id != dto.Id)
            {
                _logger.LogWarning("PUT /api/statuses/{Id} - Route ID does not match Status ID.", id);
                return BadRequest(new { message = "Route ID does not match Status ID." });
            }

            var entity = await _context.Statuses.FindAsync(id);
            if (entity == null)
            {
                _logger.LogWarning("PUT /api/statuses/{Id} - Status not found.", id);
                return NotFound(new { message = $"Status with ID {id} does not exist." });
            }

            entity.Name = dto.Name;
            
            await _context.SaveChangesAsync();
            _logger.LogInformation("PUT /api/statuses/{Id} - Status updated successfully.", id);
            return NoContent();
        }

        // DELETE
        [HttpDelete("{id:int}")]
        [ProducesResponseType(StatusCodes.Status204NoContent)]  // noteb: servisteki donus tiplerini yazmak; api doc'a bakip entegrasyon yapan icin kodunu ona gore donus tipi ayarlamasi yapmasi bakimindan iyi
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        public async Task<IActionResult> DeleteAsync(int id)
        {
            var entity = await _context.Statuses.FindAsync(id);
            if (entity == null)
            {
                _logger.LogWarning("DELETE /api/statuses/{Id} - Status not found.", id);
                return NotFound(new { message = $"Status with ID {id} not found." });
            }

            _context.Statuses.Remove(entity);
            await _context.SaveChangesAsync();
            
            _logger.LogInformation("DELETE /api/statuses/{Id} - Status deleted successfully.", id);
            return NoContent();
        }
    }
}
