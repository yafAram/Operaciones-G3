namespace API.G3Operaciones.Garcias.MX.Application.Delivery.DTOs
{
    public class DeliveryTicketDto
    {
        public string CompanyName { get; set; } = string.Empty;

        public string BranchName { get; set; } = string.Empty;
        public string? ChainName { get; set; }

        public string Address { get; set; } = string.Empty;

        public string DriverName { get; set; } = string.Empty;

        public DateTime DeliveredAt { get; set; }

        public List<DeliveryTicketItemDto> Items { get; set; } = new();

        public decimal Subtotal { get; set; }
        public decimal Total { get; set; }
    }

    public class DeliveryTicketItemDto
    {
        public string ProductName { get; set; } = string.Empty;
        public int Quantity { get; set; }
        public decimal UnitPrice { get; set; }
        public decimal Total => Quantity * UnitPrice;
    }
}
