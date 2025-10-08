using Google.Protobuf.WellKnownTypes;
using TaskService.Contracts;
using TaskService.Domain.Entities;

namespace TaskService.Application.Mappings;

public static class TaskEntityMappings
{
    public static TaskDto ToDto(this TaskEntity task) => new TaskDto
    {
        Id = task.Id.ToString(),
        ProjectId = task.ProjectId.ToString(),
        TenantId = task.TenantId.ToString(),
        BoardId = task.BoardId.ToString(),
        BoardColumnId = task.BoardColumnId.ToString(),
        Title = task.Title,
        Description = task.Description,
        CreatedByUserId = task.CreatedByUserId.ToString(),
        AssigneeUserId = task.AssigneeUserId?.ToString(),
        CreatedAt = task.CreatedAt.ToTimestamp(),
        UpdatedAt = task.UpdatedAt?.ToTimestamp(),
        TagIds = { task.TaskTags.Select(tt => tt.TagId.ToString()) }
    };
}