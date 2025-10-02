namespace Web.API.Parameters.Board;

public static class Mappings
{
    public static BoardService.Contracts.CreateBoardRequest ToGrpc (this CreateBoardParameters parameters)
    {
        return new BoardService.Contracts.CreateBoardRequest()
        {
            ProjectId = parameters.ProjectId.ToString(),
            Name = parameters.Name,
            BoardTypeId = parameters.BoardTypeId.ToString(),
            Columns =
            {
                parameters.Columns.Select(x => new BoardService.Contracts.CreateBoardColumnItemRequest()
                {
                    Name = x.Name,
                })
            }
        };
    }
    
    public static BoardService.Contracts.UpdateBoardRequest ToGrpc (this UpdateBoardParameters parameters)
    {
        return new BoardService.Contracts.UpdateBoardRequest()
        {
            BoardId = parameters.BoardId.ToString(),
            ProjectId = parameters.ProjectId.ToString(),
            Name = parameters.Name,
            Columns =
            {
                parameters.Columns.Select(x => new BoardService.Contracts.CreateOrUpdateBoardColumnItemRequest()
                {
                    Id = x.Id?.ToString() ?? string.Empty,
                    Name = x.Name,
                })
            }
        };
    }
}