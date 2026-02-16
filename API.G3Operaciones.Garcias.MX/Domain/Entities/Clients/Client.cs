namespace API.G3Operaciones.Garcias.MX.Domain.Entities.Clients
{
    public class Client
    {
        public Guid Id { get; private set; }

        public string Name { get; private set; } = default!;

        // Puede o no pertenecer a una cadena
        public Guid? ChainId { get; private set; }
        public Chain? Chain { get; private set; }

        // Teléfono propio de la sucursal
        public string PhoneNumber { get; private set; } = default!;

        // Extensión (solo aplica si pertenece a cadena)
        public string? Extension { get; private set; }

        public string Address { get; private set; } = default!;

        public string WorkSchedule { get; private set; } = default!;

        public bool IsActive { get; private set; }

        public DateTime CreatedAt { get; private set; }

        protected Client() { }

        public Client(
            string name,
            Guid? chainId,
            string phoneNumber,
            string? extension,
            string address,
            string workSchedule)
        {
            Id = Guid.NewGuid();
            Name = name;
            ChainId = chainId;
            PhoneNumber = phoneNumber;
            Extension = extension;
            Address = address;
            WorkSchedule = workSchedule;
            IsActive = true;
            CreatedAt = DateTime.UtcNow;
        }

        public void Update(
            string name,
            string phoneNumber,
            string? extension,
            string address,
            string workSchedule)
        {
            Name = name;
            PhoneNumber = phoneNumber;
            Extension = extension;
            Address = address;
            WorkSchedule = workSchedule;
        }

        public void AssignToChain(Guid chainId)
        {
            ChainId = chainId;
        }

        public void RemoveFromChain()
        {
            ChainId = null;
            Extension = null;
        }

        public void Deactivate() => IsActive = false;
        public void Activate() => IsActive = true;
    }
}
