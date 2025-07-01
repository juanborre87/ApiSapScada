using Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Infrastructure.Persistence.Configurations;

public class CommStatusConfiguration : IEntityTypeConfiguration<CommStatus>
{
    public void Configure(EntityTypeBuilder<CommStatus> entity)
    {
        entity.HasKey(e => e.StatusId);

        entity.ToTable("CommStatus");

        entity.Property(e => e.StatusDescription)
            .IsRequired()
            .HasMaxLength(50);
    }
}
