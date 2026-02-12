using API.G3Operaciones.Garcias.MX.Application.Roles.DTOs;
using API.G3Operaciones.Garcias.MX.Application.Users.DTOs;

namespace API.G3Operaciones.Garcias.MX.Application.Users.Interfaces
{
    public interface IRoleCommandService
    {
        Task<Guid> CreateAsync(CreateRoleDto dto);
        Task UpdateAsync(Guid roleId, UpdateRoleDto dto);
        Task ChangeStatusAsync(Guid roleId, bool activate);
        Task AssignRoleToUserAsync(Guid userId, Guid roleId);
    }
}
