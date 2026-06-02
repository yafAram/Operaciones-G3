namespace API.G3Operaciones.Garcias.MX.Application.Chains.Dto
{
    public class BranchSummaryDto
    {

        public Guid Id { get; set; }

        public string Name { get; set; } = default!;
        public string? Extension { get; set; }


        public string PhoneNumber { get; set; } = default!;

        public string Address { get; set; } = default!;

        public bool IsActive { get; set; }

        public decimal? Latitude { get; set; }
        public decimal? Longitude { get; set; }

        public int TotalOrders { get; set; }

        public decimal TotalSales { get; set; }

    }
}
