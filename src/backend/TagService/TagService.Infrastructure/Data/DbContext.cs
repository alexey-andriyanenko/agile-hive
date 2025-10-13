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
}