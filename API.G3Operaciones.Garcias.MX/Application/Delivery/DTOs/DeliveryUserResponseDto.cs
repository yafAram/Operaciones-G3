namespace API.G3Operaciones.Garcias.MX.Application.Delivery.DTOs
{

    public class DeliveryUserResponseDto
    {
        public Guid Id { get; set; }

        public string FirstName { get; set; } = string.Empty;
        public string LastName { get; set; } = string.Empty;

        public string PhoneNumber { get; set; } = string.Empty;

        public int Status { get; set; }

        public Guid RoleId { get; set; }
    }
}
