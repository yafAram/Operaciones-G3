namespace API.G3Operaciones.Garcias.MX.Domain.Entities.Orders
{
    public class OrderItem
    {
        public Guid Id { get; private set; }

        public Guid ProductId { get; private set; }

        public string ProductNameSnapshot { get; private set; } = string.Empty;

        public decimal UnitPrice { get; private set; }

        public int Quantity { get; private set; }

        public decimal SubTotal { get; private set; }

        protected OrderItem() { }

        public OrderItem(
            Guid productId,
            string productNameSnapshot,
            decimal unitPrice,
            int quantity)
        {
            Id = Guid.NewGuid();
            ProductId = productId;
            ProductNameSnapshot = productNameSnapshot;
            UnitPrice = unitPrice;
            Quantity = quantity;
            SubTotal = unitPrice * quantity;
        }

        public void UpdateQuantity(int quantity)
        {
            Quantity = quantity;
            SubTotal = UnitPrice * quantity;
        }
    }
}
