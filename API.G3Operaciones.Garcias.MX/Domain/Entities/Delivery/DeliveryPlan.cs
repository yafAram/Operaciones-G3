using API.G3Operaciones.Garcias.MX.Domain.Entities.Enums;

namespace API.G3Operaciones.Garcias.MX.Domain.Entities.Delivery
{
    public class DeliveryPlan
    {
        public Guid Id { get; private set; }

        public Guid DeliveryUserId { get; private set; }
        public DateTime Date { get; private set; }

        public DeliveryStatus Status { get; private set; }

        public DateTime CreatedAt { get; private set; }

        private readonly List<DeliveryStop> _stops = new();
        public IReadOnlyCollection<DeliveryStop> Stops => _stops.AsReadOnly();

        protected DeliveryPlan() { }

        public DeliveryPlan(Guid deliveryUserId, DateTime date, decimal? startLatitude = null, decimal? startLongitude = null)
        {
            Id = Guid.NewGuid();
            DeliveryUserId = deliveryUserId;
            Date = date.Date;
            Status = DeliveryStatus.Pending;
            CreatedAt = DateTime.UtcNow;
        }

        public void AddStop(DeliveryStop stop)
        {
            if (Status != DeliveryStatus.Pending)
                throw new InvalidOperationException("No se pueden agregar paradas a un plan iniciado.");

            _stops.Add(stop);
        }

        public void Start()
        {
            Status = DeliveryStatus.InProgress;
        }

        public void Complete()
        {
            Status = DeliveryStatus.Completed;
        }

        public void Cancel()
        {
            Status = DeliveryStatus.Cancelled;
        }
    }
}