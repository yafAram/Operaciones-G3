using API.G3Operaciones.Garcias.MX.Application.Inventory.DTOs;
using API.G3Operaciones.Garcias.MX.Application.Inventory.Interfaces;
using API.G3Operaciones.Garcias.MX.Application.Products.DTOs;
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

        public async Task AdjustBulkStockAsync(BulkAdjustStockDto dto, MovementType type)
        {
            // 1. Manejo inteligente de transacciones anidadas
            using var transaction = _context.Database.CurrentTransaction == null
                ? await _context.Database.BeginTransactionAsync()
                : null;

            try
            {
                // 2. Verificar que el almacén exista
                var warehouseExists = await _context.Set<Warehouse>().AnyAsync(w => w.Id == dto.WarehouseId);
                if (!warehouseExists)
                    throw new Exception("WarehouseId inválido");

                // 3. Procesar cada producto en lote
                foreach (var item in dto.Items)
                {
                    // Buscar el inventario exacto de este producto en este almacén
                    var inventory = await _context.Set<API.G3Operaciones.Garcias.MX.Domain.Entities.Inventory.Inventory>()
                        .FirstOrDefaultAsync(i => i.WarehouseId == dto.WarehouseId && i.ProductId == item.ProductId);

                    // Si no existe el registro de inventario
                    if (inventory == null)
                    {
                        if (type == MovementType.Out)
                            throw new InvalidOperationException($"Stock insuficiente para el producto {item.ProductId}.");

                        // Si es una entrada de un producto nuevo en este almacén, lo creamos
                        inventory = new API.G3Operaciones.Garcias.MX.Domain.Entities.Inventory.Inventory(item.ProductId, dto.WarehouseId);
                        _context.Set<API.G3Operaciones.Garcias.MX.Domain.Entities.Inventory.Inventory>().Add(inventory);
                    }

                    // 4. Actualizar cantidades usando los métodos de tu entidad
                    if (type == MovementType.In)
                    {
                        inventory.Increase((decimal)item.Quantity);
                    }
                    else if (type == MovementType.Out)
                    {
                        inventory.Decrease((decimal)item.Quantity);
                    }

                    // NOTA: Si en tu código original tenías líneas aquí para registrar 
                    // el historial (Movement), puedes pegarlas justo aquí abajo.
                }

                // 5. Guardar los cambios
                await _context.SaveChangesAsync();

                // 6. Hacer Commit SOLO si nosotros creamos la transacción
                if (transaction != null)
                {
                    await transaction.CommitAsync();
                }
            }
            catch (Exception)
            {
                // 7. Hacer Rollback SOLO si nosotros creamos la transacción
                if (transaction != null)
                {
                    await transaction.RollbackAsync();
                }
                throw;
            }
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
            var warehouse = await _context.Warehouses
                .Where(w => w.Id == id)
                .Select(w => new WarehouseDto
                {
                    Id = w.Id,
                    Name = w.Name,
                    Location = w.Location,
                    IsActive = w.IsActive,
                    CreatedAt = w.CreatedAt,

                    Products = _context.Inventories
                        .Where(i => i.WarehouseId == w.Id)
                        .Select(i => new WarehouseProductDto
                        {
                            ProductId = i.ProductId,
                            Name = i.Product.Name,
                            ImageUrl = i.Product.ImageUrl,
                            Quantity = i.Quantity
                        })
                        .ToList(),

                    Movements = _context.InventoryMovements
                        .Where(m => m.WarehouseId == w.Id)
                        .OrderByDescending(m => m.CreatedAt)
                        .Select(m => new InventoryMovementDto
                        {
                            Quantity = m.Quantity,
                            MovementType = m.MovementType.ToString(),
                            Reference = m.Reference,
                            CreatedAt = m.CreatedAt,
                            WarehouseId = m.WarehouseId
                        })
                        .ToList()
                })
                .FirstOrDefaultAsync();

            if (warehouse == null) return null;

            var max = warehouse.Products.Any()
                ? warehouse.Products.Max(p => p.Quantity)
                : 1;

            foreach (var p in warehouse.Products)
            {
                p.Percentage = max == 0 ? 0 : Math.Round((p.Quantity / max) * 100, 2);
            }

            return warehouse;
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
