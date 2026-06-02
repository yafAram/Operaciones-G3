using API.G3Operaciones.Garcias.MX.Domain.Entities.Enums;

namespace API.G3Operaciones.Garcias.MX.Domain.Entities.Delivery
{
    public class DeliveryAssignmentPlan
    {
        public Guid Id { get; private set; }

        public Guid DeliveryUserId { get; private set; }

        public DateTime DeliveryDate { get; private set; }

        public bool IsLocked { get; private set; }

        public DateTime CreatedAt { get; private set; }

        private readonly List<DeliveryAssignmentTarget> _targets = new();
        public IReadOnlyCollection<DeliveryAssignmentTarget> Targets => _targets.AsReadOnly();

        protected DeliveryAssignmentPlan() { }

        public DeliveryAssignmentPlan(Guid deliveryUserId, DateTime deliveryDate)
        {
            Id = Guid.NewGuid();
            DeliveryUserId = deliveryUserId;
            DeliveryDate = deliveryDate.Date;
            CreatedAt = DateTime.UtcNow;
            IsLocked = false;
        }

        // =========================
        // DOMAIN BEHAVIOR
        // =========================

        public void AddTarget(DeliveryAssignmentTarget target)
        {
            // 🔥 ya no bloqueamos aquí
            _targets.Add(target);
        }

        public void RemoveTarget(Guid targetId)
        {
            if (IsLocked)
                throw new InvalidOperationException("El plan está bloqueado.");

            var target = _targets.FirstOrDefault(x => x.Id == targetId)
                ?? throw new InvalidOperationException("Target no encontrado.");

            target.Deactivate();
        }

        public void Lock()
        {
            if (!_targets.Any(x => x.IsActive))
                throw new InvalidOperationException("No puedes bloquear un plan sin asignaciones.");

            IsLocked = true;
        }

        public void Unlock()
        {
            IsLocked = false;
        }
    }
}