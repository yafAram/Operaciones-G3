namespace API.G3Operaciones.Garcias.MX.Application.Users.DTOs
{
    public class CreateUserDto
    {
        public required string FullName { get; set; }
        public DateTime BirthDate { get; set; }
        public required string PhoneNumber { get; set; }

        public required string Username { get; set; }
        public required Guid RoleId { get; set; }
    }
}
