namespace API.G3Operaciones.Garcias.MX.Application.Users.DTOs
{
    public class UpdateUserDto
    {
        public required string FirstName { get; set; }
        public required string LastName { get; set; }
        public required string PhoneNumber { get; set; }
        public required DateTime BirthDate { get; set; }
    }

}
