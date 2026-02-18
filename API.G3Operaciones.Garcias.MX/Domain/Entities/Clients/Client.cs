namespace API.G3Operaciones.Garcias.MX.Domain.Entities.Clients
{
    public class Client
    {
        public Guid Id { get; private set; }

        public string Name { get; private set; } = default!;

        public Guid? ChainId { get; private set; }
        public Chain? Chain { get; private set; }

        public string PhoneNumber { get; private set; } = default!;
        public string? Extension { get; private set; }

        public string Address { get; private set; } = default!;
        public string WorkSchedule { get; private set; } = default!;
        public decimal? Latitude { get; private set; }
        public decimal? Longitude { get; private set; }

        public bool IsActive { get; private set; }
        public DateTime CreatedAt { get; private set; }
        public DateTime? DeactivatedAt { get; private set; }


        protected Client() { }

        public Client(
            string name,
            string phoneNumber,
            string address,
            string workSchedule,
            Guid? chainId = null,
            string? extension = null,
            decimal? latitude = null,
            decimal? longitude = null)
        {
            ValidateExtension(chainId, extension);

            Id = Guid.NewGuid();
            Name = name;
            PhoneNumber = phoneNumber;
            Address = address;
            WorkSchedule = workSchedule;
            ChainId = chainId;
            Extension = extension;

            Latitude = latitude;
            Longitude = longitude;

            IsActive = true;
            CreatedAt = DateTime.UtcNow;
        }


        public void Update(
            string name,
            string phoneNumber,
            string address,
            string workSchedule)
        {
            Name = name;
            PhoneNumber = phoneNumber;
            Address = address;
            WorkSchedule = workSchedule;
        }

        public void ChangeChain(Guid? newChainId, string? extension)
        {
            ValidateExtension(newChainId, extension);

            ChainId = newChainId;
            Extension = newChainId == null ? null : extension;
        }

        public void UpdateLocation(decimal? latitude, decimal? longitude)
        {
            Latitude = latitude;
            Longitude = longitude;
        }

        private void ValidateExtension(Guid? chainId, string? extension)
        {
            if (chainId == null && extension != null)
                throw new InvalidOperationException("No se puede asignar extensión sin cadena.");
        }

        public void Deactivate()
        {
            if (!IsActive)
                throw new InvalidOperationException("El cliente ya está desactivado.");

            IsActive = false;
            DeactivatedAt = DateTime.UtcNow;
        }

        public void Activate()
        {
            IsActive = true;
            DeactivatedAt = null;
        }
    }
}
