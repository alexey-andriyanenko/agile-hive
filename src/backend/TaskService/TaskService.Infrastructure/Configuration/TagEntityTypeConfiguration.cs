using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using TaskService.Domain.Entities;

namespace TaskService.Infrastructure.Configuration;

public class TagEntityTypeConfiguration : IEntityTypeConfiguration<TagEntity>
{
    public void Configure(EntityTypeBuilder<TagEntity> builder)
    {
        builder.HasKey(x => x.Id);

        builder
            .HasIndex(x => new { x.Name, x.TenantId, x.ProjectId })
            .IsUnique();

        builder.Property(x => x.Name)
            .IsRequired()
            .HasMaxLength(100);

        builder.Property(x => x.TenantId)
            .IsRequired();
        
        builder.Property(x => x.ProjectId)
            .IsRequired();
        
        builder.Property(x => x.Color)
            .HasMaxLength(7);

        builder.HasMany(x => x.TaskTags)
            .WithOne(x => x.Tag)
            .HasForeignKey(x => x.TagId)
            .OnDelete(DeleteBehavior.Cascade);
        
        builder.ToTable("Tags");
    }
}