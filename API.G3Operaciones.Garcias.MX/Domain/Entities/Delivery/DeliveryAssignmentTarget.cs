namespace API.G3Operaciones.Garcias.MX.Domain.Entities.Delivery
{
    public class DeliveryAssignmentTarget
    {
        public Guid Id { get; private set; }

        public Guid DeliveryAssignmentPlanId { get; private set; }
        public DeliveryAssignmentPlan DeliveryAssignmentPlan { get; private set; } = null!;

        public Guid? ChainId { get; private set; }
        public Guid? ClientId { get; private set; }

        public bool IsActive { get; private set; }

        public DateTime CreatedAt { get; private set; }

        protected DeliveryAssignmentTarget() { }

        public DeliveryAssignmentTarget(Guid planId, Guid? chainId, Guid? clientId)
        {
            if (chainId == null && clientId == null)
                throw new ArgumentException("Debe especificar ChainId o ClientId.");

            Id = Guid.NewGuid();
            DeliveryAssignmentPlanId = planId;
            ChainId = chainId;
            ClientId = clientId;
            IsActive = true;
            CreatedAt = DateTime.UtcNow;
        }

        // =========================
        // DOMAIN BEHAVIOR
        // =========================

        public void Deactivate()
        {
            IsActive = false;
        }

        public void Activate()
        {
            IsActive = true;
        }
    }
}