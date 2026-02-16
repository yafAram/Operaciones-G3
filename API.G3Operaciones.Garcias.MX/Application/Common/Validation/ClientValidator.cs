using System;
using System.Threading.Tasks;
using API.G3Operaciones.Garcias.MX.Application.Clients.Dto;
using API.G3Operaciones.Garcias.MX.Infrastructure.Persistence;
using Microsoft.EntityFrameworkCore;

namespace API.G3Operaciones.Garcias.MX.Application.Common.Validation
{
    public static class ClientValidator
    {
        public static async Task ValidateCreateAsync(
            CreateClientDto dto,
            ApplicationDbContext context)
        {
            if (string.IsNullOrWhiteSpace(dto.Name))
                throw new ArgumentException("El nombre es obligatorio.");

            if (string.IsNullOrWhiteSpace(dto.PhoneNumber))
                throw new ArgumentException("El teléfono es obligatorio.");

            if (string.IsNullOrWhiteSpace(dto.Address))
                throw new ArgumentException("La dirección es obligatoria.");

            if (string.IsNullOrWhiteSpace(dto.WorkSchedule))
                throw new ArgumentException("El horario es obligatorio.");

            if (dto.ChainId == null && dto.Extension != null)
                throw new InvalidOperationException("No se puede asignar extensión sin cadena.");

            if (dto.ChainId != null)
            {
                var exists = await context.Chains
                    .AnyAsync(c => c.Id == dto.ChainId && c.IsActive);

                if (!exists)
                    throw new InvalidOperationException("La cadena no existe o está inactiva.");
            }
        }
    }
}
