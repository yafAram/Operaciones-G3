using API.G3Operaciones.Garcias.MX.Application.Chains.Dto;
using API.G3Operaciones.Garcias.MX.Application.Orders.DTOs;
using API.G3Operaciones.Garcias.MX.Application.Products.DTOs;

namespace API.G3Operaciones.Garcias.MX.Application.Clients.Dto
{
    public class ClientDetailDto
    {
        public Guid Id { get; set; }
        public string Name { get; set; } = default!;
        public string PhoneNumber { get; set; } = default!;
        public string Address { get; set; } = default!;
        public string WorkSchedule { get; set; } = default!;
        public bool IsActive { get; set; }
        public DateTime CreatedAt { get; set; }

        public decimal? Latitude { get; set; }
        public decimal? Longitude { get; set; }

        public ChainInfoDto? Chain { get; set; }

        // Estadística
        public decimal TotalPurchases { get; set; }
        public int TotalOrders { get; set; }

        public List<ProductTopDto> TopProducts { get; set; } = new();
        public List<OrdersByDateDto> OrdersByDate { get; set; } = new();
    }
}
