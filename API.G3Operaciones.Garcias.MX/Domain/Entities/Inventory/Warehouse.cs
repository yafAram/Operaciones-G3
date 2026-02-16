namespace API.G3Operaciones.Garcias.MX.Domain.Entities.Inventory
{
    public class Warehouse
    {
        public Guid Id { get; private set; }

        public string Name { get; private set; } = default!;
        public string Location { get; private set; } = default!;

        public bool IsActive { get; private set; }

        public DateTime CreatedAt { get; private set; }

        protected Warehouse() { }

        public Warehouse(string name, string location)
        {
            Id = Guid.NewGuid();
            Name = name;
            Location = location;
            IsActive = true;
            CreatedAt = DateTime.UtcNow;
        }
    }
}
