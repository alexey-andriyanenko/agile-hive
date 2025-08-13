using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using OrganizationService.Domain.Entities;

namespace OrganizationService.Infrastructure.Configurations;

public class OrganizationUserConfiguration : IEntityTypeConfiguration<OrganizationUser>
{
    public void Configure(EntityTypeBuilder<OrganizationUser> builder)
    {
        builder.HasKey(x => new { x.OrganizationId, x.UserId });

        builder.HasIndex(x => x.OrganizationId);
        builder.HasIndex(x => x.UserId);
    }
}
