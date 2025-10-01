using Microsoft.EntityFrameworkCore;
using TaskService.Domain.Entities;
using TaskService.Infrastructure.Configuration;

namespace TaskService.Infrastructure.Data;

public class ApplicationDbContext(DbContextOptions<ApplicationDbContext> options) : DbContext(options)
{
    public DbSet<TaskEntity> Tasks { get; set; }
    
    public DbSet<TagEntity> Tags { get; set; }
    
    public DbSet<TaskTagEntity> TaskTags { get; set; }
    
    public DbSet<CommentEntity> Comments { get; set; }
    

    protected override void OnModelCreating(ModelBuilder builder)
    {
        builder.ApplyConfiguration(new TaskEntityTypeConfiguration());
        builder.ApplyConfiguration(new TagEntityTypeConfiguration());
        builder.ApplyConfiguration(new TaskTagEntityTypeConfiguration());
        builder.ApplyConfiguration(new CommentEntityTypeConfiguration());

        base.OnModelCreating(builder);
    }

    protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
    {
        optionsBuilder.UseSeeding((context, _) =>
        {
            var tagsContext = context.Set<TagEntity>();
            var tags = tagsContext.ToList();

            if (tags.Count == 0)
            {
                tags.AddRange([
                    new TagEntity()
                    {
                        Id = Guid.NewGuid(),
                        Name = "Urgent",
                        Color = "#FF0000"
                    },
                    new TagEntity()
                    {
                        Id = Guid.NewGuid(),
                        Name = "High Priority",
                        Color = "#FFA500"
                    },
                    new TagEntity()
                    { Id = Guid.NewGuid(),
                        Name = "Low Priority",
                        Color = "#008000"
                    },
                    new TagEntity()
                    {
                        Id = Guid.NewGuid(),
                        Name = "Bug",
                        Color = "#0000FF"
                    },
                    new TagEntity()
                    { 
                        Id = Guid.NewGuid(),
                        Name = "Feature",
                        Color = "#800080"
                    }
                ]);
                
                tagsContext.AddRange(tags);
                context.SaveChanges();
            }
        });

        optionsBuilder.UseAsyncSeeding(async (context, _, cancellationToken) =>
        {
            var tagsContext = context.Set<TagEntity>();
            var tags = await tagsContext.ToListAsync(cancellationToken);

            if (tags.Count == 0)
            {
                tags.AddRange([
                    new TagEntity()
                    {
                        Id = Guid.NewGuid(),
                        Name = "Urgent",
                        Color = "#FF0000"
                    },
                    new TagEntity()
                    {
                        Id = Guid.NewGuid(),
                        Name = "High Priority",
                        Color = "#FFA500"
                    },
                    new TagEntity()
                    { Id = Guid.NewGuid(),
                        Name = "Low Priority",
                        Color = "#008000"
                    },
                    new TagEntity()
                    {
                        Id = Guid.NewGuid(),
                        Name = "Bug",
                        Color = "#0000FF"
                    },
                    new TagEntity()
                    { 
                        Id = Guid.NewGuid(),
                        Name = "Feature",
                        Color = "#800080"
                    }
                ]);
                
                await tagsContext.AddRangeAsync(tags, cancellationToken);
                await context.SaveChangesAsync(cancellationToken);
            }
        });

        base.OnConfiguring(optionsBuilder);
    }
}