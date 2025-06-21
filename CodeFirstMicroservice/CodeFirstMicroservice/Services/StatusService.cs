using AutoMapper;
using CodeFirstMicroservice.Interfaces;
using CodeFirstMicroservice.Models;
using CodeFirstMicroservice.Models.Dtos;
using Microsoft.EntityFrameworkCore;

namespace CodeFirstMicroservice.Services
{
    public class StatusService : IStatusService
    {
        private readonly TaskManagementContext _context;
        private readonly ILogger<StatusService> _logger;
        private readonly IMapper _mapper;

        public StatusService(TaskManagementContext context, ILogger<StatusService> logger, IMapper mapper)
        {
            _context = context;
            _logger = logger;
            _mapper = mapper;
        }

        public async Task<bool> DeleteAsync(int id)
        {
            var entity = await _context.Statuses.FindAsync(id);
            if (entity == null) return false;

            _context.Statuses.Remove(entity);
            await _context.SaveChangesAsync();

            _logger.LogInformation("Status with ID {Id} deleted successfully.", id);
            return true;
        }

        public async Task<IEnumerable<StatusDto>> GetAllAsync()
        {
            var statusList = await _context.Statuses.ToListAsync();

            if (!statusList.Any()) return Enumerable.Empty<StatusDto>();

            _logger.LogInformation("{Count} statuses retrieved.", statusList.Count);

            return _mapper.Map<IEnumerable<StatusDto>>(statusList); // noteb: dto olarak donuyoruz objeleri, veritabani objelerini donmuyoruz
        }

        public async Task<StatusDto?> GetByIdAsync(int id)
        {
            var status = await _context.Statuses.FindAsync(id);
            return status == null ? null : _mapper.Map<StatusDto>(status);
        }

        public async Task<StatusDto> PostAsync(StatusDto dto)
        {
            var entity = _mapper.Map<Status>(dto);
            
            await _context.Statuses.AddAsync(entity);
            await _context.SaveChangesAsync();
            _logger.LogInformation("Status with ID {Id} created.", entity.Id);

            return _mapper.Map<StatusDto>(entity);
        }

        public async Task<bool> UpdateAsync(int id, StatusDto dto)
        {
            if (id != dto.Id) return false;

            var entity = await _context.Statuses.FindAsync(id);
            if (entity == null) return false;

            entity.Name = dto.Name;
            await _context.SaveChangesAsync();
            _logger.LogInformation("Status with ID {Id} updated.", id);

            return true;
        }
    }
}

// Noteb
// Service katmanı sadece veri döndürmeli, is katmanidir, HTTP ile ilgili kavramlar yok. Controller bu veriye göre HTTP cevabı üretmelidir (ActionResult, IActionResult, FromBody, NotFound, Ok, BadRequest).
// Controller sadece servisle iletişime geçiyor, veritabanına doğrudan erişmiyor. Bu, test edilebilirliği ve bağımlılıkların ayrışmasını artırır.
// Controller katmani, sadece servis katmanıyla konuşuyor.
// Service katmanı, veritabanı işlemlerini ve loglamayı yönetiyor.