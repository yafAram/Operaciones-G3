using API.G3Operaciones.Garcias.MX.Domain.Entities.Pricing;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace API.G3Operaciones.Garcias.MX.Infrastructure.Persistence.Configurations
{
    public class PriceConfigurationConfiguration : IEntityTypeConfiguration<PriceConfiguration>
    {
        public void Configure(EntityTypeBuilder<PriceConfiguration> builder)
        {
            builder.HasKey(p => p.Id);

            builder.Property(p => p.Price)
                .HasColumnType("decimal(18,2)")
                .IsRequired();

            builder.Property(p => p.IsActive)
                .IsRequired();

            builder.Property(p => p.CreatedAt)
                .IsRequired();

            builder.HasOne(p => p.Product)
                .WithMany(p => p.Prices)
                .HasForeignKey(p => p.ProductId);

            builder.HasOne(p => p.Chain)
                .WithMany()
                .HasForeignKey(p => p.ChainId)
                .OnDelete(DeleteBehavior.Restrict);
        }
    }
}