using API.G3Operaciones.Garcias.MX.Application.Pricing.DTOs;

namespace API.G3Operaciones.Garcias.MX.Application.Pricing.Interfaces
{
    public interface IPriceConfigurationService
    {
        Task<Guid> CreateAsync(CreatePriceConfigurationDto dto);

        Task UpdatePriceAsync(Guid id, UpdatePriceConfigurationDto dto);

        Task DeactivateAsync(Guid id);

        Task<PriceConfigurationResponseDto?> GetByIdAsync(Guid id);

        Task<List<PriceConfigurationResponseDto>> GetByProductAsync(Guid productId);

        Task<decimal> ResolvePriceAsync(Guid productId, Guid? chainId, Guid? clientId);
    }
}
