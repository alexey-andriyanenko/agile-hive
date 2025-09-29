using BoardService.Contracts;

namespace Web.API.Results.Board;

public class GetManyBoardsResult
{
    public IReadOnlyList<Dtos.Board.BoardDto> Boards { get; set; } = [];
}