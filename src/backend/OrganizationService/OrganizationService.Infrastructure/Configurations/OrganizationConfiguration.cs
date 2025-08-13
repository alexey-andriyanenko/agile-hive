using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using OrganizationService.Domain.Entities;
using OrganizationService.Domain.ValueObjects;

namespace OrganizationService.Infrastructure.Configurations;

public class OrganizationConfiguration : IEntityTypeConfiguration<Organization>
{
    public void Configure(EntityTypeBuilder<Organization> builder)
    {
        builder.HasKey(o => o.Id);
        builder.Property(o => o.Id)
            .ValueGeneratedNever();
        
        builder.Property(o => o.OwnerUserId)
            .IsRequired();
        builder.HasIndex(o => o.OwnerUserId);
        
        builder.Property(o => o.Name)
            .HasConversion(
                name => name.Value,
                value => new OrganizationName(value))
            .IsRequired()
            .HasMaxLength(16)
            .HasColumnName("name");
        
        builder.ToTable("organizations", t =>
        {
            t.HasCheckConstraint("ck_organization_name_min_length",
                "char_length(name) >= 2");
            t.HasCheckConstraint("ck_organization_no_double_spaces",
                "name !~ ' {2,}'");
            t.HasCheckConstraint("ck_organization_name_allowed_chars",
                "name ~ '^[A-Za-z0-9 \\-_.]+$'");
        }); 
    }
}
