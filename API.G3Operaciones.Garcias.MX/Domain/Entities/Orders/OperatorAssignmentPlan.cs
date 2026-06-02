namespace API.G3Operaciones.Garcias.MX.Domain.Entities.Orders
{
    public class OperatorAssignmentPlan
    {
        public Guid Id { get; private set; }
        public Guid OperatorId { get; private set; }
        public DateTime Date { get; private set; }
        public bool IsLocked { get; private set; }

        // 🔥 CRÍTICO: La lista debe ser trackeable por Entity Framework
        public List<OperatorAssignmentTarget> Targets { get; private set; } = new List<OperatorAssignmentTarget>();

        private OperatorAssignmentPlan() { }

        public OperatorAssignmentPlan(Guid operatorId, DateTime date)
        {
            Id = Guid.NewGuid();
            OperatorId = operatorId;
            Date = date;
            IsLocked = false;
        }

        public void AddClient(Guid clientId)
        {
            Targets.Add(new OperatorAssignmentTarget(Id, clientId, null));
        }

        public void AddChain(Guid chainId)
        {
            Targets.Add(new OperatorAssignmentTarget(Id, null, chainId));
        }

        public void ClearTargets()
        {
            Targets.Clear();
        }
    }
}