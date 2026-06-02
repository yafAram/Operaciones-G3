namespace API.G3Operaciones.Garcias.MX.Domain.Entities.Orders
{
    public class OperatorAssignmentTarget
    {
        public Guid Id { get; private set; }

        public Guid OperatorAssignmentPlanId { get; private set; }

        public Guid? ClientId { get; private set; }
        public Guid? ChainId { get; private set; }

        public bool IsActive { get; private set; }

        // 🔥 CRÍTICO: Propiedad de navegación obligatoria para EF Core
        public OperatorAssignmentPlan Plan { get; private set; }

        private OperatorAssignmentTarget() { }

        // Cambiado a public para asegurar que EF y los servicios puedan instanciarlo
        public OperatorAssignmentTarget(Guid planId, Guid? clientId, Guid? chainId)
        {
            Id = Guid.NewGuid();
            OperatorAssignmentPlanId = planId;
            ClientId = clientId;
            ChainId = chainId;
            IsActive = true;
        }

        public void Deactivate() => IsActive = false;
    }
}