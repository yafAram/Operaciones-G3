namespace API.G3Operaciones.Garcias.MX.Application.Orders.DTOs
{
    public class CreateOrderRequestDto
    {
        public int BranchId { get; set; }

        public int? ChainId { get; set; }

        public decimal? Latitude { get; set; }

        public decimal? Longitude { get; set; }

        public string? Notes { get; set; }
    }
}
