using Microsoft.EntityFrameworkCore;
using TaskService.Domain.Entities;
using TaskService.Infrastructure.Configuration;

namespace TaskService.Infrastructure.Data;

public class ApplicationDbContext(DbContextOptions<ApplicationDbContext> options) : DbContext(options)
{
    public DbSet<TaskEntity> Tasks { get; set; }
    
    public DbSet<TaskTagEntity> TaskTags { get; set; }
    
    public DbSet<CommentEntity> Comments { get; set; }
    

    protected override void OnModelCreating(ModelBuilder builder)
    {
        builder.ApplyConfiguration(new TaskEntityTypeConfiguration());
        builder.ApplyConfiguration(new TaskTagEntityTypeConfiguration());
        builder.ApplyConfiguration(new CommentEntityTypeConfiguration());

        base.OnModelCreating(builder);
    }
}