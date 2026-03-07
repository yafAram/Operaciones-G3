namespace API.G3Operaciones.Garcias.MX.Application.Orders.DTOs
{
    public class CreateCallLogDto
    {
        public int OrderId { get; set; }

        public string Result { get; set; } = string.Empty;

        public string? Notes { get; set; }

        public DateTime? NextCallDate { get; set; }
    }
}
