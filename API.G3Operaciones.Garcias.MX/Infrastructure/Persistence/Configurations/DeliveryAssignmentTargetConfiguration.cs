using API.G3Operaciones.Garcias.MX.Domain.Entities.Delivery;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace API.G3Operaciones.Garcias.MX.Infrastructure.Persistence.Configurations.Delivery
{
    public class DeliveryAssignmentTargetConfiguration : IEntityTypeConfiguration<DeliveryAssignmentTarget>
    {
        public void Configure(EntityTypeBuilder<DeliveryAssignmentTarget> builder)
        {
            builder.ToTable("DeliveryAssignmentTargets");

            builder.HasKey(x => x.Id);

            builder.Property(x => x.DeliveryAssignmentPlanId)
                .IsRequired();

            builder.Property(x => x.ChainId)
                .IsRequired(false);

            builder.Property(x => x.ClientId)
                .IsRequired(false);

            builder.Property(x => x.IsActive)
                .IsRequired()
                .HasDefaultValue(true);

            builder.Property(x => x.CreatedAt)
                .IsRequired();

            // 🔥 SOLO UNA RELACIÓN (NO DUPLICAR)
            builder.HasOne(x => x.DeliveryAssignmentPlan)
                .WithMany(x => x.Targets)
                .HasForeignKey(x => x.DeliveryAssignmentPlanId)
                .OnDelete(DeleteBehavior.Restrict);

            builder.HasCheckConstraint(
                "CK_Target_ChainOrClient",
                "[ChainId] IS NOT NULL OR [ClientId] IS NOT NULL"
            );

            builder.HasIndex(x => x.DeliveryAssignmentPlanId);
            builder.HasIndex(x => x.ChainId);
            builder.HasIndex(x => x.ClientId);
        }
    }
}