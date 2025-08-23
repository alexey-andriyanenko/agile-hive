using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using OrganizationService.Domain.Entities;
using OrganizationService.Domain.Enums;

namespace OrganizationService.Infrastructure.Configurations;

public class OrganizationMemberConfiguration : IEntityTypeConfiguration<OrganizationMember>
{
    public void Configure(EntityTypeBuilder<OrganizationMember> builder)
    {
        builder.HasKey(x => new { x.OrganizationId, x.UserId });

        builder.Property(x => x.Role)
            .IsRequired()
            .HasConversion(
                x => x.ToString(),
                x => (OrganizationMemberRole)Enum.Parse(typeof(OrganizationMemberRole), x)
            );
    }
}