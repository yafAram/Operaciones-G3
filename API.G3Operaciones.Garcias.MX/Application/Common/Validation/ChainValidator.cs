using System;
using System.Threading.Tasks;
using API.G3Operaciones.Garcias.MX.Application.Chains.Dto;
using API.G3Operaciones.Garcias.MX.Infrastructure.Persistence;
using Microsoft.EntityFrameworkCore;


namespace API.G3Operaciones.Garcias.MX.Application.Common.Validation
{
    public static class ChainValidator
    {
        public static async Task ValidateCreateAsync(
            CreateChainDto dto,
            ApplicationDbContext context)
        {
            if (string.IsNullOrWhiteSpace(dto.Name))
                throw new ArgumentException("El nombre es obligatorio.");

            if (string.IsNullOrWhiteSpace(dto.BasePhoneNumber))
                throw new ArgumentException("El número base es obligatorio.");

            var exists = await context.Chains
                .AnyAsync(c => c.Name == dto.Name);

            if (exists)
                throw new InvalidOperationException("Ya existe una cadena con ese nombre.");
        }
    }
}
