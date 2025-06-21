using CodeFirstMicroservice.Interfaces;
using CodeFirstMicroservice.Models;
using CodeFirstMicroservice.Models.Dtos;
using Microsoft.AspNetCore.Mvc;

namespace CodeFirstMicroservice.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class StatusesController : ControllerBase
    {
        private readonly IStatusService _statusService;
        private readonly ILogger<StatusesController> _logger;

        public StatusesController(ILogger<StatusesController> logger, IStatusService statusService)
        {
            _logger = logger;
            _statusService = statusService;
        }

        // GET ALL
        [HttpGet]
        [ProducesResponseType(typeof(IEnumerable<StatusDto>), StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        public async Task<ActionResult<IEnumerable<StatusDto>>> GetAllAsync()
        {
            var statuses = await _statusService.GetAllAsync();
            if (!statuses.Any())
            {
                _logger.LogInformation("GET /api/statuses - No statuses found.");
                return NotFound("No statuses found.");
            }

            _logger.LogInformation("GET /api/statuses - {Count} statuses retrieved", statuses.Count());

            return Ok(statuses);
        }

        // GET BY ID
        [HttpGet("{id:int}", Name = "GetByIdAsync")]
        [ProducesResponseType(typeof(StatusDto), StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        public async Task<ActionResult<StatusDto>> GetByIdAsync(int id)
        {
            var status = await _statusService.GetByIdAsync(id);

            if (status == null)
            {
                _logger.LogWarning("GET /api/statuses/{Id} - Status not found.", id);
                return NotFound(new { message = $"Status with ID {id} not found." });
            }

            _logger.LogInformation("GET /api/statuses/{Id} - Status retrieved successfully.", id);

            return Ok(status);
        }

        // POST
        [HttpPost]
        [ProducesResponseType(typeof(StatusDto), StatusCodes.Status201Created)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        public async Task<ActionResult<StatusDto>> PostAsync([FromBody] StatusDto dto) // noteb: dto alip dto donuyoruz, veritabi objesi donmuyoruz (Güvenlik, Ayrışma(decoupling), Geliştirilebilirlik, Versiyonlama kolaylığı)
        {
            if (!ModelState.IsValid)
            {
                _logger.LogWarning("POST /api/statuses - Invalid model state.");
                return BadRequest(ModelState);
            }

            var createdStatus = await _statusService.PostAsync(dto);
            
            _logger.LogInformation("POST /api/statuses - Status created with ID {Id}.", createdStatus.Id);

            return CreatedAtRoute(nameof(GetByIdAsync), new { id = createdStatus.Id }, createdStatus);
        }

        // PUT
        [HttpPut("{id:int}")]   // noteb: id from url - /api/statuses/5 
        [ProducesResponseType(StatusCodes.Status204NoContent)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        public async Task<IActionResult> UpdateAsync(int id, [FromBody] StatusDto dto)   // noteb: id'yi url'den aldim, StatusDto dto is body. Ayni id mi kiyasliyoruz
        {
            if (!ModelState.IsValid)
            {
                _logger.LogWarning("PUT api/status/{Id} - Attempted to update with invalid data.", id);
                return BadRequest(ModelState);
            }

            if (id != dto.Id)
            {
                _logger.LogWarning("PUT /api/statuses/{Id} - Route ID does not match Status ID.", id);
                return BadRequest(new { message = "Route ID does not match Status ID." });
            }
            
            var updatedStatus = await _statusService.UpdateAsync(id, dto);

            if (!updatedStatus)
            {
                _logger.LogWarning("PUT /api/statuses/{Id} - Status not found.", id);
                return NotFound(new { message = $"Status with ID {id} does not exist." });
            }
             
            _logger.LogInformation("PUT /api/statuses/{Id} - Status updated successfully.", id);
            return NoContent();
        }

        // DELETE
        [HttpDelete("{id:int}")]
        [ProducesResponseType(StatusCodes.Status204NoContent)]  // noteb: servisteki donus tiplerini yazmak; api doc'a bakip entegrasyon yapan icin kodunu ona gore donus tipi ayarlamasi yapmasi bakimindan iyi
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        public async Task<IActionResult> DeleteAsync(int id)
        {
            var deleted = await _statusService.DeleteAsync(id);
            
            if (!deleted)
            {
                _logger.LogWarning("DELETE /api/statuses/{Id} - Status not found.", id);
                return NotFound(new { message = $"Status with ID {id} not found." });
            }
            
            _logger.LogInformation("DELETE /api/statuses/{Id} - Status deleted successfully.", id);
            return NoContent();
        }
    }
}
