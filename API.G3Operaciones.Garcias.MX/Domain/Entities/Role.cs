using API.G3Operaciones.Garcias.MX.Domain.Entities.Enums;

namespace API.G3Operaciones.Garcias.MX.Domain.Entities
{
    public class Role
    {
        public Guid Id { get; private set; }
        public string Name { get; set; } // Cambiado a set público
        public DateTime CreatedAt { get; private set; }
        public RoleStatus Status { get; private set; }

        // Constructor público para permitir la creación desde DataSeeder
        public Role()
        {
            Id = Guid.NewGuid();
            CreatedAt = DateTime.UtcNow;
            Status = RoleStatus.Active; // Ajusta según tu lógica de estados
        }
    }
}
