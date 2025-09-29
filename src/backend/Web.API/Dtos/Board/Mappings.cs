namespace Web.API.Dtos.Board;

public static class Mappings
{
    public static BoardDto ToHttp(this BoardService.Contracts.BoardDto boardGrpcDto)
    {
        return new BoardDto()
        {
            Id = Guid.Parse(boardGrpcDto.Id),
            ProjectId = Guid.Parse(boardGrpcDto.ProjectId),
            Name = boardGrpcDto.Name,
            CreatedAt = boardGrpcDto.CreatedAt.ToDateTime(),
            UpdatedAt = boardGrpcDto.UpdatedAt.ToDateTime(),
            CreatedByUserId = Guid.Parse(boardGrpcDto.CreatedByUserId),
            Type = boardGrpcDto.BoardType.ToHttp(),
            Columns = boardGrpcDto.Columns.Select(c => c.ToHttp()).ToList()
        };
    }

    public static BoardColumnDto ToHttp(this BoardService.Contracts.BoardColumnDto boardColumnGrpcDto)
    {
        return new BoardColumnDto()
        {
            Id = Guid.Parse(boardColumnGrpcDto.Id),
            BoardId = Guid.Parse(boardColumnGrpcDto.BoardId),
            CreatedByUserId = Guid.Parse(boardColumnGrpcDto.CreatedByUserId),
            Name = boardColumnGrpcDto.Name,
            Order = boardColumnGrpcDto.Order,
            CreatedAt = boardColumnGrpcDto.CreatedAt.ToDateTime(),
            UpdatedAt = boardColumnGrpcDto.UpdatedAt.ToDateTime()
        };
    }

    public static BoardTypeDto ToHttp(this BoardService.Contracts.BoardTypeDto boardTypeGrpcDto)
    {
        return new BoardTypeDto()
        {
            Id = Guid.Parse(boardTypeGrpcDto.Id),
            Name = boardTypeGrpcDto.Name,
            IsEssential = boardTypeGrpcDto.IsEssential,
        };
    }
}