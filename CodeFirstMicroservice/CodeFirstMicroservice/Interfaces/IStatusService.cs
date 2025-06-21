using CodeFirstMicroservice.Models.Dtos;
using Microsoft.AspNetCore.Mvc;

namespace CodeFirstMicroservice.Interfaces
{
    public interface IStatusService
    {
        Task<IEnumerable<StatusDto>> GetAllAsync();
        Task<StatusDto?> GetByIdAsync(int id);
        Task<StatusDto> PostAsync(StatusDto dto);
        Task<bool> UpdateAsync(int id, StatusDto dto);
        Task<bool> DeleteAsync(int id);
    }
}