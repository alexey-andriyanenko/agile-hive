using Google.Protobuf.WellKnownTypes;
using Grpc.Core;
using Microsoft.EntityFrameworkCore;
using TaskService.Application.Mappings;
using TaskService.Contracts;
using TaskService.Infrastructure.Data;

namespace TaskService.Application.Services;

public class TagService(ApplicationDbContext dbContext) : Contracts.TagService.TagServiceBase
{
    public override async Task<GetManyTagsByProjectIdResponse> GetManyByProjectId(GetManyTagsByProjectIdRequest request, ServerCallContext context)
    {
        var tags = await dbContext.Tags
            .Where(t => t.ProjectId == Guid.Parse(request.ProjectId))
            .ToListAsync();

        return new GetManyTagsByProjectIdResponse()
        {
            Tags = { tags.Select(x => x.ToDto()) }
        };
    }

    public override async Task<TagDto> Create(CreateTagRequest request, ServerCallContext context)
    {
        var newTag = new Domain.Entities.TagEntity()
        {
            Id = Guid.NewGuid(),
            Name = request.Name,
            Color = request.Color,
            ProjectId = Guid.Parse(request.ProjectId)
        };

        dbContext.Tags.Add(newTag);
        await dbContext.SaveChangesAsync(context.CancellationToken);

        return newTag.ToDto();
    }
    
    public override async Task<TagDto> Update(UpdateTagRequest request, ServerCallContext context)
    {
        var tagId = Guid.Parse(request.TagId);
        var tag = await dbContext.Tags.FirstOrDefaultAsync(t => t.Id == tagId, context.CancellationToken);

        if (tag == null)
        {
            throw new RpcException(new Status(StatusCode.NotFound, $"Tag with ID {tagId} not found."));
        }

        tag.Name = request.Name;
        tag.Color = request.Color;

        dbContext.Tags.Update(tag);
        await dbContext.SaveChangesAsync(context.CancellationToken);

        return tag.ToDto();
    }
    
    public override async Task<Empty> Delete(DeleteTagRequest request, ServerCallContext context)
    {
        var tagId = Guid.Parse(request.TagId);
        var tag = await dbContext.Tags.FirstOrDefaultAsync(t => t.Id == tagId, context.CancellationToken);

        if (tag == null)
        {
            throw new RpcException(new Status(StatusCode.NotFound, $"Tag with ID {tagId} not found."));
        }

        dbContext.Tags.Remove(tag);
        await dbContext.SaveChangesAsync(context.CancellationToken);

        return new Empty();
    }
}
