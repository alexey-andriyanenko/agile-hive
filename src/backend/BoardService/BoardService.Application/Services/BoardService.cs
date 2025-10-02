using BoardService.Application.Mappings;
using BoardService.Contracts;
using BoardService.Domain.Constants;
using BoardService.Domain.Entities;
using BoardService.Infrastructure;
using BoardService.Infrastructure.Data;
using Google.Protobuf.WellKnownTypes;
using Grpc.Core;
using Microsoft.AspNetCore.Http;
using Microsoft.EntityFrameworkCore;

namespace BoardService.Application.Services;

public class BoardService(ApplicationDbContext dbContext, IHttpContextAccessor httpContextAccessor) : Contracts.BoardService.BoardServiceBase
{
    public override async Task<BoardDto> Create(CreateBoardRequest request, ServerCallContext context)
    {
        var userContext = (UserContext)httpContextAccessor.HttpContext!.Items["UserContext"]!;
        
        var projectId = Guid.Parse(request.ProjectId);
        var existingBoard = await dbContext.Boards
            .SingleOrDefaultAsync(b => b.Name == request.Name && b.ProjectId == projectId);

        if (existingBoard != null)
        {
            throw new RpcException(new Status(StatusCode.AlreadyExists,
                "A board with the same name already exists in the project."));
        }

        var boardTypeId = Guid.Parse(request.BoardTypeId);
        var boardType = await dbContext.BoardTypes
            .SingleOrDefaultAsync(bt => bt.Id == boardTypeId);

        if (boardType == null)
        {
            throw new RpcException(new Status(StatusCode.InvalidArgument, "The specified board type does not exist."));
        }

        var defaultColumns = GetDefaultColumns(boardType.Name);
        var defaultColumnNames = defaultColumns.Select(c => c.Name).ToHashSet(StringComparer.OrdinalIgnoreCase);
        if (request.Columns.Any(c => defaultColumnNames.Contains(c.Name)))
        {
            throw new RpcException(new Status(StatusCode.InvalidArgument,
                "One or more column names conflict with the predefined columns for the selected board type."));
        }

        var board = new Domain.Entities.Board
        {
            Id = Guid.NewGuid(),
            Name = request.Name,
            ProjectId = projectId, BoardTypeId = Guid.Parse(request.BoardTypeId),
            CreatedAt = DateTime.UtcNow,
            BoardType = boardType,
            CreatedByUserId = userContext.UserId,
            Columns = defaultColumns
                .Concat(request.Columns.Select((c, index) => new BoardColumn
                {
                    Id = Guid.NewGuid(),
                    CreatedByUserId = userContext.UserId,
                    Name = c.Name,
                    CreatedAt = DateTime.UtcNow,
                    Order = defaultColumns.Count + index + 1
                }))
                .ToList()
        };

        dbContext.Boards.Add(board);
        await dbContext.SaveChangesAsync();

        return board.ToDto();
    }

    public override async Task<BoardDto> GetById(GetBoardByIdRequest request, ServerCallContext context)
    {
        var boardId = Guid.Parse(request.BoardId);
        var projectId = Guid.Parse(request.ProjectId);
        var board = await dbContext.Boards
            .Include(b => b.BoardType)
            .Include(b => b.Columns)
            .SingleOrDefaultAsync(b => b.Id == boardId && b.ProjectId == projectId);

        if (board == null)
        {
            throw new RpcException(new Status(StatusCode.NotFound,
                $"Board with ID '{request.BoardId}' not found in the the project with ID '{request.ProjectId}'."));
        }

        return board.ToDto();
    }

    public override async Task<GetManyBoardsResponse> GetMany(GetManyBoardsRequest request, ServerCallContext context)
    {
        var projectId = Guid.Parse(request.ProjectId);
        var boards = await dbContext.Boards
            .Include(b => b.BoardType)
            .Include(b => b.Columns)
            .Where(b => b.ProjectId == projectId)
            .ToListAsync();

        return new GetManyBoardsResponse()
        {
            Boards = { boards.Select(b => b.ToDto()) }
        };
    }

    public override async Task<BoardDto> Update(UpdateBoardRequest request, ServerCallContext context)
    {
        var userContext = (UserContext)httpContextAccessor.HttpContext!.Items["UserContext"]!;

        var boardId = Guid.Parse(request.BoardId);
        var projectId = Guid.Parse(request.ProjectId);
        var board = await dbContext.Boards
            .Include(b => b.BoardType)
            .Include(b => b.Columns)
            .SingleOrDefaultAsync(b => b.Id == boardId && b.ProjectId == projectId);

        if (board == null)
        {
            throw new RpcException(new Status(StatusCode.NotFound,
                $"Board with ID '{request.BoardId}' not found in the the project with ID '{request.ProjectId}'."));
        }
        
        var boardColumnsById = board.Columns.ToDictionary(c => c.Id);
        
        var nameChanged = !string.Equals(board.Name, request.Name, StringComparison.Ordinal);

        if (nameChanged)
        {
            var existingBoard = await dbContext.Boards
                .SingleOrDefaultAsync(b => b.Name == request.Name && b.ProjectId == projectId);

            if (existingBoard != null)
            {
                throw new RpcException(new Status(StatusCode.AlreadyExists,
                    "A board with the same name already exists in the project."));
            }

            board.Name = request.Name;
        }

        var deletedColumnIds = board.Columns
            .Where(c => request.Columns.All(rc => rc.Id != c.Id.ToString()))
            .Select(c => c.Id)
            .ToList();
        var addedColumns = request.Columns
            .Where(rc => string.IsNullOrEmpty(rc.Id))
            .Select((rc, index) => new BoardColumn
            {
                Id = Guid.NewGuid(),
                Name = rc.Name,
                CreatedAt = DateTime.UtcNow,
                Order = board.Columns.Count + index + 1,
                BoardId = board.Id,
                CreatedByUserId = userContext.UserId
            })
            .ToList();
        var updatedColumns = board.Columns
            .Where(c => request.Columns.Any(rc =>
                rc.Id == c.Id.ToString() &&
                !string.Equals(rc.Name, c.Name, StringComparison.Ordinal) ||
                c.Order != request.Columns.IndexOf(rc)))
            .ToList();

        foreach (var column in updatedColumns)
        {
            var updatedColumnData = boardColumnsById[column.Id];
            column.Name = updatedColumnData.Name;
        }

        dbContext.BoardColumns.RemoveRange(board.Columns.Where(c => deletedColumnIds.Contains(c.Id)));
        dbContext.BoardColumns.AddRange(addedColumns);

        if (nameChanged || deletedColumnIds.Count != 0 || addedColumns.Count != 0 || updatedColumns.Count != 0)
        {
            board.UpdatedAt = DateTime.UtcNow;
        }

        await dbContext.SaveChangesAsync();

        return board.ToDto();
    }

    public override async Task<Empty> Delete(DeleteBoardRequest request, ServerCallContext context)
    {
        var board = await dbContext.Boards
            .SingleOrDefaultAsync(b =>
                b.Id == Guid.Parse(request.BoardId) && b.ProjectId == Guid.Parse(request.ProjectId));

        if (board == null)
        {
            throw new RpcException(new Status(StatusCode.NotFound,
                $"Board with ID '{request.BoardId}' not found in the the project with ID '{request.ProjectId}'."));
        }

        dbContext.Boards.Remove(board);
        await dbContext.SaveChangesAsync();
        return new Empty();
    }

    public override async Task<GetManyBoardTypesResponse> GetManyTypes(GetManyBoardTypesRequest request,
        ServerCallContext context)
    {
        // TODO: make board types tenant-specific in the future

        var boardTypes = await dbContext.BoardTypes
            .ToListAsync();

        return new GetManyBoardTypesResponse()
        {
            BoardTypes = { boardTypes.Select(bt => bt.ToDto()) }
        };
    }

    private static IReadOnlyCollection<BoardColumn> GetDefaultKanbanColumns()
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
                Name = "Done",
                CreatedAt = DateTime.UtcNow,
                Order = 3
            }
        };
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
                Order = 4
            }
        };
    }

    private static IReadOnlyCollection<BoardColumn> GetDefaultBacklogColumns()
    {
        return new List<BoardColumn>
        {
            new BoardColumn
            {
                Id = Guid.NewGuid(),
                Name = "Backlog",
                CreatedAt = DateTime.UtcNow,
                Order = 1
            }
        };
    }

    private static IReadOnlyCollection<BoardColumn> GetDefaultCustomColumns()
    {
        return new List<BoardColumn>();
    }

    private static IReadOnlyCollection<BoardColumn> GetDefaultColumns(string boardTypeName)
    {
        return boardTypeName switch
        {
            PredefinedBoardTypes.KanbanName => GetDefaultKanbanColumns(),
            PredefinedBoardTypes.ScrumName => GetDefaultScrumColumns(),
            PredefinedBoardTypes.BacklogName => GetDefaultBacklogColumns(),
            PredefinedBoardTypes.CustomName => GetDefaultCustomColumns(),
            _ => throw new ArgumentOutOfRangeException(nameof(boardTypeName),
                $"No default columns defined for board type '{boardTypeName}'")
        };
    }
}