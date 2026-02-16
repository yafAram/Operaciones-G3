using API.G3Operaciones.Garcias.MX.Application.Chains.Dto;

namespace API.G3Operaciones.Garcias.MX.Application.Chains.Interfaces
{
    public interface IChainService
    {
        Task<ChainResponseDto> CreateAsync(CreateChainDto dto);
    }
}
