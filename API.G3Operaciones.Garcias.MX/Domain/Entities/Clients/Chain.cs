namespace API.G3Operaciones.Garcias.MX.Domain.Entities.Clients
{
    public class Chain
    {
        public Guid Id { get; private set; }

        public string Name { get; private set; } = default!;

        // Número base corporativo
        public string BasePhoneNumber { get; private set; } = default!;

        public bool IsActive { get; private set; }

        public DateTime CreatedAt { get; private set; }

        public ICollection<Client> Clients { get; private set; } = new List<Client>();

        protected Chain() { }

        public Chain(string name, string basePhoneNumber)
        {
            Id = Guid.NewGuid();
            Name = name;
            BasePhoneNumber = basePhoneNumber;
            IsActive = true;
            CreatedAt = DateTime.UtcNow;
        }

        public void Update(string name, string basePhoneNumber)
        {
            Name = name;
            BasePhoneNumber = basePhoneNumber;
        }

        public void Deactivate() => IsActive = false;
        public void Activate() => IsActive = true;
    }
}
