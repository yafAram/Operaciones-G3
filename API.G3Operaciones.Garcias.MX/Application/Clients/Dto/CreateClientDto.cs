namespace API.G3Operaciones.Garcias.MX.Application.Clients.Dto
{
    public class CreateClientDto
    {
        public string Name { get; set; } = default!;
        public string PhoneNumber { get; set; } = default!;
        public string Address { get; set; } = default!;
        public string WorkSchedule { get; set; } = default!;
        public Guid? ChainId { get; set; }
        public string? Extension { get; set; }
    }

}
