using API.G3Operaciones.Garcias.MX.Domain.Entities.Products;

namespace API.G3Operaciones.Garcias.MX.Domain.Entities.Inventory
{
    public class Inventory
    {
        public Guid Id { get; private set; }

        public Guid ProductId { get; private set; }
        public Product Product { get; private set; } = default!;

        public Guid WarehouseId { get; private set; }
        public Warehouse Warehouse { get; private set; } = default!;

        public decimal Quantity { get; private set; }

        public DateTime LastUpdated { get; private set; }

        protected Inventory() { }

        public Inventory(Guid productId, Guid warehouseId)
        {
            Id = Guid.NewGuid();
            ProductId = productId;
            WarehouseId = warehouseId;
            Quantity = 0;
            LastUpdated = DateTime.UtcNow;
        }

        public void Increase(decimal quantity)
        {
            Quantity += quantity;
            LastUpdated = DateTime.UtcNow;
        }

        public void Decrease(decimal quantity)
        {
            if (Quantity < quantity)
                throw new InvalidOperationException("Stock insuficiente.");

            Quantity -= quantity;
            LastUpdated = DateTime.UtcNow;
        }
    }
}
