using BoardService.Contracts;
using ProjectUserService.Contracts;
using TaskAggregatorService.Contracts;
using TaskService.Contracts;

namespace TaskAggregatorService.Application.Mappings;

public static class TaskAggregateMappings
{
    public static TaskAggregateDto ToDto(this TaskDto taskDto, BoardColumnDto boardColumnDto, ProjectUserDto creatorUserDto, ProjectUserDto? assigneeUserDto)
    {
        return new TaskAggregateDto
        {
            
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