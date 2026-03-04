using API.G3Operaciones.Garcias.MX.Domain.Entities.Pricing;
using System;
using System.Collections.Generic;

namespace API.G3Operaciones.Garcias.MX.Domain.Entities.Products
{
    public class Product
    {
        public Guid Id { get; private set; }

        public string Name { get; private set; } = null!;
        public string? Description { get; private set; }
        public string? Presentation { get; private set; }
        public string? ImageUrl { get; private set; }

        public DateTime CreatedAt { get; private set; }

        public bool IsActive { get; private set; }

        public ICollection<PriceConfiguration> Prices { get; private set; } = new List<PriceConfiguration>();
        public ICollection<Inventory.Inventory> Inventories { get; private set; } = new List<Inventory.Inventory>();

        protected Product() { }

        public Product(string name, string? description, string? presentation, string? imageUrl)
        {
            Id = Guid.NewGuid();
            Name = name;
            Description = description;
            Presentation = presentation;
            ImageUrl = imageUrl;
            CreatedAt = DateTime.UtcNow;
            IsActive = true;
        }

        public void Update(string name, string? description, string? presentation, string? imageUrl)
        {
            Name = name;
            Description = description;
            Presentation = presentation;
            ImageUrl = imageUrl;
        }

        public void Deactivate() => IsActive = false;
        public void Activate() => IsActive = true;
    }
}