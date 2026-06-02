namespace API.G3Operaciones.Garcias.MX.Application.Products.DTOs
{
    public class BulkAdjustStockDto
    {
        public Guid WarehouseId { get; set; }
        public string Reference { get; set; } = null!; // Aquí mandarás el ID de la Orden
        public List<StockItemDto> Items { get; set; } = new();
    }
}
