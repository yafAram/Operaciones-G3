namespace API.G3Operaciones.Garcias.MX.Application.Orders.DTOs
{
    public class OrderListItemDto
    {
        public int Id { get; set; }

        public string BranchName { get; set; } = string.Empty;

        public string? ChainName { get; set; }

        public decimal TotalAmount { get; set; }

        public string Status { get; set; } = string.Empty;

        public DateTime CreatedAt { get; set; }
    }
}
