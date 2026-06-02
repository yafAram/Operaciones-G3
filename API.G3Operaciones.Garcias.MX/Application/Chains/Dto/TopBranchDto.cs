namespace API.G3Operaciones.Garcias.MX.Application.Chains.Dto
{
    public class TopBranchDto
    {

        public int Rank { get; set; }

        public Guid BranchId { get; set; }

        public string BranchName { get; set; } = default!;

        public int TotalOrders { get; set; }

        public decimal TotalSales { get; set; }

    }
}
