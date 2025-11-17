using OrganizationService.Domain.Entities;
using Microsoft.EntityFrameworkCore;
using OrganizationService.Infrastructure.Configurations;

namespace OrganizationService.Infrastructure.Data;

public class ApplicationDbContext(DbContextOptions<ApplicationDbContext> options, TenantContext tenantContext)
    : DbContext(options)
{
    public DbSet<OrganizationMember> OrganizationMembers { get; set; }

    protected override void OnModelCreating(ModelBuilder builder)
    {
        builder.ApplyConfiguration(new OrganizationMemberConfiguration());
        
        base.OnModelCreating(builder);
    }

    protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
    {
        if (!optionsBuilder.IsConfigured && !string.IsNullOrEmpty(tenantContext.DbConnectionString))
        {
            optionsBuilder.UseNpgsql(tenantContext.DbConnectionString);
        }

        base.OnConfiguring(optionsBuilder);
    }
}   