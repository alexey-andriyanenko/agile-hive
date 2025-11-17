using Microsoft.EntityFrameworkCore;
using TagService.Domain.Entities;
using TagService.Infrastructure.Configuration;

namespace TagService.Infrastructure.Data;

public class ApplicationDbContext(DbContextOptions<ApplicationDbContext> options, TenantContext tenantContext) : DbContext(options)
{
    public DbSet<TagEntity> Tags { get; set; }

    protected override void OnModelCreating(ModelBuilder builder)
    {
        builder.ApplyConfiguration(new TagEntityTypeConfiguration());

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