using Tag.Contracts;
using TagService.Domain.Entities;

namespace TagService.Application.Mappings;

public static class TagEntityMappings
{
    public static TagDto ToDto(this TagEntity tag) => new TagDto
    {
        Id = tag.Id.ToString(),
        Name = tag.Name,
        Color = tag.Color
    };
}