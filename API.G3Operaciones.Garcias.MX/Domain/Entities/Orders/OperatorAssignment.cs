namespace API.G3Operaciones.Garcias.MX.Domain.Entities.Orders
{
    public class OperatorAssignment
    {
        public Guid Id { get; private set; }

        public Guid OperatorId { get; private set; }

        public Guid? BranchId { get; private set; }
        public Guid? ChainId { get; private set; }

        public DayOfWeek AssignedDay { get; private set; }

        public bool IsActive { get; private set; }

        public DateTime CreatedAt { get; private set; }

        protected OperatorAssignment() { }

        public OperatorAssignment(
            Guid operatorId,
            DayOfWeek assignedDay,
            Guid? branchId,
            Guid? chainId)
        {
            if (branchId != null && chainId != null)
                throw new InvalidOperationException("No puede asignarse sucursal y cadena simultáneamente.");

            Id = Guid.NewGuid();
            OperatorId = operatorId;
            AssignedDay = assignedDay;
            BranchId = branchId;
            ChainId = chainId;
            IsActive = true;
            CreatedAt = DateTime.UtcNow;
        }

        public void Deactivate() => IsActive = false;
    }
}
