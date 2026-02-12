using API.G3Operaciones.Garcias.MX.Application.Roles.DTOs;

namespace API.G3Operaciones.Garcias.MX.Application.Users.Interfaces
{
    public interface IRoleQueryService
    {
        Task<List<RoleResponseDto>> GetAllAsync();
        Task<RoleResponseDto?> GetByIdAsync(Guid roleId);
    }
}
