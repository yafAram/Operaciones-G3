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

        public ProductService(
            ApplicationDbContext context,
            IWebHostEnvironment environment)
        {
            _context = context;
            _environment = environment;
        }

        public async Task<Guid> CreateAsync(CreateProductDto dto)
        {
            string? imagePath = null;

            if (dto.Image != null)
            {
                var folderPath = Path.Combine(_environment.WebRootPath, "images");

                if (!Directory.Exists(folderPath))
                    Directory.CreateDirectory(folderPath);

                var fileName = $"{Guid.NewGuid()}{Path.GetExtension(dto.Image.FileName)}";
                var filePath = Path.Combine(folderPath, fileName);

                using var stream = new FileStream(filePath, FileMode.Create);
                await dto.Image.CopyToAsync(stream);

                imagePath = $"/images/{fileName}";
            }

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
                .FirstOrDefaultAsync(x => x.Id == id)
                ?? throw new Exception("Producto no encontrado");

            string? imagePath = product.ImageUrl;

            if (dto.Image != null)
            {
                var folderPath = Path.Combine(_environment.WebRootPath, "images");

                if (!Directory.Exists(folderPath))
                    Directory.CreateDirectory(folderPath);

                var fileName = $"{Guid.NewGuid()}{Path.GetExtension(dto.Image.FileName)}";
                var filePath = Path.Combine(folderPath, fileName);

                using var stream = new FileStream(filePath, FileMode.Create);
                await dto.Image.CopyToAsync(stream);

                imagePath = $"/images/{fileName}";
            }

            product.Update(dto.Name, dto.Description, dto.Presentation, imagePath);

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
            using var transaction = await _context.Database.BeginTransactionAsync();

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
            await transaction.CommitAsync();
        }

        public async Task<ProductResponseDto> GetByIdAsync(Guid id)
        {
            var product = await _context.Products
                .AsNoTracking()
                .FirstOrDefaultAsync(x => x.Id == id)
                ?? throw new Exception("Producto no encontrado");

            return new ProductResponseDto
            {
                Id = product.Id,
                Name = product.Name,
                Description = product.Description,
                Presentation = product.Presentation,
                ImageUrl = product.ImageUrl,
                IsActive = product.IsActive
            };
        }

        public async Task<List<ProductResponseDto>> GetAllAsync()
        {
            return await _context.Products
                .AsNoTracking()
                .Select(x => new ProductResponseDto
                {
                    Id = x.Id,
                    Name = x.Name,
                    Description = x.Description,
                    Presentation = x.Presentation,
                    ImageUrl = x.ImageUrl,
                    IsActive = x.IsActive
                })
                .ToListAsync();
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