namespace API.G3Operaciones.Garcias.MX.Application.Products.DTOs
{
    public class CreateProductDto
    {
        public string Name { get; set; } = null!;
        public string? Description { get; set; }
        public string? Presentation { get; set; }
        public IFormFile? Image { get; set; }
    }
}
