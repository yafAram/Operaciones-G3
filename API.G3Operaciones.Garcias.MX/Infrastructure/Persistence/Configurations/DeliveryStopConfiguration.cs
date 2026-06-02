using API.G3Operaciones.Garcias.MX.Domain.Entities.Delivery;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace API.G3Operaciones.Garcias.MX.Infrastructure.Persistence.Configurations
{
    public class DeliveryStopConfiguration : IEntityTypeConfiguration<DeliveryStop>
    {
        public void Configure(EntityTypeBuilder<DeliveryStop> builder)
        {
            builder.ToTable("DeliveryStops");

            builder.HasKey(x => x.Id);

            builder.Property(x => x.OrderId)
                .IsRequired();

            builder.Property(x => x.ClientId)
                .IsRequired();

            builder.Property(x => x.Latitude)
                .HasPrecision(9, 6)
                .IsRequired();

            builder.Property(x => x.Longitude)
                .HasPrecision(9, 6)
                .IsRequired();

            builder.Property(x => x.Sequence)
                .IsRequired();

            builder.Property(x => x.RouteGroup)
                .IsRequired();

            builder.Property(x => x.Status)
                .IsRequired();

            builder.Property(x => x.CreatedAt)
                .IsRequired();

            // Índices clave para rendimiento
            builder.HasIndex(x => x.DeliveryPlanId);
            builder.HasIndex(x => x.OrderId);
            builder.HasIndex(x => new { x.DeliveryPlanId, x.Sequence });

            // Relación con DeliveryPlan (ya definida pero reforzada aquí)
            builder.HasOne(x => x.DeliveryPlan)
                .WithMany(x => x.Stops)
                .HasForeignKey(x => x.DeliveryPlanId)
                .OnDelete(DeleteBehavior.Cascade);
        }
    }
}
