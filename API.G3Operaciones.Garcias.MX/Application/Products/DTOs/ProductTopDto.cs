namespace API.G3Operaciones.Garcias.MX.Application.Products.DTOs
{
    public class ProductTopDto
    {
        public Guid ProductId { get; set; }
        public string Name { get; set; } = null!;
        public decimal TotalSold { get; set; }
    }
}
