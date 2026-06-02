using API.G3Operaciones.Garcias.MX.Application.Clients.Dto;

namespace API.G3Operaciones.Garcias.MX.Application.Clients.Interfaces
{
    public interface IClientService
    {
        Task<ClientResponseDto> CreateAsync(CreateClientDto dto);
        Task<ClientResponseDto> UpdateAsync(Guid id, UpdateClientDto dto);
        Task<List<ClientResponseDto>> GetAllAsync();

        Task DeactivateAsync(Guid id);

        Task ActivateAsync(Guid id);

        Task<ClientDetailDto> GetDetailByIdAsync(Guid id);

    }

}
