using API.G3Operaciones.Garcias.MX.Application.Inventory.DTOs;

namespace API.G3Operaciones.Garcias.MX.Application.Inventory.Interfaces
{
    public interface IWarehouseService
    {
        Task<WarehouseDto> CreateAsync(CreateWarehouseDto dto);
        Task<List<WarehouseDto>> GetAllAsync();
        Task<WarehouseDto?> GetByIdAsync(Guid id);
        Task DeactivateAsync(Guid id);
        Task ActivateAsync(Guid id);
    }
}
