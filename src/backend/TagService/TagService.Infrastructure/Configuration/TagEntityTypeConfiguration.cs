using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using TagService.Domain.Entities;

namespace TagService.Infrastructure.Configuration;

public class TagEntityTypeConfiguration : IEntityTypeConfiguration<TagEntity>
{
    public void Configure(EntityTypeBuilder<TagEntity> builder)
    {
        builder.HasKey(x => new { x.Id, x.TenantId, x.Name, x.ProjectId });

        builder.HasIndex(x => new { x.Id, x.TenantId, x.Name })
            .IsUnique()
            .HasFilter("\"ProjectId\" IS NULL");

        builder.HasIndex(x => new { x.Id, x.TenantId, x.ProjectId, x.Name })
            .IsUnique()
            .HasFilter("\"ProjectId\" IS NOT NULL");

        builder.Property(x => x.Name)
            .IsRequired()
            .HasMaxLength(100);

        builder.Property(x => x.TenantId)
            .IsRequired();
        
        builder.Property(x => x.Color)
            .HasMaxLength(7)
            .IsRequired();

        builder.Property(x => x.CreatedAt)
            .IsRequired()
            .ValueGeneratedOnAdd()
            .HasDefaultValueSql("CURRENT_TIMESTAMP");


        builder.Property(x => x.UpdatedAt)
            .ValueGeneratedOnUpdate()
            .HasDefaultValueSql("CURRENT_TIMESTAMP");
        
        builder.ToTable("Tags");
    }
}