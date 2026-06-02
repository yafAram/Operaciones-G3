namespace API.G3Operaciones.Garcias.MX.Application.Inventory.DTOs
{
    public class WarehouseDto
    {
        public Guid Id { get; set; }
        public string Name { get; set; } = default!;
        public string Location { get; set; } = default!;
        public bool IsActive { get; set; }
        public DateTime CreatedAt { get; set; }


        public List<WarehouseProductDto> Products { get; set; } = new();
        public List<InventoryMovementDto> Movements { get; set; } = new();
    }
}
