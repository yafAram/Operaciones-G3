using API.G3Operaciones.Garcias.MX.Domain.Entities.Clients;
using API.G3Operaciones.Garcias.MX.Domain.Entities.Products;

namespace API.G3Operaciones.Garcias.MX.Domain.Entities.Pricing
{
    public class PriceConfiguration
    {
        public Guid Id { get; private set; }

        // 🔹 Producto obligatorio
        public Guid ProductId { get; private set; }
        public Product Product { get; private set; } = default!;

        public decimal Price { get; private set; }

        // 🔹 Puede aplicar a cadena
        public Guid? ChainId { get; private set; }
        public Chain? Chain { get; private set; }

        // 🔹 Puede aplicar a cliente
        public Guid? ClientId { get; private set; }
        public Client? Client { get; private set; }

        // Si ChainId y ClientId son null → es precio genérico
        public bool IsActive { get; private set; }

        public DateTime CreatedAt { get; private set; }

        protected PriceConfiguration() { }

        public PriceConfiguration(
            Guid productId,
            decimal price,
            Guid? chainId,
            Guid? clientId)
        {
            if (chainId != null && clientId != null)
                throw new InvalidOperationException("No puede existir precio para cadena y cliente al mismo tiempo.");

            Id = Guid.NewGuid();
            ProductId = productId;
            Price = price;
            ChainId = chainId;
            ClientId = clientId;
            IsActive = true;
            CreatedAt = DateTime.UtcNow;
        }

        public void UpdatePrice(decimal price)
        {
            Price = price;
        }

        public void Deactivate()
        {
            IsActive = false;
        }
    }
}
