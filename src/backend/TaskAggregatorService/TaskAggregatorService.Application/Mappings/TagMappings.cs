using Tag.Contracts;
using TaskAggregatorService.Contracts;

namespace TaskAggregatorService.Application.Mappings;

public static class TagMappings
{
    public static TaskTagDto ToDto(this TagDto tagDto)
    {
        return new TaskTagDto
        {
            Id = tagDto.Id,
            Name = tagDto.Name,
            Color = tagDto.Color
        };
    }
}