using System;

namespace API.G3Operaciones.Garcias.MX.Application.Products.DTOs
{
    public class ProductResponseDto
    {
        public Guid Id { get; set; }
        public string Name { get; set; } = null!;
        public string? Description { get; set; }
        public string? Presentation { get; set; }
        public string? ImageUrl { get; set; }
        public bool IsActive { get; set; }
    }
}