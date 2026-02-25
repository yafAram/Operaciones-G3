using System;
using System.Collections.Generic;

namespace API.G3Operaciones.Garcias.MX.Domain.Entities.Inventory
{
    public class Warehouse
    {
        public Guid Id { get; private set; }

        public string Name { get; private set; } = default!;
        public string Location { get; private set; } = default!;

        public bool IsActive { get; private set; }

        public DateTime CreatedAt { get; private set; }

        public ICollection<Inventory> Inventories { get; private set; } = new List<Inventory>();
        public ICollection<InventoryMovement> Movements { get; private set; } = new List<InventoryMovement>();

        protected Warehouse() { }

        public Warehouse(string name, string location)
        {
            Id = Guid.NewGuid();
            Name = name;
            Location = location;
            IsActive = true;
            CreatedAt = DateTime.UtcNow;
        }

        public void Deactivate() => IsActive = false;
        public void Activate() => IsActive = true;
    }
}