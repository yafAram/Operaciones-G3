// ProductService.cs
using API.G3Operaciones.Garcias.MX.Application.Inventory.DTOs;
using API.G3Operaciones.Garcias.MX.Application.Products.DTOs;
using API.G3Operaciones.Garcias.MX.Application.Products.Interfaces;
using API.G3Operaciones.Garcias.MX.Domain.Entities.Inventory;
using API.G3Operaciones.Garcias.MX.Domain.Entities.Products;
using API.G3Operaciones.Garcias.MX.Infrastructure.Persistence;
using Microsoft.EntityFrameworkCore;
using InventoryEntity = API.G3Operaciones.Garcias.MX.Domain.Entities.Inventory.Inventory;

namespace API.G3Operaciones.Garcias.MX.Application.Products.Services
{
    public class ProductService : IProductService
    {
        private readonly ApplicationDbContext _context;
        private readonly IWebHostEnvironment _environment;

        public ProductService(ApplicationDbContext context, IWebHostEnvironment environment)
        {
            _context = context;
            _environment = environment;
        }

        private async Task<string?> SaveImageAsync(IFormFile? image)
        {
            if (image is null)
                return null;

            var folderPath = Path.Combine(_environment.WebRootPath, "images");

            if (!Directory.Exists(folderPath))
                Directory.CreateDirectory(folderPath);

            var fileName = $"{Guid.NewGuid()}{Path.GetExtension(image.FileName)}";
            var filePath = Path.Combine(folderPath, fileName);

            using var stream = new FileStream(filePath, FileMode.Create);
            await image.CopyToAsync(stream);

            return $"/images/{fileName}";
        }

        public async Task<Guid> CreateAsync(CreateProductDto dto)
        {
            var imagePath = await SaveImageAsync(dto.Image);

            var product = new Product(
                dto.Name,
                dto.Description,
                dto.Presentation,
                imagePath
            );

            _context.Products.Add(product);
            await _context.SaveChangesAsync();

            return product.Id;
        }

        public async Task UpdateAsync(Guid id, UpdateProductDto dto)
        {
            var product = await _context.Products
                .Include(x => x.Inventories)
                .FirstOrDefaultAsync(x => x.Id == id)
                ?? throw new Exception("Producto no encontrado");

            var imagePath = product.ImageUrl;

            if (dto.Image != null)
            {
                imagePath = await SaveImageAsync(dto.Image);
            }

            product.Update(
                dto.Name,
                dto.Description,
                dto.Presentation,
                imagePath
            );

            if (dto.WarehouseId.HasValue && dto.WarehouseId.Value != Guid.Empty)
            {
                var warehouseExists = await _context.Warehouses
                    .AnyAsync(w => w.Id == dto.WarehouseId.Value);

                if (!warehouseExists)
                    throw new Exception("Almacén no encontrado");

                var currentInventory = await _context.Inventories
                    .FirstOrDefaultAsync(x => x.ProductId == product.Id);

                if (currentInventory is null)
                {
                    var newInventory = new InventoryEntity(product.Id, dto.WarehouseId.Value);
                    _context.Inventories.Add(newInventory);
                }
                else if (currentInventory.WarehouseId != dto.WarehouseId.Value)
                {
                    var destinationInventory = await _context.Inventories
                        .FirstOrDefaultAsync(x =>
                            x.ProductId == product.Id &&
                            x.WarehouseId == dto.WarehouseId.Value);

                    if (destinationInventory is not null)
                    {
                        destinationInventory.Increase(currentInventory.Quantity);
                        _context.Inventories.Remove(currentInventory);
                    }
                    else
                    {
                        _context.Entry(currentInventory)
                            .Property(nameof(InventoryEntity.WarehouseId))
                            .CurrentValue = dto.WarehouseId.Value;
                    }
                }
            }

            await _context.SaveChangesAsync();
        }

        public async Task DeactivateAsync(Guid id)
        {
            var product = await _context.Products
                .FirstOrDefaultAsync(x => x.Id == id)
                ?? throw new Exception("Producto no encontrado");

            product.Deactivate();
            await _context.SaveChangesAsync();
        }

        public async Task AdjustStockAsync(AdjustStockDto dto, MovementType movementType)
        {
            if (dto.WarehouseId == Guid.Empty)
                throw new Exception("WarehouseId inválido");

            var inventory = await _context.Inventories
                .FirstOrDefaultAsync(x =>
                    x.ProductId == dto.ProductId &&
                    x.WarehouseId == dto.WarehouseId);

            if (inventory == null)
            {
                inventory = new InventoryEntity(dto.ProductId, dto.WarehouseId);
                _context.Inventories.Add(inventory);
            }

            if (movementType == MovementType.In)
                inventory.Increase(dto.Quantity);
            else if (movementType == MovementType.Out)
                inventory.Decrease(dto.Quantity);

            var movement = new InventoryMovement(
                dto.ProductId,
                dto.WarehouseId,
                dto.Quantity,
                movementType,
                dto.Reference);

            _context.InventoryMovements.Add(movement);

            await _context.SaveChangesAsync();
        }

        public async Task<ProductResponseDto> GetByIdAsync(Guid id)
        {
            var product = await _context.Products
                .AsNoTracking()
                .Where(x => x.Id == id)
                .Select(x => new
                {
                    x.Id,
                    x.Name,
                    x.Description,
                    x.Presentation,
                    x.ImageUrl,
                    x.IsActive,

                    WarehouseId = x.Inventories
                        .Select(i => (Guid?)i.WarehouseId)
                        .FirstOrDefault(),

                    WarehouseName = x.Inventories
                        .Select(i => i.Warehouse.Name)
                        .FirstOrDefault(),

                    StockQuantity = x.Inventories
                        .Sum(i => (decimal?)i.Quantity) ?? 0,

                    Movements = _context.InventoryMovements
                        .Where(m => m.ProductId == x.Id)
                        .OrderBy(m => m.CreatedAt)
                        .Select(m => new InventoryMovementDto
                        {
                            Quantity = m.Quantity,
                            Reference = m.Reference,
                            MovementType = m.MovementType.ToString(),
                            CreatedAt = m.CreatedAt,
                            WarehouseId = m.WarehouseId
                        })
                        .ToList(),

                    RawMovements = _context.InventoryMovements
                        .Where(m => m.ProductId == x.Id)
                        .OrderBy(m => m.CreatedAt)
                        .Select(m => new
                        {
                            m.Quantity,
                            m.MovementType
                        })
                        .ToList()
                })
                .FirstOrDefaultAsync()
                ?? throw new Exception("Producto no encontrado");

            decimal runningStock = 0;
            decimal maxStock = 0;

            foreach (var m in product.RawMovements)
            {
                if (m.MovementType == MovementType.In)
                    runningStock += m.Quantity;
                else
                    runningStock -= m.Quantity;

                if (runningStock > maxStock)
                    maxStock = runningStock;
            }

            if (maxStock == 0)
                maxStock = product.StockQuantity;

            return new ProductResponseDto
            {
                Id = product.Id,
                Name = product.Name,
                Description = product.Description,
                Presentation = product.Presentation,
                ImageUrl = product.ImageUrl,
                IsActive = product.IsActive,
                WarehouseId = product.WarehouseId,
                WarehouseName = product.WarehouseName,

                StockQuantity = product.StockQuantity,
                MaxStockReference = maxStock,

                StockPercentage = maxStock > 0
                    ? Math.Round((product.StockQuantity / maxStock) * 100, 2)
                    : 0,

                Movements = product.Movements
            };
        }

        public async Task<List<ProductResponseDto>> GetAllAsync()
        {
            var products = await _context.Products
                .AsNoTracking()
                .Select(x => new
                {
                    x.Id,
                    x.Name,
                    x.Description,
                    x.Presentation,
                    x.ImageUrl,
                    x.IsActive,

                    WarehouseId = x.Inventories
                        .Select(i => (Guid?)i.WarehouseId)
                        .FirstOrDefault(),

                    WarehouseName = x.Inventories
                        .Select(i => i.Warehouse.Name)
                        .FirstOrDefault(),

                    StockQuantity = x.Inventories
                        .Sum(i => (decimal?)i.Quantity) ?? 0,

                    Movements = _context.InventoryMovements
                        .Where(m => m.ProductId == x.Id)
                        .OrderBy(m => m.CreatedAt)
                        .Select(m => new
                        {
                            m.Quantity,
                            m.MovementType
                        })
                        .ToList()
                })
                .ToListAsync();

            return products.Select(p =>
            {
                decimal runningStock = 0;
                decimal maxStock = 0;

                foreach (var m in p.Movements)
                {
                    if (m.MovementType == MovementType.In)
                        runningStock += m.Quantity;
                    else
                        runningStock -= m.Quantity;

                    if (runningStock > maxStock)
                        maxStock = runningStock;
                }

                if (maxStock == 0)
                    maxStock = p.StockQuantity;

                return new ProductResponseDto
                {
                    Id = p.Id,
                    Name = p.Name,
                    Description = p.Description,
                    Presentation = p.Presentation,
                    ImageUrl = p.ImageUrl,
                    IsActive = p.IsActive,
                    WarehouseId = p.WarehouseId,
                    WarehouseName = p.WarehouseName,

                    StockQuantity = p.StockQuantity,
                    MaxStockReference = maxStock,

                    StockPercentage = maxStock > 0
                        ? Math.Round((p.StockQuantity / maxStock) * 100, 2)
                        : 0
                };
            }).ToList();
        }

        public async Task<ProductCardsDto> GetCardsAsync()
        {
            var products = await _context.Products
                .AsNoTracking()
                .Select(x => new
                {
                    x.Id,
                    x.IsActive
                })
                .ToListAsync();

            var inventories = await _context.Inventories
                .AsNoTracking()
                .GroupBy(i => i.ProductId)
                .Select(g => new
                {
                    ProductId = g.Key,
                    Stock = g.Sum(x => x.Quantity)
                })
                .ToListAsync();

            var result = products.Select(p =>
            {
                var stock = inventories
                    .FirstOrDefault(i => i.ProductId == p.Id)?.Stock ?? 0;

                return new
                {
                    p.IsActive,
                    StockQuantity = stock
                };
            }).ToList();

            return new ProductCardsDto
            {
                TotalActiveProducts = result.Count(x => x.IsActive),

                LowStockProducts = result.Count(x =>
                    x.IsActive &&
                    x.StockQuantity > 0 &&
                    x.StockQuantity < 30), // 🔥 ya simplificado

                ExhaustedProducts = result.Count(x =>
                    x.IsActive &&
                    x.StockQuantity <= 0)
            };
        }

        public async Task ActivateAsync(Guid id)
        {
            var product = await _context.Products
                .FirstOrDefaultAsync(x => x.Id == id)
                ?? throw new Exception("Producto no encontrado");

            product.Activate();
            await _context.SaveChangesAsync();
        }

        public async Task<List<ProductTopDto>> GetTopProductsAsync(int top)
        {
            return await _context.InventoryMovements
                .Where(x => x.MovementType == MovementType.Out)
                .GroupBy(x => new { x.ProductId, x.Product!.Name })
                .Select(g => new ProductTopDto
                {
                    ProductId = g.Key.ProductId,
                    Name = g.Key.Name,
                    TotalSold = g.Sum(x => x.Quantity)
                })
                .OrderByDescending(x => x.TotalSold)
                .Take(top)
                .ToListAsync();
        }
    }
}