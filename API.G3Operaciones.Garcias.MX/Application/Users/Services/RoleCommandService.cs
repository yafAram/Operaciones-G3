using API.G3Operaciones.Garcias.MX.Application.Roles.DTOs;
using API.G3Operaciones.Garcias.MX.Application.Users.DTOs;
using API.G3Operaciones.Garcias.MX.Application.Users.Interfaces;
using API.G3Operaciones.Garcias.MX.Domain.Entities;
using API.G3Operaciones.Garcias.MX.Domain.Entities.Enums;
using API.G3Operaciones.Garcias.MX.Domain.Entities.Users;
using API.G3Operaciones.Garcias.MX.Infrastructure.Persistence;

namespace API.G3Operaciones.Garcias.MX.Application.Users.Services
{
    public class RoleCommandService : IRoleCommandService
    {
        private readonly ApplicationDbContext _context;

        public RoleCommandService(ApplicationDbContext context)
        {
            _context = context;
        }

        public async Task<Guid> CreateAsync(CreateRoleDto dto)
        {
            var role = new Role(dto.Name);

            _context.Add(role);
            await _context.SaveChangesAsync();

            return role.Id;
        }

        public async Task UpdateAsync(Guid roleId, UpdateRoleDto dto)
        {
            var role = await _context.Set<Role>().FindAsync(roleId);

            if (role == null)
                throw new Exception("Rol no encontrado");

            role.Update(dto.Name);

            await _context.SaveChangesAsync();
        }

        public async Task ChangeStatusAsync(Guid roleId, bool activate)
        {
            var role = await _context.Set<Role>().FindAsync(roleId);

            if (role == null)
                throw new Exception("Rol no encontrado");

            if (activate)
                role.Activate();
            else
                role.Deactivate();

            await _context.SaveChangesAsync();
        }

        public async Task AssignRoleToUserAsync(Guid userId, Guid roleId)
        {
            var user = await _context.Set<User>().FindAsync(userId);
            var role = await _context.Set<Role>().FindAsync(roleId);

            if (user == null || role == null)
                throw new Exception("Usuario o Rol no encontrado");

            if (role.Status != RoleStatus.Active)
                throw new Exception("No se puede asignar un rol inactivo");

            user.ChangeRole(roleId);

            await _context.SaveChangesAsync();
        }
    }
}
