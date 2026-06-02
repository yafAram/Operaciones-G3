using API.G3Operaciones.Garcias.MX.Domain.Entities.Delivery;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace API.G3Operaciones.Garcias.MX.Infrastructure.Persistence.Configurations
{
    public class DeliveryPlanConfiguration : IEntityTypeConfiguration<DeliveryPlan>
    {
        public void Configure(EntityTypeBuilder<DeliveryPlan> builder)
        {
            builder.ToTable("DeliveryPlans");

            builder.HasKey(x => x.Id);

            builder.Property(x => x.DeliveryUserId)
                .IsRequired();

            builder.Property(x => x.Date)
                .IsRequired();

            builder.Property(x => x.Status)
                .IsRequired();

            builder.Property(x => x.CreatedAt)
                .IsRequired();

            // Índice para consultas por repartidor y fecha
            builder.HasIndex(x => new { x.DeliveryUserId, x.Date });

            // Relación 1 - N con DeliveryStop
            builder.HasMany(x => x.Stops)
                .WithOne(x => x.DeliveryPlan)
                .HasForeignKey(x => x.DeliveryPlanId)
                .OnDelete(DeleteBehavior.Cascade);
        }
    }
}
