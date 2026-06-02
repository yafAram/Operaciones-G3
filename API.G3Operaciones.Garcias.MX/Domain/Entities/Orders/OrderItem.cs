using API.G3Operaciones.Garcias.MX.Domain.Entities.Products;

namespace API.G3Operaciones.Garcias.MX.Domain.Entities.Orders
{
    public class OrderItem
    {
        public Guid Id { get; private set; }

        public Guid OrderId { get; private set; }
        public Guid ProductId { get; private set; }

        public Product? Product { get; private set; }

        public int Quantity { get; private set; }
        public decimal UnitPrice { get; private set; }
        public decimal SubTotal { get; private set; }

        protected OrderItem() { }

        public OrderItem(Guid orderId, Guid productId, int quantity, decimal unitPrice)
        {
            Id = Guid.NewGuid();
            OrderId = orderId;
            ProductId = productId;
            Quantity = quantity;
            UnitPrice = unitPrice;
            SubTotal = quantity * unitPrice;
        }
    }
}