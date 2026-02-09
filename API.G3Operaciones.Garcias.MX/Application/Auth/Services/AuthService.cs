using API.G3Operaciones.Garcias.MX.Application.Auth.DTOs;
using API.G3Operaciones.Garcias.MX.Application.Auth.Interfaces;
using API.G3Operaciones.Garcias.MX.Application.Common.Validation;
using API.G3Operaciones.Garcias.MX.Application.Users.Interfaces;
using API.G3Operaciones.Garcias.MX.Domain.Entities;
using API.G3Operaciones.Garcias.MX.Domain.Entities.API.G3Operaciones.Garcias.MX.Domain.Entities;
using API.G3Operaciones.Garcias.MX.Infrastructure.Persistence;
using Microsoft.EntityFrameworkCore;


namespace API.G3Operaciones.Garcias.MX.Application.Auth.Services
{
    public class AuthService : IAuthService
    {
        private readonly ApplicationDbContext _context;
        private readonly IPasswordHasher _passwordHasher;
        private readonly IJwtTokenGenerator _jwtTokenGenerator;

        public AuthService(
            ApplicationDbContext context,
            IPasswordHasher passwordHasher,
            IJwtTokenGenerator jwtTokenGenerator)
        {
            _context = context;
            _passwordHasher = passwordHasher;
            _jwtTokenGenerator = jwtTokenGenerator;
        }

        public async Task<LoginResponseDto?> LoginAsync(LoginRequestDto request)
        {
            var credential = await _context.Set<UserCredential>()
                .Include(x => x.User)
                    .ThenInclude(u => u.Role)
                .FirstOrDefaultAsync(x => x.Username == request.Username);

            if (credential == null || credential.IsLocked)
                return null; // Usuario no encontrado o bloqueado

            var isValidPassword = _passwordHasher.Verify(request.Password, credential.PasswordHash);

            if (!isValidPassword)
                return null; // Contraseña inválida

            var token = _jwtTokenGenerator.GenerateToken(credential.User, credential.MustChangePassword);

            return new LoginResponseDto
            {
                Token = token,
                MustChangePassword = credential.MustChangePassword
            };
        }



        public async Task<ChangePasswordResultDto> ChangePasswordAsync(Guid userId, ChangePasswordDto dto)
        {
            var credential = await _context.Set<UserCredential>()
                .FirstOrDefaultAsync(x => x.UserId == userId);

            if (credential == null)
            {
                return new ChangePasswordResultDto
                {
                    Success = false,
                    Message = "Credenciales no encontradas"
                };
            }

            if (!_passwordHasher.Verify(dto.CurrentPassword, credential.PasswordHash))
            {
                return new ChangePasswordResultDto
                {
                    Success = false,
                    Message = "La contraseña actual es incorrecta"
                };
            }

            if (!PasswordValidator.IsValid(dto.NewPassword))
            {
                return new ChangePasswordResultDto
                {
                    Success = false,
                    Message =
                        "La nueva contraseña debe tener entre 8 y 32 caracteres, " +
                        "al menos una mayúscula, un número y un símbolo"
                };
            }

            var newHash = _passwordHasher.Hash(dto.NewPassword);

            credential.UpdatePassword(newHash);

            await _context.SaveChangesAsync();

            return new ChangePasswordResultDto
            {
                Success = true,
                Message = "Contraseña actualizada correctamente"
            };
        }



    }
}
