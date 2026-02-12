using API.G3Operaciones.Garcias.MX.Application.Roles.DTOs;
using API.G3Operaciones.Garcias.MX.Application.Users.Interfaces;
using API.G3Operaciones.Garcias.MX.Infrastructure.Persistence;
using Microsoft.EntityFrameworkCore;

namespace API.G3Operaciones.Garcias.MX.Application.Users.Services
{
    public class RoleQueryService : IRoleQueryService
    {
        private readonly ApplicationDbContext _context;

        public RoleQueryService(ApplicationDbContext context)
        {
            _context = context;
        }

        public async Task<List<RoleResponseDto>> GetAllAsync()
        {
            return await _context.Roles
                .Select(r => new RoleResponseDto
                {
                    Id = r.Id,
                    Name = r.Name,
                    Status = r.Status.ToString()
                })
                .ToListAsync();
        }

        public async Task<RoleResponseDto?> GetByIdAsync(Guid roleId)
        {
            return await _context.Roles
                .Where(r => r.Id == roleId)
                .Select(r => new RoleResponseDto
                {
                    Id = r.Id,
                    Name = r.Name,
                    Status = r.Status.ToString()
                })
                .FirstOrDefaultAsync();
        }
    }
}
