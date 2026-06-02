using API.G3Operaciones.Garcias.MX.Application.Chains.Dto;
using API.G3Operaciones.Garcias.MX.Application.Clients.Dto;

namespace API.G3Operaciones.Garcias.MX.Application.Chains.Interfaces
{
    public interface IChainService
    {
        Task<ChainResponseDto> CreateAsync(CreateChainDto dto);

        Task<List<ChainResponseDto>> GetAllAsync();

        Task<ChainDetailDto> GetDetailByIdAsync(Guid id);

        Task DeactivateAsync(Guid id);

    }
}
