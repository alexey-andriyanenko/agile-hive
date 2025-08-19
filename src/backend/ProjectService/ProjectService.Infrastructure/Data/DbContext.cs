using Microsoft.EntityFrameworkCore;
using ProjectService.Domain.Entities;
using ProjectService.Domain.Enums;
using ProjectService.Infrastructure.Configurations;

namespace ProjectService.Infrastructure.Data;

public class ApplicationDbContext(DbContextOptions<ApplicationDbContext> options) : DbContext(options)
{
    public DbSet<Project> Projects { get; set; }
    
    public DbSet<ProjectMember> ProjectMembers { get; set; }

    protected override void OnModelCreating(ModelBuilder builder)
    {
        builder.ApplyConfiguration(new ProjectConfiguration());
        builder.ApplyConfiguration(new ProjectMemberConfiguration());
        
        base.OnModelCreating(builder);
    }
}