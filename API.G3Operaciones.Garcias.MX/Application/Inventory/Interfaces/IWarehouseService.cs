using API.G3Operaciones.Garcias.MX.Application.Inventory.DTOs;
using API.G3Operaciones.Garcias.MX.Application.Products.DTOs;
using API.G3Operaciones.Garcias.MX.Domain.Entities.Inventory;

namespace API.G3Operaciones.Garcias.MX.Application.Inventory.Interfaces
{
    public interface IWarehouseService
    {
        Task<WarehouseDto> CreateAsync(CreateWarehouseDto dto);
        Task<List<WarehouseDto>> GetAllAsync();
        Task<WarehouseDto?> GetByIdAsync(Guid id);

        Task AdjustBulkStockAsync(BulkAdjustStockDto dto, MovementType movementType);
        Task DeactivateAsync(Guid id);
        Task ActivateAsync(Guid id);
    }
}
