using API.G3Operaciones.Garcias.MX.Application.Orders.DTOs;
using API.G3Operaciones.Garcias.MX.Application.Products.DTOs;

namespace API.G3Operaciones.Garcias.MX.Application.Chains.Dto
{
    public class ChainDetailDto
    {

        // INFO CADENA
        public Guid ChainId { get; set; }
        public string ChainName { get; set; } = default!;

        // SUCURSALES
        public int TotalBranches { get; set; }
        public List<BranchSummaryDto> Branches { get; set; } = new();

        // KPIs GENERALES
        public decimal MonthlySales { get; set; }
        public int TotalOrders { get; set; }

        // TOP SUCURSALES (1-5)
        public List<TopBranchDto> TopBranches { get; set; } = new();

        // MEJOR SUCURSAL
        public TopBranchDto? BestBranch { get; set; }
    }
}
