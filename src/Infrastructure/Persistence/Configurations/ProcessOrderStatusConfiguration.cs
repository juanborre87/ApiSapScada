using Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Infrastructure.Persistence.Configurations;

public class ProcessOrderStatusConfiguration : IEntityTypeConfiguration<ProcessOrderStatus>
{
    public void Configure(EntityTypeBuilder<ProcessOrderStatus> entity)
    {
        entity.HasKey(e => e.StatusId);

        entity.ToTable("ProcessOrderStatus");

        entity.Property(e => e.StatusDescription)
            .IsRequired()
            .HasMaxLength(50);
    }
}
