namespace TaskAggregatorService.Application.Mappings;

public static class TagMappings
{
    public static Contracts.TagDto ToDto(this TaskService.Contracts.TagDto tagDto)
    {
        return new Contracts.TagDto
        {
            Id = tagDto.Id,
            Name = tagDto.Name,
            Color = tagDto.Color
        };
    }
}