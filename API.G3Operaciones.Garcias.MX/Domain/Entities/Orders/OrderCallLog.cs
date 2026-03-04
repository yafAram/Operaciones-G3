namespace API.G3Operaciones.Garcias.MX.Domain.Entities.Orders
{
    public class OrderCallLog
    {
        public Guid Id { get; private set; }

        public Guid OperatorId { get; private set; }

        public Guid? BranchId { get; private set; }
        public Guid? ChainId { get; private set; }

        public CallResultType ResultType { get; private set; }

        public DateTime CallDate { get; private set; }

        public DateTime? ScheduledDate { get; private set; }

        protected OrderCallLog() { }

        public OrderCallLog(
            Guid operatorId,
            Guid? branchId,
            Guid? chainId,
            CallResultType resultType,
            DateTime? scheduledDate = null)
        {
            Id = Guid.NewGuid();
            OperatorId = operatorId;
            BranchId = branchId;
            ChainId = chainId;
            ResultType = resultType;
            ScheduledDate = scheduledDate;
            CallDate = DateTime.UtcNow;
        }
    }
}
