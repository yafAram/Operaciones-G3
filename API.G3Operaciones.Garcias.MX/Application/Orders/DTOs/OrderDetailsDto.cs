namespace API.G3Operaciones.Garcias.MX.Application.Orders.DTOs
{
    public class OrderDetailsDto
    {
        public int Id { get; set; }

        public string BranchName { get; set; } = string.Empty;

        public string? ChainName { get; set; }

        public string Status { get; set; } = string.Empty;

        public decimal TotalAmount { get; set; }

        public DateTime CreatedAt { get; set; }

        public List<OrderItemDto> Items { get; set; } = new();

        public List<OrderHistoryDto> History { get; set; } = new();
    }
}
