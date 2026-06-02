namespace API.G3Operaciones.Garcias.MX.Domain.Entities.Delivery
{
    public class DeliveryIncident
    {
        public Guid Id { get; private set; }

        public Guid DeliveryStopId { get; private set; }
        public Guid OrderId { get; private set; }

        public string Reason { get; private set; } = null!;

        public DateTime CreatedAt { get; private set; }

        protected DeliveryIncident() { }

        public DeliveryIncident(Guid deliveryStopId, Guid orderId, string reason)
        {
            Id = Guid.NewGuid();
            DeliveryStopId = deliveryStopId;
            OrderId = orderId;
            Reason = reason;
            CreatedAt = DateTime.UtcNow;
        }
    }
}