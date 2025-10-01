using Google.Protobuf.WellKnownTypes;
using TaskService.Contracts;

namespace TaskService.Application.Mappings;

public static class CommentEntityMappings
{
    public static TaskCommentDto ToDto(this Domain.Entities.CommentEntity commentEntity) => new()
    {
        Id = commentEntity.Id.ToString(),
        TaskId = commentEntity.TaskId.ToString(),
        Content = commentEntity.Content,
        CreatedByUserId = commentEntity.CreatedByUserId.ToString(),
        CreatedAt = commentEntity.CreatedAt.ToTimestamp(),
        UpdatedAt = commentEntity.UpdatedAt?.ToTimestamp() ?? new Timestamp()
    };
}