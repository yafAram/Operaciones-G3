using API.G3Operaciones.Garcias.MX.Domain.Entities.Orders;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace API.G3Operaciones.Garcias.MX.Infrastructure.Persistence.Configurations
{
    public class OrderConfiguration : IEntityTypeConfiguration<Order>
    {
        public void Configure(EntityTypeBuilder<Order> builder)
        {
            builder.ToTable("Orders");

            builder.HasKey(x => x.Id);

            builder.Property(x => x.OperatorId)
                .IsRequired();

            builder.Property(x => x.BranchId);

            builder.Property(x => x.ChainId);

            builder.Property(x => x.BranchNameSnapshot)
                .HasMaxLength(200)
                .IsRequired();

            builder.Property(x => x.CreatedAt)
                .IsRequired();

            builder.Property(x => x.ClosedAt);

            builder.Property(x => x.Status)
                .HasConversion<int>()
                .IsRequired();

            builder.Property(x => x.Total)
                .HasPrecision(18, 2)
                .IsRequired();

            builder.Property(x => x.Latitude);

            builder.Property(x => x.Longitude);

            builder.Property(x => x.IsActive)
                .IsRequired();

            // Relación con Items (campo privado _items)
            builder.Metadata
                .FindNavigation(nameof(Order.Items))!
                .SetPropertyAccessMode(PropertyAccessMode.Field);

            builder.HasMany(typeof(OrderItem), "_items")
                .WithOne()
                .OnDelete(DeleteBehavior.Cascade);

            builder.HasIndex(x => x.OperatorId);
            builder.HasIndex(x => x.BranchId);
            builder.HasIndex(x => x.ChainId);
            builder.HasIndex(x => x.Status);
        }
    }
}
