using Tag.Contracts;

namespace Web.API.Results.Tag;

public class GetTagsResult
{
    public IReadOnlyList<TagDto> Tags { get; set; } = [];
}
