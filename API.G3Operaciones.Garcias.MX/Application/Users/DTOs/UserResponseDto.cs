namespace API.G3Operaciones.Garcias.MX.Application.Users.DTOs
{
    public class UserResponseDto
    {
        public Guid Id { get; set; }

        public string FullName { get; set; }

        public string PhoneNumber { get; set; }

        public string Role { get; set; }

        public string Username { get; set; } = string.Empty;


        public string Status { get; set; }
    }
}
