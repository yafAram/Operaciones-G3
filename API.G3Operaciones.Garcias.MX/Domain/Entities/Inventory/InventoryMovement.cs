namespace API.G3Operaciones.Garcias.MX.Domain.Entities.Inventory
{
    public class InventoryMovement
    {
        public Guid Id { get; private set; }

        public Guid ProductId { get; private set; }
        public Guid WarehouseId { get; private set; }

        public decimal Quantity { get; private set; }

        public string MovementType { get; private set; } = default!;
        // IN / OUT / ADJUSTMENT

        public string? Reference { get; private set; }

        public DateTime CreatedAt { get; private set; }

        protected InventoryMovement() { }

        public InventoryMovement(
            Guid productId,
            Guid warehouseId,
            decimal quantity,
            string movementType,
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
