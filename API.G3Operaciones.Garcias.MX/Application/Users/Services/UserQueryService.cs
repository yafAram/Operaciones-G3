using API.G3Operaciones.Garcias.MX.Application.Users.DTOs;
using API.G3Operaciones.Garcias.MX.Application.Users.Interfaces;
using API.G3Operaciones.Garcias.MX.Domain.Entities;
using API.G3Operaciones.Garcias.MX.Domain.Entities.Users;
using API.G3Operaciones.Garcias.MX.Infrastructure.Persistence;
using Microsoft.EntityFrameworkCore;

namespace API.G3Operaciones.Garcias.MX.Application.Users.Services
{
    public class UserQueryService : IUserQueryService
    {
        private readonly ApplicationDbContext _context;

        public UserQueryService(ApplicationDbContext context)
        {
            _context = context;
        }

        public async Task<List<UserResponseDto>> GetAllAsync()
        {
            var users = await (
                from u in _context.Users
                join c in _context.UserCredentials
                    on u.Id equals c.UserId
                join r in _context.Roles
                    on u.RoleId equals r.Id
                select new UserResponseDto
                {
                    Id = u.Id,
                    FullName = u.FirstName + " " + u.LastName,
                    PhoneNumber = u.PhoneNumber,
                    Role = r.Name,
                    Username = c.Username,
                    Status = u.Status.ToString()
                }
            ).ToListAsync();

            return users;
        }


    }

}
