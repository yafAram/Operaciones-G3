namespace API.G3Operaciones.Garcias.MX.Domain.Entities
{
    public class AuditLog
    {
        public Guid Id { get; set; }
        public string EntityName { get; set; } = default!;
        public Guid EntityId { get; set; }

        public string PropertyName { get; set; } = default!;
        public string? OldValue { get; set; }
        public string? NewValue { get; set; }

        public Guid ModifiedBy { get; set; }
        public DateTime ModifiedAt { get; set; }
    }
}
