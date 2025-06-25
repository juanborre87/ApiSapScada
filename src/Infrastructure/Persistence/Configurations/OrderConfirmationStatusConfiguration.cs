using Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Infrastructure.Persistence.Configurations;

public class OrderConfirmationStatusConfiguration : IEntityTypeConfiguration<OrderConfirmationStatus>
{
    public void Configure(EntityTypeBuilder<OrderConfirmationStatus> entity)
    {
        entity.HasKey(e => e.StatusId);

        entity.ToTable("OrderConfirmationStatus");

        entity.Property(e => e.StatusDescription)
            .IsRequired()
            .HasMaxLength(50);
    }
}
