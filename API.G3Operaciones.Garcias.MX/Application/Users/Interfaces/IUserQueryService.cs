using API.G3Operaciones.Garcias.MX.Application.Users.DTOs;

namespace API.G3Operaciones.Garcias.MX.Application.Users.Interfaces
{
    public interface IUserQueryService
    {
        Task<List<UserResponseDto>> GetAllAsync();

    }
}
