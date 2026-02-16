namespace API.G3Operaciones.Garcias.MX.Application.Chains.Dto
{
    public class ChainResponseDto
    {
        public Guid Id { get; set; }
        public string Name { get; set; } = default!;
        public string BasePhoneNumber { get; set; } = default!;
        public bool IsActive { get; set; }
        public DateTime CreatedAt { get; set; }
    }
}
