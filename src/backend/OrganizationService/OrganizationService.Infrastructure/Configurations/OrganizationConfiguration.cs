using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using OrganizationService.Domain.Entities;

namespace OrganizationService.Infrastructure.Configurations;

public class OrganizationConfiguration : IEntityTypeConfiguration<Organization>
{
    public void Configure(EntityTypeBuilder<Organization> builder)
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
