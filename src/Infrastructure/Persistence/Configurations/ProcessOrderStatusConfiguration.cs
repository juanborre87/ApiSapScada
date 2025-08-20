using Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Infrastructure.Persistence.Configurations;

public class ProcessOrderStatusConfiguration : IEntityTypeConfiguration<ProcessOrderStatus>
{
    public void Configure(EntityTypeBuilder<ProcessOrderStatus> entity)
    {
        entity.HasKey(e => e.Id).HasName("PK__ProcessO__3214EC07F93D358E");

        entity.ToTable("ProcessOrderStatus");

        entity.Property(e => e.Description)
            .IsRequired()
            .HasMaxLength(50);
    }
}
