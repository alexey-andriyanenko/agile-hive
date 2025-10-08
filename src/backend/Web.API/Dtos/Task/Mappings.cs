namespace Web.API.Dtos.Task;

public static class Mappings
{
    public static TaskDto ToHttp(this TaskAggregatorService.Contracts.TaskAggregateDto dto)
    {
        return new TaskDto
        {
            Id = Guid.Parse(dto.Id),
            Title = dto.Title,
            description = dto.Description,
            TenantId = Guid.Parse(dto.TenantId),
            ProjectId = Guid.Parse(dto.ProjectId),
            BoardId = Guid.Parse(dto.BoardId),
            BoardColumn = dto.BoardColumn.ToHttp(),
            CreatedBy = dto.CreatorUser.ToHttp(),
            AssignedTo = dto.AssigneeUser?.ToHttp(),
            CreatedAt = dto.CreatedAt.ToDateTime(),
            UpdatedAt = dto.UpdatedAt?.ToDateTime(),
            Tags = dto.Tags.Select(t => t.ToHttp()).ToList()
        };
    }
    
    public static TaskUserDto ToHttp(this TaskAggregatorService.Contracts.TaskAggregateUserDto dto)
    {
        return new TaskUserDto
        {
            Id = Guid.Parse(dto.Id),
            FullName = dto.FullName,
            Email = dto.Email
        };
    }
    
    public static TaskBoardColumnDto ToHttp(this TaskAggregatorService.Contracts.TaskAggregateBoardColumnDto dto)
    {
        return new TaskBoardColumnDto
        {
            Id = Guid.Parse(dto.Id),
            Name = dto.Name
        };
    }
    
    public static TaskTagDto ToHttp(this TaskAggregatorService.Contracts.TaskTagDto dto)
    {
        return new TaskTagDto
        {
            Id = Guid.Parse(dto.Id),
            Name = dto.Name,
            Color = dto.Color
        };
    }
}