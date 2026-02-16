using API.G3Operaciones.Garcias.MX.Application.Clients.Dto;

namespace API.G3Operaciones.Garcias.MX.Application.Clients.Interfaces
{
    public interface IClientService
    {
        Task<ClientResponseDto> CreateAsync(CreateClientDto dto);
    }

}
