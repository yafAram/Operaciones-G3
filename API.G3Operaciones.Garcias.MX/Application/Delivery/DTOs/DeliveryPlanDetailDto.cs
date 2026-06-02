namespace API.G3Operaciones.Garcias.MX.Application.Delivery.DTOs
{
    public class DeliveryPlanDetailDto
    {
        public Guid Id { get; set; }
        public Guid DeliveryUserId { get; set; }
        public DateTime Date { get; set; }
        public string Status { get; set; } = default!;

        public List<DeliveryStopDto> Stops { get; set; } = new();
    }
}
