using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using TenantContextService.Domain.Entities;
using TenantContextService.Domain.Enum;

namespace TenantContextService.Infrastructure.Configuration;

public class TenantMemberReadEntityTypeConfiguration : IEntityTypeConfiguration<TenantMemberReadEntity>
{
    public void Configure(EntityTypeBuilder<TenantMemberReadEntity> builder)
    {
        builder.HasKey(x => new { x.TenantId, x.UserId });

        builder.Property(x => x.Role)
            .IsRequired()
            .HasConversion(
                x => x.ToString(),
                x => (TenantMemberRole)Enum.Parse(typeof(TenantMemberRole), x)
            );
    }
}