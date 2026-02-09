using API.G3Operaciones.Garcias.MX.Application.Users.DTOs;

namespace API.G3Operaciones.Garcias.MX.Application.Users.Interfaces
{
    public interface IUserRegistrationService
    {
        Task<(bool Success, string? Error)> RegisterUserAsync(CreateUserDto dto);
    }
}
