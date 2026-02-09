using API.G3Operaciones.Garcias.MX.Application.Auth.DTOs;

namespace API.G3Operaciones.Garcias.MX.Application.Auth.Interfaces
{
    public interface IAuthService
    {
        Task<LoginResponseDto> LoginAsync(LoginRequestDto request);
        Task<ChangePasswordResultDto> ChangePasswordAsync(Guid userId, ChangePasswordDto dto);

    }
}
