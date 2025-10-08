using Microsoft.EntityFrameworkCore;
using TagService.Domain.Entities;
using TagService.Infrastructure.Configuration;

namespace TagService.Infrastructure.Data;

public class ApplicationDbContext(DbContextOptions<ApplicationDbContext> options) : DbContext(options)
{
    public DbSet<TagEntity> Tags { get; set; }
    

    protected override void OnModelCreating(ModelBuilder builder)
    {
        builder.ApplyConfiguration(new TagEntityTypeConfiguration());

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
                        Color = "#FF0000",
                        CreatedAt = DateTime.UtcNow
                    },
                    new TagEntity()
                    {
                        Id = Guid.NewGuid(),
                        Name = "High Priority",
                        Color = "#FFA500",
                        CreatedAt = DateTime.UtcNow
                    },
                    new TagEntity()
                    {
                        Id = Guid.NewGuid(),
                        Name = "Low Priority",
                        Color = "#008000",
                        CreatedAt = DateTime.UtcNow
                    },
                    new TagEntity()
                    {
                        Id = Guid.NewGuid(),
                        Name = "Bug",
                        Color = "#0000FF",
                        CreatedAt = DateTime.UtcNow
                    },
                    new TagEntity()
                    {
                        Id = Guid.NewGuid(),
                        Name = "Feature",
                        Color = "#800080",
                        CreatedAt = DateTime.UtcNow
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
                        Color = "#FF0000",
                        CreatedAt = DateTime.UtcNow
                    },
                    new TagEntity()
                    {
                        Id = Guid.NewGuid(),
                        Name = "High Priority",
                        Color = "#FFA500",
                        CreatedAt = DateTime.UtcNow
                    },
                    new TagEntity()
                    {
                        Id = Guid.NewGuid(),
                        Name = "Low Priority",
                        Color = "#008000",
                        CreatedAt = DateTime.UtcNow
                    },
                    new TagEntity()
                    {
                        Id = Guid.NewGuid(),
                        Name = "Bug",
                        Color = "#0000FF",
                        CreatedAt = DateTime.UtcNow
                    },
                    new TagEntity()
                    {
                        Id = Guid.NewGuid(),
                        Name = "Feature",
                        Color = "#800080",
                        CreatedAt = DateTime.UtcNow
                    }
                ]);
                
                await tagsContext.AddRangeAsync(tags, cancellationToken);
                await context.SaveChangesAsync(cancellationToken);
            }
        });

        base.OnConfiguring(optionsBuilder);
    }
}