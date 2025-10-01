using Google.Protobuf.WellKnownTypes;
using TaskService.Contracts;
using TaskService.Domain.Entities;

namespace TaskService.Application.Mappings;

public static class TaskEntityMappings
{
    public static TaskDto ToDto(this TaskEntity task) => new TaskDto
    {
        Id = task.Id.ToString(),
        Title = task.Title,
        Description = task.Description,
        BoardId = task.BoardId.ToString(),
        BoardColumnId = task.BoardColumnId.ToString(),
        CreatedByUserId = task.CreatedByUserId.ToString(),
        AssigneeUserId = task.AssigneeUserId?.ToString(),
        CreatedAt = task.CreatedAt.ToTimestamp(),
        UpdatedAt = task.UpdatedAt?.ToTimestamp()
    };
}