using API.G3Operaciones.Garcias.MX.Domain.Entities.Orders;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace API.G3Operaciones.Garcias.MX.Infrastructure.Persistence.Configurations
{
    public class OrderScheduleConfigurationConfiguration : IEntityTypeConfiguration<OrderScheduleConfiguration>
    {
        public void Configure(EntityTypeBuilder<OrderScheduleConfiguration> builder)
        {
            builder.ToTable("OrderScheduleConfigurations");

            builder.HasKey(x => x.Id);

            builder.Property(x => x.OrderDay)
                .HasConversion<int>()
                .IsRequired();

            builder.Property(x => x.DeliveryDay)
                .HasConversion<int>()
                .IsRequired();

            builder.Property(x => x.AllowedHoursWindow)
                .IsRequired();

            builder.Property(x => x.IsActive)
                .IsRequired();

            builder.Property(x => x.CreatedAt)
                .IsRequired();

            builder.HasIndex(x => x.OrderDay);
            builder.HasIndex(x => x.DeliveryDay);
        }
    }
}
