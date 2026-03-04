using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using API.G3Operaciones.Garcias.MX.Domain.Entities.Orders;

namespace API.G3Operaciones.Garcias.MX.Infrastructure.Persistence.Configurations
{
    public class OperatorAssignmentConfiguration : IEntityTypeConfiguration<OperatorAssignment>
    {
        public void Configure(EntityTypeBuilder<OperatorAssignment> builder)
        {
            builder.ToTable("OperatorAssignments");

            builder.HasKey(x => x.Id);

            builder.Property(x => x.OperatorId)
                .IsRequired();

            builder.Property(x => x.BranchId);

            builder.Property(x => x.ChainId);

            builder.Property(x => x.AssignedDay)
                .HasConversion<int>()
                .IsRequired();

            builder.Property(x => x.IsActive)
                .IsRequired();

            builder.Property(x => x.CreatedAt)
                .IsRequired();

            builder.HasIndex(x => x.OperatorId);
            builder.HasIndex(x => x.BranchId);
            builder.HasIndex(x => x.ChainId);
            builder.HasIndex(x => x.AssignedDay);
        }
    }
}