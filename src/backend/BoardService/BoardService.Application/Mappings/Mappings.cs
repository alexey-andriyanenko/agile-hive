using BoardService.Contracts;
using BoardService.Domain.Entities;
using Google.Protobuf.WellKnownTypes;

namespace BoardService.Application.Mappings;

public static class Mappings
{
    public static BoardDto ToDto(this Board board) => new()
    {
        Id = board.Id.ToString(),
        Name = board.Name,
        ProjectId = board.ProjectId.ToString(),
        CreatedByUserId = board.CreatedByUserId.ToString(),
        BoardType = board.BoardType!.ToDto(),
        Columns = { board.Columns.Select(c => c.ToDto()) },
        CreatedAt = board.CreatedAt.ToTimestamp(),
        UpdatedAt = board.UpdatedAt?.ToTimestamp()
    };
    
    public static BoardColumnDto ToDto(this BoardColumn column) => new()
    {
        Id = column.Id.ToString(),
        Name = column.Name,
        BoardId = column.BoardId.ToString(),
        Order = column.Order,
        CreatedAt = column.CreatedAt.ToTimestamp(),
        UpdatedAt = column.UpdatedAt?.ToTimestamp()
    };
    
    public static BoardTypeDto ToDto(this BoardType boardType) => new()
    {
        Id = boardType.Id.ToString(),
        Name = boardType.Name,
        IsEssential = boardType.IsEssential,
    };
}