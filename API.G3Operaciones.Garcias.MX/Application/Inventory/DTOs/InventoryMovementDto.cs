namespace API.G3Operaciones.Garcias.MX.Application.Inventory.DTOs
{
    public class InventoryMovementDto
    {
        public decimal Quantity { get; set; }
        public string MovementType { get; set; } = null!;
        public string Reference { get; set; } = null!;
        public DateTime CreatedAt { get; set; }
        public Guid WarehouseId { get; set; }
    }
}
