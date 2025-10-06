using TaskAggregatorService.Contracts;

namespace Web.API.Results.Task;

public class GetTagsByProjectIdResponse
{
    public IReadOnlyList<TagDto> Tags { get; set; } = [];
}
