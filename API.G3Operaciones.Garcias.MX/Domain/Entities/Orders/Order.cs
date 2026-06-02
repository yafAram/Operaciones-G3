using API.G3Operaciones.Garcias.MX.Domain.Entities.Clients;
using API.G3Operaciones.Garcias.MX.Domain.Entities.Enums;

namespace API.G3Operaciones.Garcias.MX.Domain.Entities.Orders
{
    public class Order
    {
        public Guid Id { get; private set; }
        public Guid OperatorId { get; private set; }

        public Guid? ClientId { get; private set; }
        public Client? Client { get; private set; }

        public Guid? ChainId { get; private set; }
        public Chain? Chain { get; private set; }

        public decimal TotalAmount { get; private set; }
        public OrderStatus Status { get; private set; }

        public string? TicketPath { get; private set; }
        public DateTime CreatedAt { get; private set; }

        private readonly List<OrderItem> _items = new();
        public IReadOnlyCollection<OrderItem> Items => _items.AsReadOnly();

        protected Order() { }

        public Order(Guid operatorId, Guid? clientId, Guid? chainId)
        {
            Id = Guid.NewGuid();
            OperatorId = operatorId;
            ClientId = clientId;
            ChainId = chainId;
            Status = OrderStatus.Pending;
            CreatedAt = DateTime.UtcNow;
        }

        public void AddItem(Guid productId, int quantity, decimal unitPrice)
        {
            if (Status != OrderStatus.Pending)
                throw new InvalidOperationException("No se pueden agregar productos a un pedido no pendiente.");

            var item = new OrderItem(Id, productId, quantity, unitPrice);
            _items.Add(item);

            CalculateTotal();
        }

        public void CalculateTotal()
        {
            TotalAmount = _items.Sum(i => i.SubTotal);
        }

        public void FinalizeOrder(string ticketPath)
        {
            if (!_items.Any())
                throw new InvalidOperationException("No se puede finalizar un pedido sin productos.");

            Status = OrderStatus.Finalized;
            TicketPath = ticketPath;
        }

        public void CancelOrder()
        {
            if (Status == OrderStatus.Delivered)
                throw new InvalidOperationException("No se puede cancelar un pedido entregado.");

            Status = OrderStatus.Cancelled;
        }

        public void MarkAsDelivered()
        {
            if (Status != OrderStatus.Finalized)
                throw new InvalidOperationException("Solo pedidos finalizados pueden entregarse.");

            Status = OrderStatus.Delivered;
        }



        public void MarkAsIncident()
        {
            if (Status != OrderStatus.Finalized)
                throw new InvalidOperationException("Solo pedidos finalizados pueden marcar incidencia.");

            Status = OrderStatus.Incident;
        }
    }
}