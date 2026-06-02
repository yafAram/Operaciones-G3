using API.G3Operaciones.Garcias.MX.Application.Products.DTOs;
using API.G3Operaciones.Garcias.MX.Domain.Entities.Inventory;

namespace API.G3Operaciones.Garcias.MX.Application.Products.Interfaces
{
    public interface IProductService
    {
        Task<Guid> CreateAsync(CreateProductDto dto);
        Task UpdateAsync(Guid id, UpdateProductDto dto);
        Task DeactivateAsync(Guid id);

        Task AdjustStockAsync(AdjustStockDto dto, MovementType movementType);

        Task<ProductResponseDto> GetByIdAsync(Guid id);
        Task<ProductCardsDto> GetCardsAsync();
        Task<List<ProductResponseDto>> GetAllAsync();

        Task ActivateAsync(Guid id);
        Task<List<ProductTopDto>> GetTopProductsAsync(int top);
    }
}
