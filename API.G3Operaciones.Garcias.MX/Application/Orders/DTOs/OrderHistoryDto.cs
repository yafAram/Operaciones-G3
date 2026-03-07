namespace API.G3Operaciones.Garcias.MX.Application.Orders.DTOs
{
    public class OrderHistoryDto
    {
        public string FieldChanged { get; set; } = string.Empty;

        public string? OldValue { get; set; }

        public string? NewValue { get; set; }

        public int ChangedByUserId { get; set; }

        public DateTime ChangedAt { get; set; }
    }
}
