using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using TenantContextService.Domain.Entities;
using TenantContextService.Domain.Enum;

namespace TenantContextService.Infrastructure.Configuration;

public class TenantEntityTypeConfiguration : IEntityTypeConfiguration<TenantEntity>
{
    public void Configure(EntityTypeBuilder<TenantEntity> builder)
    {
        builder.HasKey(o => o.Id);
        builder.Property(o => o.Id)
            .ValueGeneratedNever();

        builder.Property(o => o.Name)
            .IsRequired()
            .HasMaxLength(100)
            .HasColumnName("name");

        builder.HasIndex(o => o.Name)
            .IsUnique();

        builder.HasIndex(o => o.Slug)
            .IsUnique();
    }
}