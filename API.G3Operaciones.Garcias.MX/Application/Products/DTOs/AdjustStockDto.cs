namespace API.G3Operaciones.Garcias.MX.Application.Products.DTOs
{
    public class AdjustStockDto
    {
        public Guid ProductId { get; set; }
        public Guid WarehouseId { get; set; }
        public decimal Quantity { get; set; }
        public string Reference { get; set; } = null!;
    }
}
