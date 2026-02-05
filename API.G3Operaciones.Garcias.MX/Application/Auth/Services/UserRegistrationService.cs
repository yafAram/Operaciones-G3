using API.G3Operaciones.Garcias.MX.Application.Auth.Interfaces;
using API.G3Operaciones.Garcias.MX.Application.Users.DTOs;
using API.G3Operaciones.Garcias.MX.Domain.Entities;
using API.G3Operaciones.Garcias.MX.Domain.Entities.API.G3Operaciones.Garcias.MX.Domain.Entities;
using API.G3Operaciones.Garcias.MX.Domain.Entities.Enums;
using API.G3Operaciones.Garcias.MX.Domain.Entities.Users;
using API.G3Operaciones.Garcias.MX.Infrastructure.Persistence;
using Microsoft.EntityFrameworkCore;

namespace API.G3Operaciones.Garcias.MX.Application.Auth.Services
{
    public class UserRegistrationService : IUserRegistrationService
    {
        private readonly ApplicationDbContext _context;
        private readonly IPasswordHasher _passwordHasher;

        public UserRegistrationService(
            ApplicationDbContext context,
            IPasswordHasher passwordHasher)
        {
            _context = context;
            _passwordHasher = passwordHasher;
        }

        public async Task RegisterUserAsync(CreateUserDto dto)
        {
            var role = await _context.Set<Role>()
                .FirstOrDefaultAsync(r => r.Id == dto.RoleId && r.Status == RoleStatus.Active);

            if (role == null)
                throw new Exception("Rol inválido");

            var exists = await _context.Set<UserCredential>()
                .AnyAsync(x => x.Username == dto.Username);

            if (exists)
                throw new Exception("El nombre de usuario ya existe");

            // Reemplaza la creación del usuario para separar el nombre completo en nombre y apellido
            var names = dto.FullName.Split(' ', 2, StringSplitOptions.RemoveEmptyEntries);
            var firstName = names.Length > 0 ? names[0] : string.Empty;
            var lastName = names.Length > 1 ? names[1] : string.Empty;

            var user = new User(
                firstName,
                lastName,
                dto.BirthDate,
                dto.PhoneNumber,
                role.Id
            );


            _context.Add(user);
            await _context.SaveChangesAsync();

            var temporaryPassword = GenerateTemporaryPassword();

            var credential = new UserCredential(
                user.Id,
                dto.Username,
                _passwordHasher.Hash(temporaryPassword)
            );


            _context.Add(credential);
            await _context.SaveChangesAsync();

            // IMPORTANTE:
            // La contraseña temporal se devuelve SOLO al admin
            // El frontend decidirá cómo mostrarla o entregarla
        }

        private static string GenerateTemporaryPassword()
        {
            return $"Tmp-{Guid.NewGuid():N}".Substring(0, 12);
        }
    }
}
