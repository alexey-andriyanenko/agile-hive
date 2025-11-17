using BoardService.Domain.Constants;
using BoardService.Domain.Entities;
using BoardService.Infrastructure.Configurations;
using Microsoft.EntityFrameworkCore;

namespace BoardService.Infrastructure.Data;

public class ApplicationDbContext(DbContextOptions<ApplicationDbContext> options, TenantContext tenantContext) : DbContext(options)
{
    public DbSet<Board> Boards { get; set; }

    public DbSet<BoardColumn> BoardColumns { get; set; }

    public DbSet<BoardType> BoardTypes { get; set; }

    protected override void OnModelCreating(ModelBuilder builder)
    {
        builder.ApplyConfiguration(new BoardConfiguration());
        builder.ApplyConfiguration(new BoardColumnConfiguration());
        builder.ApplyConfiguration(new BoardTypeConfiguration());

        base.OnModelCreating(builder);
    }

    protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
    {
        if (!optionsBuilder.IsConfigured && !string.IsNullOrEmpty(tenantContext.DbConnectionString))
        {
            optionsBuilder.UseNpgsql(tenantContext.DbConnectionString);
        }
        
        optionsBuilder.UseSeeding((context, _) =>
        {
            var boardTypesContext = context.Set<BoardType>();
            var boardTypes = boardTypesContext.ToList();

            if (boardTypes.Count == 0)
            {
                boardTypes.AddRange([
                    new BoardType()
                    {
                        Id = PredefinedBoardTypes.KanbanId,
                        Name = PredefinedBoardTypes.KanbanName,
                    },
                    new BoardType()
                    {
                        Id = PredefinedBoardTypes.ScrumId,
                        Name = PredefinedBoardTypes.ScrumName,
                    },
                    new BoardType()
                    {
                        Id = PredefinedBoardTypes.BacklogId,
                        Name = PredefinedBoardTypes.BacklogName,
                    },
                    new BoardType()
                    {
                        Id = PredefinedBoardTypes.CustomId,
                        Name = PredefinedBoardTypes.CustomName,
                    }
                ]);
                
                boardTypesContext.AddRange(boardTypes);
                context.SaveChanges();
            }
        });

        optionsBuilder.UseAsyncSeeding(async (context, _, cancellationToken) =>
        {
            var boardTypesContext = context.Set<BoardType>();
            var boardTypes = await boardTypesContext.ToListAsync(cancellationToken);

            if (boardTypes.Count == 0)
            {
                boardTypes.AddRange([
                    new BoardType()
                    {
                        Id = PredefinedBoardTypes.KanbanId,
                        Name = PredefinedBoardTypes.KanbanName,
                    },
                    new BoardType()
                    {
                        Id = PredefinedBoardTypes.ScrumId,
                        Name = PredefinedBoardTypes.ScrumName,
                    },
                    new BoardType()
                    {
                        Id = PredefinedBoardTypes.BacklogId,
                        Name = PredefinedBoardTypes.BacklogName,
                    },
                    new BoardType()
                    {
                        Id = PredefinedBoardTypes.CustomId,
                        Name = PredefinedBoardTypes.CustomName,
                    }
                ]);
                
                await boardTypesContext.AddRangeAsync(boardTypes, cancellationToken);
                await context.SaveChangesAsync(cancellationToken);
            }
        });

        base.OnConfiguring(optionsBuilder);
    }
}