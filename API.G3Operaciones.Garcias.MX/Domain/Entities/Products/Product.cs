namespace API.G3Operaciones.Garcias.MX.Domain.Entities.Products
{
    public class Product
    {
        public Guid Id { get; private set; }

        public string Name { get; private set; } = default!;
        public string? Description { get; private set; }

        public string UnitOfMeasure { get; private set; } = default!;
        // Ej: "Caja", "Kg", "Pieza"

        public bool IsActive { get; private set; }

        public DateTime CreatedAt { get; private set; }

        protected Product() { }

        public Product(string name, string? description, string unitOfMeasure)
        {
            Id = Guid.NewGuid();
            Name = name;
            Description = description;
            UnitOfMeasure = unitOfMeasure;
            IsActive = true;
            CreatedAt = DateTime.UtcNow;
        }

        public void Update(string name, string? description, string unitOfMeasure)
        {
            Name = name;
            Description = description;
            UnitOfMeasure = unitOfMeasure;
        }

        public void Deactivate() => IsActive = false;
        public void Activate() => IsActive = true;
    }
}
