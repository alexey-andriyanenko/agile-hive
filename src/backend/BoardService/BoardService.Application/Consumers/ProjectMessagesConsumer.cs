using BoardMessages.Messages;
using BoardService.Domain.Constants;
using BoardService.Domain.Entities;
using BoardService.Infrastructure;
using BoardService.Infrastructure.Data;
using MassTransit;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Caching.Memory;
using ProjectMessages.Messages;
using TenantContextService.Contracts;

namespace BoardService.Application.Consumers;

public class ProjectMessagesConsumer(TenantContextService.Contracts.TenantContextService.TenantContextServiceClient tenantContextServiceClient, IPublishEndpoint publishEndpoint, IMemoryCache memoryCache) : IConsumer<ProjectCreationSucceededMessage>
{
    private const string ServiceNameConst = "BoardService";
    
    public async Task Consume(ConsumeContext<ProjectCreationSucceededMessage> context)
    {
        var message = context.Message;
        
        var cacheKey = $"tenantcontext:{message.OrganizationId}:{ServiceNameConst}";
        
        var tenantContextResult = await memoryCache.GetOrCreateAsync(cacheKey, async entry =>
        {
            entry.AbsoluteExpirationRelativeToNow = TimeSpan.FromDays(1);
        
            var resp = await tenantContextServiceClient.GetTenantContextAsync(new GetTenantContextRequest
            {
                TenantId = message.OrganizationId.ToString(),
                ServiceName = ServiceNameConst
            }).ResponseAsync;

            return new TenantContext()
            {
                TenantId = context.Message.OrganizationId,
                DbConnectionString = resp.DbConnectionString,
                ServiceName = ServiceNameConst
            };
        });
        
        var optionsBuilder = new DbContextOptionsBuilder<ApplicationDbContext>();
        optionsBuilder.UseNpgsql(tenantContextResult.DbConnectionString);
        
        var tenantContext = new TenantContext()
        {
            TenantId = context.Message.OrganizationId,
            ServiceName = "BoardService",
            DbConnectionString = tenantContextResult.DbConnectionString,
        };
        
        await using var db = new ApplicationDbContext(optionsBuilder.Options, tenantContext);


        var boards = new List<Board>();
        
        for(var i = 1; i <= 1000; i++)
        {
            var board = new Board
            {
                Id = Guid.NewGuid(),
                ProjectId = message.ProjectId,
                Name = $"Board {i} for Project {message.ProjectId}",
                BoardTypeId = PredefinedBoardTypes.ScrumId,
                CreatedAt = DateTime.UtcNow,
                Columns = GetDefaultScrumColumns().ToList(),
            };
            
            boards.Add(board);
        }

        await db.Boards.AddRangeAsync(boards);
        await db.SaveChangesAsync();
        
        foreach (var board in boards)
        {
            await publishEndpoint.Publish(new BoardCreatedMessage()
            {
                OrganizationId = message.OrganizationId,
                ProjectId = message.ProjectId,
                CreatedByUserId = message.CreatedByUserId,
                BoardId = board.Id,
                BoardColumnIds = board.Columns.Select(c => c.Id).ToList()
            });
        }
    }
    
    private static IReadOnlyCollection<BoardColumn> GetDefaultScrumColumns()
    {
        return new List<BoardColumn>
        {
            new BoardColumn
            {
                Id = Guid.NewGuid(),
                Name = "To Do",
                CreatedAt = DateTime.UtcNow,
                Order = 1
            },
            new BoardColumn
            {
                Id = Guid.NewGuid(),
                Name = "In Progress",
                CreatedAt = DateTime.UtcNow,
                Order = 2
            },
            new BoardColumn
            {
                Id = Guid.NewGuid(),
                Name = "In Review",
                CreatedAt = DateTime.UtcNow,
                Order = 3
            },
            new BoardColumn()
            {
                Id = Guid.NewGuid(),
                Name = "In Testing",
                CreatedAt = DateTime.UtcNow,
                Order = 4
            },
            new BoardColumn
            {
                Id = Guid.NewGuid(),
                Name = "Done",
                CreatedAt = DateTime.UtcNow,
                Order = 5
            }
        };
    }
}