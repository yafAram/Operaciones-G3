using API.G3Operaciones.Garcias.MX.Application.Inventory.DTOs;
using System;

namespace API.G3Operaciones.Garcias.MX.Application.Products.DTOs
{
    public class ProductResponseDto
    {
        public Guid Id { get; set; }
        public string Name { get; set; } = null!;
        public string? Description { get; set; }
        public string? Presentation { get; set; }
        public string? ImageUrl { get; set; }
        public bool IsActive { get; set; }

        public Guid? WarehouseId { get; set; }
        public string? WarehouseName { get; set; }
        public decimal StockQuantity { get; set; }

        public decimal StockPercentage { get; set; }
        public decimal MaxStockReference { get; set; } // opcional pero recomendado

        public List<InventoryMovementDto> Movements { get; set; } = new();
    }
}