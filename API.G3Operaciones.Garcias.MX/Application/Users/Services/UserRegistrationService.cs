using API.G3Operaciones.Garcias.MX.Application.Common.Security;
using API.G3Operaciones.Garcias.MX.Application.Common.Validation;
using API.G3Operaciones.Garcias.MX.Application.Users.DTOs;
using API.G3Operaciones.Garcias.MX.Application.Users.Interfaces;
using API.G3Operaciones.Garcias.MX.Domain.Entities;
using API.G3Operaciones.Garcias.MX.Domain.Entities.Enums;
using API.G3Operaciones.Garcias.MX.Domain.Entities.Users;
using API.G3Operaciones.Garcias.MX.Infrastructure.Persistence;
using Microsoft.EntityFrameworkCore;
using System.ComponentModel.DataAnnotations;
using System.Text.RegularExpressions;

namespace API.G3Operaciones.Garcias.MX.Application.Users.Services
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

        public async Task<(bool Success, string? Error)> RegisterUserAsync(CreateUserDto dto)
        {
            var role = await _context.Set<Role>()
                .FirstOrDefaultAsync(r => r.Id == dto.RoleId && r.Status == RoleStatus.Active);

            if (role == null)
                return (false, "Rol inválido");

            var exists = await _context.Set<UserCredential>()
                .AnyAsync(x => x.Username == dto.Username);

            if (!UsernameValidator.IsValid(dto.Username))
                throw new ValidationException(
                    "Formato Incorrecto"
                );

            if (!UsernameValidator.IsValid(dto.Username))
                throw new ValidationException(
                    "Formato Incorrecto"
                );

            if (exists)
                return (false, "El nombre de usuario ya existe");




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

            var temporaryPassword = dto.Username;

            var credential = new UserCredential(
                user.Id,
                dto.Username,
                _passwordHasher.Hash(temporaryPassword)
            );

            _context.Add(credential);
            await _context.SaveChangesAsync();

            return (true, null);
        }


    }
}
