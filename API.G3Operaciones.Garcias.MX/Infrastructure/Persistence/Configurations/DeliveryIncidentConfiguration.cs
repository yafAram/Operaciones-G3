using API.G3Operaciones.Garcias.MX.Domain.Entities.Delivery;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace API.G3Operaciones.Garcias.MX.Infrastructure.Persistence.Configurations
{
    public class DeliveryIncidentConfiguration : IEntityTypeConfiguration<DeliveryIncident>
    {
        public void Configure(EntityTypeBuilder<DeliveryIncident> builder)
        {
            builder.ToTable("DeliveryIncidents");

            builder.HasKey(x => x.Id);

            builder.Property(x => x.Reason)
                .IsRequired()
                .HasMaxLength(500);

            builder.Property(x => x.CreatedAt)
                .IsRequired();

            builder.HasIndex(x => x.DeliveryStopId);

            builder.HasOne<DeliveryStop>()
                .WithMany()
                .HasForeignKey(x => x.DeliveryStopId)
                .OnDelete(DeleteBehavior.Cascade);
        }
    }
}
