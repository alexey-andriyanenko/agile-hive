using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using TenantContextService.Domain.Entities;

namespace TenantContextService.Infrastructure.Configuration;

public class TenantDbEntityTypeConfiguration : IEntityTypeConfiguration<TenantDbEntity>
{
    public void Configure(EntityTypeBuilder<TenantDbEntity> builder)
    {
        builder.HasKey(x => x.Id);

        builder.HasIndex(x => x.TenantId);

        builder.Property(x => x.ServiceName)
            .IsRequired()
            .HasMaxLength(200);
        
        builder.Property(x => x.ConnectionString)
            .IsRequired()
            .HasMaxLength(2000);
        
        builder.Property(x => x.Active)
            .IsRequired();
        
        builder.ToTable("TenantDbs");
    }
}