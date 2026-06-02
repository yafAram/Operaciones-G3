namespace API.G3Operaciones.Garcias.MX.Application.Delivery.DTOs
{
    public class CreateDeliveryAssignmentPlanDto
    {
        public Guid DeliveryUserId { get; set; }
        public DateTime DeliveryDate { get; set; }
    }

    public class AddDeliveryAssignmentTargetDto
    {
        public Guid DeliveryUserId { get; set; }

        public Guid? ChainId { get; set; }
        public Guid? ClientId { get; set; }
    }

    public class DeliveryAssignmentPlanDto
    {
        public Guid Id { get; set; }
        public Guid DeliveryUserId { get; set; }
        public DateTime DeliveryDate { get; set; }
        public bool IsLocked { get; set; }

        public List<DeliveryAssignmentTargetDto> Targets { get; set; } = new();
    }

    public class DeliveryAssignmentTargetDto
    {
        public Guid Id { get; set; }
        public Guid? ChainId { get; set; }
        public Guid? ClientId { get; set; }
    }
}