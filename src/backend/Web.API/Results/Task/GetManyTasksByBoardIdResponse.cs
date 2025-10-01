using Web.API.Dtos.Task;

namespace Web.API.Results.Task;

public class GetManyTasksByBoardIdResponse
{
    public IReadOnlyList<TaskDto> Tasks { get; set; } = [];
}