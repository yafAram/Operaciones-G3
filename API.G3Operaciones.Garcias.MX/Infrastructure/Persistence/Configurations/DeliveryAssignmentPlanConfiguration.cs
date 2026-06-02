using API.G3Operaciones.Garcias.MX.Domain.Entities.Delivery;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace API.G3Operaciones.Garcias.MX.Infrastructure.Persistence.Configurations.Delivery
{
    public class DeliveryAssignmentPlanConfiguration : IEntityTypeConfiguration<DeliveryAssignmentPlan>
    {
        public void Configure(EntityTypeBuilder<DeliveryAssignmentPlan> builder)
        {
            builder.ToTable("DeliveryAssignmentPlans");

            builder.HasKey(x => x.Id);

            builder.Property(x => x.DeliveryUserId)
                .IsRequired();

            builder.Property(x => x.DeliveryDate)
                .IsRequired();

            builder.Property(x => x.IsLocked)
                .HasDefaultValue(false);

            builder.Property(x => x.CreatedAt)
                .IsRequired();

            // 🔹 RELACIÓN 1:N con Targets
            builder.HasMany(x => x.Targets)
    .WithOne(x => x.DeliveryAssignmentPlan) // 🔥 clave
    .HasForeignKey(x => x.DeliveryAssignmentPlanId)
    .OnDelete(DeleteBehavior.Cascade);

            // 🔹 ÍNDICE ÚNICO (1 plan por repartidor por día)
            builder.HasIndex(x => new { x.DeliveryUserId, x.DeliveryDate })
                .IsUnique();
        }
    }
}