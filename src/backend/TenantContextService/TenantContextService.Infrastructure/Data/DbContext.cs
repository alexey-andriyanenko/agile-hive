using Microsoft.EntityFrameworkCore;
using TenantContextService.Domain.Entities;
using TenantContextService.Infrastructure.Configuration;

namespace TenantContextService.Infrastructure.Data;

public class ApplicationDbContext(DbContextOptions<ApplicationDbContext> options) : DbContext(options)
{
    public DbSet<TenantEntity> Tenants { get; set; }
    
    public DbSet<TenantMemberReadEntity> TenantMembers { get; set; }
    
    public DbSet<TenantDbEntity> TenantDbs { get; set; }

    protected override void OnModelCreating(ModelBuilder builder)
    {
        builder.ApplyConfiguration(new TenantEntityTypeConfiguration());
        builder.ApplyConfiguration(new TenantMemberReadEntityTypeConfiguration());
        builder.ApplyConfiguration(new TenantDbEntityTypeConfiguration());

        base.OnModelCreating(builder);
    }
}