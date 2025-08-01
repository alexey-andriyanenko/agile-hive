using OrganizationService.Domain.Entities;
using Microsoft.EntityFrameworkCore;
using OrganizationService.Infrastructure.Configurations;

namespace OrganizationService.Infrastructure.Data;

public class ApplicationDbContext : DbContext
{
    public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options) :
        base(options)
    { }
    
    public DbSet<Organization> Organizations { get; set; }

    protected override void OnModelCreating(ModelBuilder builder)
    {
        builder.ApplyConfiguration(new OrganizationConfiguration());
        
        base.OnModelCreating(builder);
    }

    protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
    {
        base.OnConfiguring(optionsBuilder);
    }
}   