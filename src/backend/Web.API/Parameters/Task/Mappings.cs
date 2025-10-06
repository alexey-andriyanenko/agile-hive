using TaskAggregatorService.Contracts;

namespace Web.API.Parameters.Task;

public static class Mappings
{
    public static CreateTaskRequest ToGrpcRequest(this CreateTaskParameters parameters)
    {
        return new CreateTaskRequest
        {
            TenantId = parameters.TenantId.ToString(),
            ProjectId = parameters.ProjectId.ToString(),
            BoardId = parameters.BoardId.ToString(),
            BoardColumnId = parameters.BoardColumnId.ToString(),
            Title = parameters.Title,
            Description = parameters.Description,
            AssigneeUserId = parameters.AssigneeUserId?.ToString(),
            TagIds = { parameters.TagIds.Select(id => id.ToString()) }
        };
    }

    public static UpdateTaskRequest ToGrpcRequest(this UpdateTaskParameters parameters)
    {
        return new UpdateTaskRequest
        {
            TenantId = parameters.TenantId.ToString(),
            ProjectId = parameters.ProjectId.ToString(),
            TaskId = parameters.TaskId.ToString(),
            BoardId = parameters.BoardId.ToString(),
            BoardColumnId = parameters.BoardColumnId.ToString(),
            Title = parameters.Title,
            Description = parameters.Description,
            AssigneeUserId = parameters.AssigneeUserId?.ToString(),
            TagIds = { parameters.TagIds.Select(id => id.ToString()) }
        };
    }
}