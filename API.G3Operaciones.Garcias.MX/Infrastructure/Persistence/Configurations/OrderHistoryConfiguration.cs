using API.G3Operaciones.Garcias.MX.Domain.Entities.Orders;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace API.G3Operaciones.Garcias.MX.Infrastructure.Persistence.Configurations
{
    public class OrderHistoryConfiguration : IEntityTypeConfiguration<OrderHistory>
    {
        public void Configure(EntityTypeBuilder<OrderHistory> builder)
        {
            builder.ToTable("OrderHistories");

            builder.HasKey(x => x.Id);

            builder.Property(x => x.OrderId)
                .IsRequired();

            builder.Property(x => x.ModifiedByUserId)
                .IsRequired();

            builder.Property(x => x.Action)
                .HasMaxLength(300)
                .IsRequired();

            builder.Property(x => x.OldValue)
                .HasMaxLength(1000);

            builder.Property(x => x.NewValue)
                .HasMaxLength(1000);

            builder.Property(x => x.ModifiedAt)
                .IsRequired();

            builder.HasIndex(x => x.OrderId);
            builder.HasIndex(x => x.ModifiedByUserId);
        }
    }
}
