using API.G3Operaciones.Garcias.MX.Application.Common.Security;
using API.G3Operaciones.Garcias.MX.Application.Users.DTOs;
using API.G3Operaciones.Garcias.MX.Application.Users.Interfaces;
using API.G3Operaciones.Garcias.MX.Domain.Entities;
using API.G3Operaciones.Garcias.MX.Domain.Entities.Users;
using API.G3Operaciones.Garcias.MX.Infrastructure.Persistence;
using API.G3Operaciones.Garcias.MX.Infrastructure.Security;
using Microsoft.EntityFrameworkCore;
using System.Security.Claims;

namespace API.G3Operaciones.Garcias.MX.Application.Users.Services
{
    public class UserCommandService : IUserCommandService
    {
        private readonly ApplicationDbContext _context;
        private readonly IHttpContextAccessor _httpContextAccessor;
        private readonly IPasswordHasher _passwordHasher;
        private readonly IPasswordGenerator _passwordGenerator;



        public UserCommandService(ApplicationDbContext context, IHttpContextAccessor httpContextAccessor, IPasswordHasher passwordHasher,
        IPasswordGenerator passwordGenerator){
            _context = context;
            _httpContextAccessor = httpContextAccessor;
            _passwordHasher = passwordHasher;
            _passwordGenerator = passwordGenerator;
        }


        public async Task UpdateAsync(Guid userId, UpdateUserDto dto)
        {
            var user = await _context.Users
                .FirstOrDefaultAsync(u => u.Id == userId);

            if (user == null)
                throw new Exception("Usuario no encontrado");

            var currentUserId = _httpContextAccessor.HttpContext!.User
                .FindFirst(ClaimTypes.NameIdentifier)!.Value;

            var audits = new List<AuditLog>();

            void Track(string property, string? oldValue, string? newValue)
            {
                if (oldValue != newValue)
                {
                    audits.Add(new AuditLog
                    {
                        Id = Guid.NewGuid(),
                        EntityName = "User",
                        EntityId = user.Id,
                        PropertyName = property,
                        OldValue = oldValue,
                        NewValue = newValue,
                        ModifiedBy = Guid.Parse(currentUserId),
                        ModifiedAt = DateTime.UtcNow
                    });
                }
            }

            // Auditoría
            Track("FullName", user.FirstName, dto.FullName);
            Track("PhoneNumber", user.PhoneNumber, dto.PhoneNumber);

            // Aplicar cambios usando método del dominio
            user.UpdatePersonalInfo(dto.FullName, dto.PhoneNumber);


            _context.AuditLogs.AddRange(audits);

            await _context.SaveChangesAsync();
        }



        public async Task ChangeStatusAsync(Guid userId, bool activate)
        {
            var user = await _context.Set<User>().FindAsync(userId);

            if (user == null)
                throw new Exception("Rol no encontrado");

            if (activate)
                user.Activate();
            else
                user.Deactivate();

            await _context.SaveChangesAsync();
        }


        public async Task<string> ResetPasswordAsync(Guid userId)
        {
            var credential = await _context.UserCredentials
                .FirstOrDefaultAsync(c => c.UserId == userId);

            if (credential == null)
                throw new Exception("Credenciales no encontradas");

            var temporaryPassword = _passwordGenerator.Generate();

            var hash = _passwordHasher.Hash(temporaryPassword);

            credential.ForcePasswordReset(hash);

            await _context.SaveChangesAsync();

            return temporaryPassword;
        }



    }

}
