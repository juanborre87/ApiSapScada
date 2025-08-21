using Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Infrastructure.Persistence.Configurations;

public class CommStatusConfiguration : IEntityTypeConfiguration<CommStatus>
{
    public void Configure(EntityTypeBuilder<CommStatus> entity)
    {
        entity.HasKey(e => e.Id).HasName("PK__CommStat__3214EC0739C7AD28");

        entity.ToTable("CommStatus");

        entity.Property(e => e.Description)
            .IsRequired()
            .HasMaxLength(50);
    }
}
