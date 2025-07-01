using Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Infrastructure.Persistence.Configurations;

public class CommStatusConfiguration : IEntityTypeConfiguration<CommStatus>
{
    public void Configure(EntityTypeBuilder<CommStatus> entity)
    {
        entity.HasKey(e => e.Id).HasName("PK__CommStat__3214EC079C5F650D");

        entity.ToTable("CommStatus");

        entity.Property(e => e.Description)
            .IsRequired()
            .HasMaxLength(50);
    }
}
