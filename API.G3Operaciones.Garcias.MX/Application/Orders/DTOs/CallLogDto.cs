namespace API.G3Operaciones.Garcias.MX.Application.Orders.DTOs
{
    public class CallLogDto
    {
        public int Id { get; set; }

        public int OrderId { get; set; }

        public string Result { get; set; } = string.Empty;

        public string? Notes { get; set; }

        public int CreatedByUserId { get; set; }

        public DateTime CreatedAt { get; set; }

        public DateTime? NextCallDate { get; set; }
    }
}
