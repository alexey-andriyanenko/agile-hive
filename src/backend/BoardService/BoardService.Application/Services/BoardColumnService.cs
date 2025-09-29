using BoardService.Application.Mappings;
using BoardService.Contracts;
using BoardService.Infrastructure;
using BoardService.Infrastructure.Data;
using Google.Protobuf.WellKnownTypes;
using Grpc.Core;
using Microsoft.EntityFrameworkCore;

namespace BoardService.Application.Services;

public class BoardColumnService(ApplicationDbContext dbContext) : Contracts.BoardColumnService.BoardColumnServiceBase
{
    public override async Task<BoardColumnDto> Create(CreateBoardColumnRequest request, ServerCallContext context)
    {
        var userContext = (UserContext)context.GetHttpContext()!.Items["UserContext"]!;

        var boardId = Guid.Parse(request.BoardId);
        var board = await dbContext.Boards.FirstOrDefaultAsync(x => x.Id == boardId);
        
        if (board == null)
        {
            throw new RpcException(new Status(StatusCode.NotFound, "Board not found."));
        }
        
        var column = new Domain.Entities.BoardColumn
        {
            Id = Guid.NewGuid(),
            BoardId = boardId,
            Name = request.Name,
            CreatedAt = DateTime.UtcNow,
            CreatedByUserId = userContext.UserId,
            Order = request.Order
        };
        
        board.Columns.Add(column);
        
        await dbContext.SaveChangesAsync();
        
        return column.ToDto();
    }
    
    public override async Task<BoardColumnDto> GetById(GetBoardColumnByIdRequest request, ServerCallContext context)
    {
        var boardId = Guid.Parse(request.BoardId);
        var board = await dbContext.Boards
            .FirstOrDefaultAsync(x => x.Id == boardId);
        
        if (board == null)
        {
            throw new RpcException(new Status(StatusCode.NotFound, $"Board with ID '{request.BoardId}' not found."));
        }
        
        var columnId = Guid.Parse(request.BoardColumnId);
        var column = await dbContext.BoardColumns
            .FirstOrDefaultAsync(x => x.Id == columnId);
        
        if (column == null)
        {
            throw new RpcException(new Status(StatusCode.NotFound, $"Board columns with ID '{request.BoardColumnId}' not found in board with ID '{request.BoardId}'."));
        }
        
        return column.ToDto();
    }
    
    public override async Task<GetManyBoardColumnsResponse> GetMany(GetManyBoardColumnsRequest request, ServerCallContext context)
    {
        var boardId = Guid.Parse(request.BoardId);
        var board = await dbContext.Boards
            .Include(x => x.Columns)
            .FirstOrDefaultAsync(x => x.Id == boardId);

        if (board == null)
        {
            throw new RpcException(new Status(StatusCode.NotFound, $"Board with ID '{request.BoardId}' not found."));
        }

        var response = new GetManyBoardColumnsResponse()
        {
            BoardColumns = { board.Columns.Select(c => c.ToDto()) }
        };
        
        return response;
    }

    public override async Task<BoardColumnDto> Update(UpdateBoardColumnRequest request, ServerCallContext context)
    {
        var boardId = Guid.Parse(request.BoardId);
        var board = await dbContext.Boards
            .FirstOrDefaultAsync(x => x.Id == boardId);
        
        if (board == null)
        {
            throw new RpcException(new Status(StatusCode.NotFound, $"Board with ID '{request.BoardId}' not found."));
        }
        
        var columnId = Guid.Parse(request.BoardColumnId);
        var column = await dbContext.BoardColumns
            .FirstOrDefaultAsync(x => x.Id == columnId);
        
        if (column == null)
        {
            throw new RpcException(new Status(StatusCode.NotFound, $"Board columns with ID '{request.BoardColumnId}' not found in board with ID '{request.BoardId}'."));
        }

        if (column.Name != request.Name)
        {
            column.Name = request.Name;
        }

        if (column.Order != request.Order)
        {
            column.Order = request.Order;
        }

        if (column.Name != request.Name || column.Order != request.Order)
        {
            column.UpdatedAt = DateTime.UtcNow;
        }
        
        await dbContext.SaveChangesAsync();
        
        return column.ToDto();
    }

    public override async Task<Empty> Delete(DeleteBoardColumnRequest request, ServerCallContext context)
    {
        var boardId = Guid.Parse(request.BoardId);
        var board = await dbContext.Boards
            .FirstOrDefaultAsync(x => x.Id == boardId);
        
        if (board == null)
        {
            throw new RpcException(new Status(StatusCode.NotFound, $"Board with ID '{request.BoardId}' not found."));
        }
        
        var columnId = Guid.Parse(request.BoardColumnId);
        var column = await dbContext.BoardColumns
            .FirstOrDefaultAsync(x => x.Id == columnId);
        
        if (column == null)
        {
            throw new RpcException(new Status(StatusCode.NotFound, $"Board columns with ID '{request.BoardColumnId}' not found in board with ID '{request.BoardId}'."));
        }
        
        dbContext.BoardColumns.Remove(column);
        
        await dbContext.SaveChangesAsync();
        
        return new Empty();
    }
}