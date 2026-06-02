namespace API.G3Operaciones.Garcias.MX.Application.Orders.DTOs
{
    public class LogisticsChainDto
    {
        public Guid? ChainId { get; set; }
        public string ChainName { get; set; } = string.Empty;

        public int TotalOrders { get; set; }

        public List<LogisticsBranchDto> Branches { get; set; } = new();
    }
}
