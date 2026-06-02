using API.G3Operaciones.Garcias.MX.Application.Users.DTOs;

namespace API.G3Operaciones.Garcias.MX.Application.Users.Interfaces
{
    public interface IUserCommandService
    {
        Task UpdateAsync(Guid userId, UpdateUserDto dto);
        Task ChangeStatusAsync(Guid userId, bool activate);
        Task<string> ResetPasswordAsync(Guid userId);

    }
}
