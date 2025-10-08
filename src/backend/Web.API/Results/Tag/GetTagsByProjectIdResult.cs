using Tag.Contracts;

namespace Web.API.Results.Tag;

public class GetTagsByProjectIdResult
{
    public IReadOnlyList<TagDto> Tags { get; set; } = [];
}
