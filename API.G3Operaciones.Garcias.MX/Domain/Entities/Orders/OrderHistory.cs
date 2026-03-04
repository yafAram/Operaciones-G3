namespace API.G3Operaciones.Garcias.MX.Domain.Entities.Orders
{
    public class OrderHistory
    {
        public Guid Id { get; private set; }

        public Guid OrderId { get; private set; }

        public Guid ModifiedByUserId { get; private set; }

        public string Action { get; private set; } = string.Empty;

        public string? OldValue { get; private set; }
        public string? NewValue { get; private set; }

        public DateTime ModifiedAt { get; private set; }

        protected OrderHistory() { }

        public OrderHistory(
            Guid orderId,
            Guid modifiedByUserId,
            string action,
            string? oldValue,
            string? newValue)
        {
            Id = Guid.NewGuid();
            OrderId = orderId;
            ModifiedByUserId = modifiedByUserId;
            Action = action;
            OldValue = oldValue;
            NewValue = newValue;
            ModifiedAt = DateTime.UtcNow;
        }
    }
}
