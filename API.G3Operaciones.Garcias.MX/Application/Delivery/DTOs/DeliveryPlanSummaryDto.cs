namespace API.G3Operaciones.Garcias.MX.Application.Delivery.DTOs
{
    public class DeliveryPlanSummaryDto
    {
        public Guid Id { get; set; }

        public Guid DeliveryUserId { get; set; }
        public string DeliveryUserName { get; set; } = string.Empty;

        public DateTime Date { get; set; }
        public string Status { get; set; } = string.Empty;

        public int TotalStops { get; set; }
        public int DeliveredStops { get; set; }
        public int FailedStops { get; set; }
        public int PendingStops { get; set; }
    }
}
