namespace API.G3Operaciones.Garcias.MX.Application.Inventory.DTOs
{
    public class WarehouseProductDto
    {
        public Guid ProductId { get; set; }
        public string Name { get; set; } = null!;
        public string? ImageUrl { get; set; }
        public decimal Quantity { get; set; }
        public decimal Percentage { get; set; }
    }
}
