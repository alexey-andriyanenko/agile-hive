using Microsoft.EntityFrameworkCore;
using PermissionService.Domain.Entities;
using PermissionService.Domain.Enums;

namespace PermissionService.Infrastructure.Configurations;

public class PermissionEntityTypeConfiguration : IEntityTypeConfiguration<PermissionEntity>
{
    public void Configure(Microsoft.EntityFrameworkCore.Metadata.Builders.EntityTypeBuilder<PermissionEntity> builder)
    {
        builder.ToTable("Permissions");

        builder.HasKey(p => p.Id);

        builder.Property(x => x.TenantId)
            .IsRequired();

        builder.Property(x => x.Scope)
            .IsRequired()
            .HasConversion(
                (v) => v.ToString(),
                (v) => (ScopeType)Enum.Parse(typeof(ScopeType), v)
            );
        
        builder.Property(x => x.Resource)
            .IsRequired()
            .HasConversion(
                (v) => v.ToString(),
                (v) => (ResourceType)Enum.Parse(typeof(ResourceType), v)
            );
        
        builder.Property(x => x.Operation)
            .IsRequired()
            .HasConversion(
                (v) => v.ToString(),
                (v) => (OperationType)Enum.Parse(typeof(OperationType), v)
            );

        builder.Property(x => x.Role)
            .IsRequired()
            .HasMaxLength(100);
    }
}