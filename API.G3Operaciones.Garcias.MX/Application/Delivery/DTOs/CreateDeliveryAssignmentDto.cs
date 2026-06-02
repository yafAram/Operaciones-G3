namespace API.G3Operaciones.Garcias.MX.Application.Delivery.DTOs
{
    public class CreateDeliveryAssignmentDto
    {
        public Guid DeliveryUserId { get; set; }
        public Guid? ChainId { get; set; }
        public Guid? BranchId { get; set; }
        public Guid? ClientId { get; set; }
    }

    public class UpdateDeliveryAssignmentDto
    {
        public Guid Id { get; set; }

        public Guid? ChainId { get; set; }
        public Guid? BranchId { get; set; }
        public Guid? ClientId { get; set; }

        public bool IsActive { get; set; }
    }

    public class DeliveryAssignmentDto
    {
        public Guid Id { get; set; }
        public Guid DeliveryUserId { get; set; }

        public Guid? ChainId { get; set; }
        public Guid? BranchId { get; set; }
        public Guid? ClientId { get; set; }

        public bool IsActive { get; set; }
        public DateTime CreatedAt { get; set; }
    }
}