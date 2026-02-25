using System;
using API.G3Operaciones.Garcias.MX.Domain.Entities.Products;

namespace API.G3Operaciones.Garcias.MX.Domain.Entities.Inventory
{
    public class InventoryMovement
    {
        public Guid Id { get; private set; }

        public Guid ProductId { get; private set; }
        public Product Product { get; private set; } = default!;

        public Guid WarehouseId { get; private set; }
        public Warehouse Warehouse { get; private set; } = default!;

        public decimal Quantity { get; private set; }

        public MovementType MovementType { get; private set; }

        public string? Reference { get; private set; }

        public DateTime CreatedAt { get; private set; }

        protected InventoryMovement() { }

        public InventoryMovement(
            Guid productId,
            Guid warehouseId,
            decimal quantity,
            MovementType movementType,
            string? reference)
        {
            Id = Guid.NewGuid();
            ProductId = productId;
            WarehouseId = warehouseId;
            Quantity = quantity;
            MovementType = movementType;
            Reference = reference;
            CreatedAt = DateTime.UtcNow;
        }
    }
}