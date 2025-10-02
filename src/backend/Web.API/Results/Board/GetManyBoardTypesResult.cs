using Web.API.Dtos.Board;

namespace Web.API.Results.Board;

public class GetManyBoardTypesResult
{
    public IReadOnlyList<BoardTypeDto> BoardTypes { get; set; } = [];
}