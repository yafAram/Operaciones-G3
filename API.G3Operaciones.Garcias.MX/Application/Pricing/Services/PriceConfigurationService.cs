using API.G3Operaciones.Garcias.MX.Application.Pricing.DTOs;
using API.G3Operaciones.Garcias.MX.Application.Pricing.Interfaces;
using API.G3Operaciones.Garcias.MX.Domain.Entities.Pricing;
using API.G3Operaciones.Garcias.MX.Infrastructure.Persistence;
using Microsoft.EntityFrameworkCore;

namespace API.G3Operaciones.Garcias.MX.Application.Pricing.Services
{
    public class PriceConfigurationService : IPriceConfigurationService
    {
        private readonly ApplicationDbContext _context;

        public PriceConfigurationService(ApplicationDbContext context)
        {
            _context = context;
        }

        public async Task<Guid> CreateAsync(CreatePriceConfigurationDto dto)
        {
            var exists = await _context.Set<PriceConfiguration>()
                .AnyAsync(x =>
                    x.ProductId == dto.ProductId &&
                    x.ChainId == dto.ChainId &&
                    x.ClientId == dto.ClientId &&
                    x.IsActive);

            if (exists)
                throw new Exception("Ya existe una configuración de precio activa con estos parámetros.");

            var config = new PriceConfiguration(
                dto.ProductId,
                dto.Price,
                dto.ChainId,
                dto.ClientId
            );

            _context.Add(config);
            await _context.SaveChangesAsync();

            return config.Id;
        }

        public async Task UpdatePriceAsync(Guid id, UpdatePriceConfigurationDto dto)
        {
            var config = await _context.Set<PriceConfiguration>()
                .FirstOrDefaultAsync(x => x.Id == id && x.IsActive)
                ?? throw new Exception("Configuración no encontrada.");

            config.UpdatePrice(dto.Price);
            await _context.SaveChangesAsync();
        }

        public async Task DeactivateAsync(Guid id)
        {
            var config = await _context.Set<PriceConfiguration>()
                .FirstOrDefaultAsync(x => x.Id == id)
                ?? throw new Exception("Configuración no encontrada.");

            config.Deactivate();
            await _context.SaveChangesAsync();
        }

        public async Task<PriceConfigurationResponseDto?> GetByIdAsync(Guid id)
        {
            return await _context.Set<PriceConfiguration>()
                .Where(x => x.Id == id)
                .Select(x => new PriceConfigurationResponseDto
                {
                    Id = x.Id,
                    ProductId = x.ProductId,
                    Price = x.Price,
                    ChainId = x.ChainId,
                    ClientId = x.ClientId,
                    IsActive = x.IsActive
                })
                .FirstOrDefaultAsync();
        }

        public async Task<List<PriceConfigurationResponseDto>> GetByProductAsync(Guid productId)
        {
            return await _context.Set<PriceConfiguration>()
                .Where(x => x.ProductId == productId)
                .Select(x => new PriceConfigurationResponseDto
                {
                    Id = x.Id,
                    ProductId = x.ProductId,
                    Price = x.Price,
                    ChainId = x.ChainId,
                    ClientId = x.ClientId,
                    IsActive = x.IsActive
                })
                .ToListAsync();
        }

        public async Task<decimal> ResolvePriceAsync(Guid productId, Guid? chainId, Guid? clientId)
        {
            // 1️⃣ Precio por Cliente
            if (clientId != null)
            {
                var clientPrice = await _context.Set<PriceConfiguration>()
                    .Where(x => x.ProductId == productId &&
                                x.ClientId == clientId &&
                                x.IsActive)
                    .OrderByDescending(x => x.CreatedAt)
                    .FirstOrDefaultAsync();

                if (clientPrice != null)
                    return clientPrice.Price;
            }

            // 2️⃣ Precio por Cadena
            if (chainId != null)
            {
                var chainPrice = await _context.Set<PriceConfiguration>()
                    .Where(x => x.ProductId == productId &&
                                x.ChainId == chainId &&
                                x.IsActive)
                    .OrderByDescending(x => x.CreatedAt)
                    .FirstOrDefaultAsync();

                if (chainPrice != null)
                    return chainPrice.Price;
            }

            // 3️⃣ Precio Genérico
            var genericPrice = await _context.Set<PriceConfiguration>()
                .Where(x => x.ProductId == productId &&
                            x.ChainId == null &&
                            x.ClientId == null &&
                            x.IsActive)
                .OrderByDescending(x => x.CreatedAt)
                .FirstOrDefaultAsync();

            if (genericPrice != null)
                return genericPrice.Price;

            throw new Exception("No existe configuración de precio para este producto.");
        }
    }
}
