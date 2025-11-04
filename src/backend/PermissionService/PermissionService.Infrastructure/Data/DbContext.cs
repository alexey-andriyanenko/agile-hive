using Microsoft.EntityFrameworkCore;
using PermissionService.Domain.Entities;
using PermissionService.Infrastructure.Configurations;

namespace PermissionService.Infrastructure.Data;

public class ApplicationDbContext(DbContextOptions<ApplicationDbContext> options) : DbContext(options)
{
    public DbSet<PermissionEntity> Permissions { get; set; }
    
    protected override void OnModelCreating(ModelBuilder builder)
    {
        builder.ApplyConfiguration(new PermissionEntityTypeConfiguration());
        
        base.OnModelCreating(builder);
    }
}