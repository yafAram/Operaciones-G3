namespace API.G3Operaciones.Garcias.MX.Application.Pricing.DTOs
{
    public class CreatePriceConfigurationDto
    {
        public Guid ProductId { get; set; }
        public decimal Price { get; set; }
        public Guid? ChainId { get; set; }
        public Guid? ClientId { get; set; }
    }
}
