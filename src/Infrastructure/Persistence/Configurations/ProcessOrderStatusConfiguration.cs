using Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Infrastructure.Persistence.Configurations;

public class ProcessOrderStatusConfiguration : IEntityTypeConfiguration<ProcessOrderStatus>
{
    public void Configure(EntityTypeBuilder<ProcessOrderStatus> entity)
    {
        entity.HasKey(e => e.Id).HasName("PK__ProcessO__3214EC07FCF7F8E4");

        entity.ToTable("ProcessOrderStatus");

        entity.Property(e => e.Description)
            .IsRequired()
            .HasMaxLength(50);
    }
}
