using TaskService.Contracts;

namespace TaskService.Application.Mappings;

public static class TagEntityMappings
{
    public static TagDto ToDto(this Domain.Entities.TagEntity tag) => new TagDto
    {
        Id = tag.Id.ToString(),
        Name = tag.Name,
        Color = tag.Color
    };
}