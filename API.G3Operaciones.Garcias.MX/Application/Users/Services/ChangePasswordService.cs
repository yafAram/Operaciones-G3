using API.G3Operaciones.Garcias.MX.Application.Auth.DTOs;
using API.G3Operaciones.Garcias.MX.Application.Common.Security;
using API.G3Operaciones.Garcias.MX.Application.Users.Interfaces;
using API.G3Operaciones.Garcias.MX.Domain.Entities;
using API.G3Operaciones.Garcias.MX.Infrastructure.Persistence;
using Microsoft.EntityFrameworkCore;

namespace API.G3Operaciones.Garcias.MX.Application.Users.Services
{
    public class ChangePasswordService
    {
        private readonly ApplicationDbContext _db;
        private readonly IPasswordHasher _hasher;

        public ChangePasswordService(
            ApplicationDbContext db,
            IPasswordHasher hasher)
        {
            _db = db;
            _hasher = hasher;
        }

        public async Task ChangePasswordAsync(Guid userId, ChangePasswordDto dto)
        {
            var credential = await _db.Set<UserCredential>()
                .FirstOrDefaultAsync(x => x.UserId == userId);

            if (credential == null)
                throw new Exception("Credencial no encontrada");

            credential.UpdatePassword(
                _hasher.Hash(dto.NewPassword));

            await _db.SaveChangesAsync();
        }
    }
}
