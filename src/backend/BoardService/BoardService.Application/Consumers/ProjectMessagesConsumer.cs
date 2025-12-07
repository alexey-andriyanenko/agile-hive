using BoardMessages.Messages;
using BoardService.Domain.Constants;
using BoardService.Domain.Entities;
using BoardService.Infrastructure.Data;
using MassTransit;
using ProjectMessages.Messages;

namespace BoardService.Application.Consumers;

public class ProjectMessagesConsumer(ApplicationDbContext dbContext, IPublishEndpoint publishEndpoint) : IConsumer<ProjectCreationSucceededMessage>
{
    public async Task Consume(ConsumeContext<ProjectCreationSucceededMessage> context)
    {
        var message = context.Message;

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

        await dbContext.Boards.AddRangeAsync(boards);
        await dbContext.SaveChangesAsync();
        
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