namespace API.G3Operaciones.Garcias.MX.Application.Orders.DTOs
{
    public class LogisticsBranchDto
    {
        public Guid? ClientId { get; set; }
        public string ClientName { get; set; } = string.Empty;

        public int TotalOrders { get; set; }

        public Guid? AssignedOperatorId { get; set; }
        public string? AssignedOperatorName { get; set; }
    }
}
