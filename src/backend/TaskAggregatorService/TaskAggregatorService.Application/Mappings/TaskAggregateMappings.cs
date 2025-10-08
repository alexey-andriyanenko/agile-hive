using BoardService.Contracts;
using ProjectUserService.Contracts;
using Tag.Contracts;
using TaskAggregatorService.Contracts;
using TaskService.Contracts;

namespace TaskAggregatorService.Application.Mappings;

public static class TaskAggregateMappings
{
    public static TaskAggregateDto ToDto(this TaskDto taskDto, BoardColumnDto boardColumnDto, ProjectUserDto creatorUserDto, IReadOnlyList<TagDto> tags, ProjectUserDto? assigneeUserDto)
    {
        return new TaskAggregateDto
        {
            Id = taskDto.Id,
            TenantId = taskDto.TenantId,
            ProjectId = taskDto.ProjectId,
            BoardId = taskDto.BoardId,
            BoardColumn = boardColumnDto.ToDto(),
            Title = taskDto.Title,
            Description = taskDto.Description,
            CreatorUser = creatorUserDto.ToDto(),
            AssigneeUser = assigneeUserDto?.ToDto(),
            CreatedAt = taskDto.CreatedAt,
            UpdatedAt = taskDto.UpdatedAt,
            Tags =
            {
                tags.Select(x => x.ToDto())
            }
        };
    }
    
    public static TaskAggregateUserDto ToDto(this ProjectUserDto projectUserDto)
    {
        return new TaskAggregateUserDto
        {
            Id = projectUserDto.Id,
            Email = projectUserDto.Email,
            FullName = $"{projectUserDto.FirstName} {projectUserDto.LastName}"
        };
    }

    public static TaskAggregateBoardColumnDto ToDto(this BoardColumnDto boardColumnDto)
    {
        return new TaskAggregateBoardColumnDto()
        {
            Id = boardColumnDto.Id,
            Name = boardColumnDto.Name,
        };
    }
}