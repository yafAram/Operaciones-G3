using API.G3Operaciones.Garcias.MX.Domain.Entities.Orders;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace API.G3Operaciones.Garcias.MX.Infrastructure.Persistence.Configurations
{
    public class OrderCallLogConfiguration : IEntityTypeConfiguration<OrderCallLog>
    {
        public void Configure(EntityTypeBuilder<OrderCallLog> builder)
        {
            builder.ToTable("OrderCallLogs");

            builder.HasKey(x => x.Id);

            builder.Property(x => x.OperatorId)
                .IsRequired();

            builder.Property(x => x.BranchId);

            builder.Property(x => x.ChainId);

            builder.Property(x => x.ResultType)
                .HasConversion<int>()
                .IsRequired();

            builder.Property(x => x.CallDate)
                .IsRequired();

            builder.Property(x => x.ScheduledDate);

            builder.HasIndex(x => x.OperatorId);
            builder.HasIndex(x => x.BranchId);
            builder.HasIndex(x => x.ChainId);
            builder.HasIndex(x => x.ResultType);
        }
    }
}
