using API.G3Operaciones.Garcias.MX.Application.Users.DTOs;

namespace API.G3Operaciones.Garcias.MX.Application.Auth.Interfaces
{
    public interface IUserRegistrationService
    {
        Task RegisterUserAsync(CreateUserDto dto);
    }
}
