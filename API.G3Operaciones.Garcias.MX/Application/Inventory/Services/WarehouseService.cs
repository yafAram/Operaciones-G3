using API.G3Operaciones.Garcias.MX.Application.Inventory.DTOs;
using API.G3Operaciones.Garcias.MX.Application.Inventory.Interfaces;
using API.G3Operaciones.Garcias.MX.Domain.Entities.Inventory;
using API.G3Operaciones.Garcias.MX.Infrastructure.Persistence;
using Microsoft.EntityFrameworkCore;

namespace API.G3Operaciones.Garcias.MX.Application.Inventory.Services
{
    public class WarehouseService : IWarehouseService
    {
        private readonly ApplicationDbContext _context;

        public WarehouseService(ApplicationDbContext context)
        {
            _context = context;
        }

        public async Task<WarehouseDto> CreateAsync(CreateWarehouseDto dto)
        {
            var warehouse = new Warehouse(dto.Name, dto.Location);

            _context.Warehouses.Add(warehouse);
            await _context.SaveChangesAsync();

            return new WarehouseDto
            {
                Id = warehouse.Id,
                Name = warehouse.Name,
                Location = warehouse.Location,
                IsActive = warehouse.IsActive,
                CreatedAt = warehouse.CreatedAt
            };
        }

        public async Task<List<WarehouseDto>> GetAllAsync()
        {
            return await _context.Warehouses
                .Select(x => new WarehouseDto
                {
                    Id = x.Id,
                    Name = x.Name,
                    Location = x.Location,
                    IsActive = x.IsActive,
                    CreatedAt = x.CreatedAt
                })
                .ToListAsync();
        }

        public async Task<WarehouseDto?> GetByIdAsync(Guid id)
        {
            return await _context.Warehouses
                .Where(x => x.Id == id)
                .Select(x => new WarehouseDto
                {
                    Id = x.Id,
                    Name = x.Name,
                    Location = x.Location,
                    IsActive = x.IsActive,
                    CreatedAt = x.CreatedAt
                })
                .FirstOrDefaultAsync();
        }

        public async Task DeactivateAsync(Guid id)
        {
            var warehouse = await _context.Warehouses.FindAsync(id);

            if (warehouse is null)
                throw new Exception("Warehouse no encontrado");

            warehouse.Deactivate();
            await _context.SaveChangesAsync();
        }

        public async Task ActivateAsync(Guid id)
        {
            var warehouse = await _context.Warehouses.FindAsync(id);

            if (warehouse is null)
                throw new Exception("Warehouse no encontrado");

            warehouse.Activate();
            await _context.SaveChangesAsync();
        }
    }
}
