using API.G3Operaciones.Garcias.MX.Domain.Entities.Enums;

namespace API.G3Operaciones.Garcias.MX.Domain.Entities.Delivery
{
    public class DeliveryStop
    {
        public Guid Id { get; private set; }

        public Guid DeliveryPlanId { get; private set; }
        public DeliveryPlan DeliveryPlan { get; private set; } = null!;

        public Guid OrderId { get; private set; }
        public Guid ClientId { get; private set; }

        public decimal Latitude { get; private set; }
        public decimal Longitude { get; private set; }

        public int Sequence { get; private set; }
        public int RouteGroup { get; private set; }

        public DeliveryStopStatus Status { get; private set; }

        public string? FailureReason { get; private set; }

        public DateTime? ArrivedAt { get; private set; }
        public DateTime? CompletedAt { get; private set; }

        public DateTime CreatedAt { get; private set; }

        protected DeliveryStop() { }

        public DeliveryStop(
            Guid deliveryPlanId,
            Guid orderId,
            Guid clientId,
            decimal latitude,
            decimal longitude,
            int sequence,
            int routeGroup)
        {
            Id = Guid.NewGuid();
            DeliveryPlanId = deliveryPlanId;
            OrderId = orderId;
            ClientId = clientId;
            Latitude = latitude;
            Longitude = longitude;
            Sequence = sequence;
            RouteGroup = routeGroup;
            Status = DeliveryStopStatus.Pending;
            CreatedAt = DateTime.UtcNow;
        }

        public void MarkInRoute()
        {
            Status = DeliveryStopStatus.InRoute;
            ArrivedAt = DateTime.UtcNow;
        }

        public void MarkAsDelivered()
        {
            Status = DeliveryStopStatus.Delivered;
            CompletedAt = DateTime.UtcNow;
        }

        public void MarkAsFailed(string reason)
        {
            Status = DeliveryStopStatus.Failed;
            FailureReason = reason;
            CompletedAt = DateTime.UtcNow;
        }
    }
}