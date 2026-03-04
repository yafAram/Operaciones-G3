namespace API.G3Operaciones.Garcias.MX.Domain.Entities.Orders
{
    public class Order
    {
        public Guid Id { get; private set; }

        public Guid OperatorId { get; private set; }

        public Guid? BranchId { get; private set; }
        public Guid? ChainId { get; private set; }

        public string BranchNameSnapshot { get; private set; } = string.Empty;

        public DateTime CreatedAt { get; private set; }
        public DateTime? ClosedAt { get; private set; }

        public OrderStatus Status { get; private set; }

        public decimal Total { get; private set; }

        public double? Latitude { get; private set; }
        public double? Longitude { get; private set; }

        public bool IsActive { get; private set; }

        private readonly List<OrderItem> _items = new();
        public IReadOnlyCollection<OrderItem> Items => _items;

        protected Order() { }

        public Order(
            Guid operatorId,
            Guid? branchId,
            Guid? chainId,
            string branchNameSnapshot,
            double? latitude,
            double? longitude)
        {
            Id = Guid.NewGuid();
            OperatorId = operatorId;
            BranchId = branchId;
            ChainId = chainId;
            BranchNameSnapshot = branchNameSnapshot;
            Latitude = latitude;
            Longitude = longitude;
            CreatedAt = DateTime.UtcNow;
            Status = OrderStatus.Created;
            IsActive = true;
        }

        public void AddItem(OrderItem item)
        {
            _items.Add(item);
            RecalculateTotal();
        }

        public void Cancel()
        {
            Status = OrderStatus.Cancelled;
            IsActive = false;
        }

        public void Close()
        {
            Status = OrderStatus.Completed;
            ClosedAt = DateTime.UtcNow;
        }

        private void RecalculateTotal()
        {
            Total = _items.Sum(x => x.SubTotal);
        }
    }
}
