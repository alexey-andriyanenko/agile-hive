using Google.Protobuf.WellKnownTypes;
using Grpc.Core;
using Microsoft.EntityFrameworkCore;
using Tag.Contracts;
using TagService.Application.Mappings;
using TagService.Infrastructure.Data;

namespace TagService.Application.Services;

public class TagService(ApplicationDbContext dbContext) : Tag.Contracts.TagService.TagServiceBase
{
    public override async Task<GetManyTagsByProjectIdResponse> GetManyByProjectId(GetManyTagsByProjectIdRequest request, ServerCallContext context)
    {
        var tags = await dbContext.Tags
            .Where(t => t.ProjectId == Guid.Parse(request.ProjectId) && t.TenantId == Guid.Parse(request.TenantId))
            .ToListAsync();

        return new GetManyTagsByProjectIdResponse()
        {
            Tags = { tags.Select(x => x.ToDto()) }
        };
    }

    public override async Task<GetManyTagsByIdsResponse> GetManyByIds(GetManyTagsByIdsRequest request, ServerCallContext context)
    {
        var tagIds = request.TagIds.Select(Guid.Parse).ToList();
        var tagsQuery = dbContext.Tags.AsQueryable();
        var tenantId = Guid.Parse(request.TenantId);
        
        if (Guid.TryParse(request.ProjectId, out var projectId))
        {
            tagsQuery = tagsQuery.Where(t => t.ProjectId == projectId && t.TenantId == tenantId && tagIds.Contains(t.Id));
        }
        else
        {
            tagsQuery = tagsQuery.Where(t => t.TenantId == tenantId && tagIds.Contains(t.Id));
        }
        
        var tags = await tagsQuery.ToListAsync(context.CancellationToken);

        return new GetManyTagsByIdsResponse()
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
            TenantId = Guid.Parse(request.TenantId),
            ProjectId = Guid.TryParse(request.ProjectId, out var projectId) ? projectId : null,
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

        await dbContext.Tags.Where(t => t.Id == tagId).ExecuteDeleteAsync(context.CancellationToken);
        
        return new Empty();
    }
}
