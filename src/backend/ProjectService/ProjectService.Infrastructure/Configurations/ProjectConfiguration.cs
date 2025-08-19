using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using ProjectService.Domain.Entities;
using ProjectService.Domain.Enums;

namespace ProjectService.Infrastructure.Configurations;

public class ProjectConfiguration : IEntityTypeConfiguration<Project>
{
    public void Configure(EntityTypeBuilder<Project> builder)
    {
        builder.HasKey(x => x.Id);

        builder.Property(x => x.Name)
            .IsRequired()
            .HasMaxLength(100);
        builder.HasIndex(x => new { x.OrganizationId, x.Name })
            .IsUnique();

        builder.Property(x => x.Description)
            .IsRequired()
            .HasMaxLength(500);

        builder.Property(x => x.Slug)
            .IsRequired()
            .HasMaxLength(200);
        builder.HasIndex( x => new { x.OrganizationId, x.Slug })
            .IsUnique();

        builder.Property(x => x.CreateByUserId)
            .IsRequired();

        builder.Property(x => x.Visibility)
            .IsRequired();
        builder.Property(x => x.Visibility)
            .IsRequired()
            .HasConversion(
                v => v.ToString(),
                v => (ProjectVisibility)Enum.Parse(typeof(ProjectVisibility), v));

        builder.Property(x => x.OrganizationId)
            .IsRequired();

        builder.HasMany(x => x.Members)
            .WithOne()
            .HasForeignKey(x => x.ProjectId)
            .OnDelete(DeleteBehavior.Cascade);

        builder.Property(x => x.CreatedAt)
            .IsRequired();

        builder.Property(x => x.UpdatedAt);
    }
}