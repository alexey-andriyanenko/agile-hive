using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Tag.Contracts;
using TagDto = Tag.Contracts.TagDto;

namespace Web.API.Controllers;

[ApiController]
[Authorize(Policy = "TenantAccess")]
[Route("/api/v1/organizations/{organizationId}/tags")]
public class TagController : ControllerBase
{
    [HttpGet("by-tenant")]
    public async Task<ActionResult<Results.Tag.GetTagsResult>> GetManyByTenantIdAsync(
        [FromServices] TagService.TagServiceClient tagServiceClient,
        [FromRoute] Guid organizationId,
        CancellationToken cancellationToken)
    {
        var response = await tagServiceClient.getManyByTenantIdAsync(new GetManyTagsByTenantIdRequest()
        {
            TenantId = organizationId.ToString()
        }, cancellationToken: cancellationToken);

        var result = new Results.Tag.GetTagsResult()
        {
            Tags = response.Tags.ToList()
        };

        return Ok(result);
    }
    
    [HttpGet("by-project")]
    public async Task<ActionResult<Results.Tag.GetTagsResult>> GetManyByProjectIdAsync(
        [FromServices] TagService.TagServiceClient tagServiceClient,
        [FromRoute] Guid organizationId,
        [FromQuery] Guid projectId,
        CancellationToken cancellationToken)
    {
        var response = await tagServiceClient.GetManyByProjectIdAsync(new GetManyTagsByProjectIdRequest()
        {
            TenantId = organizationId.ToString(),
            ProjectId = projectId.ToString()
        }, cancellationToken: cancellationToken);

        var result = new Results.Tag.GetTagsResult
        {
            Tags = response.Tags.ToList()
        };

        return Ok(result);
    }

    [HttpPost]
    public async Task<ActionResult<TagDto>> CreateAsync(
        [FromServices] TagService.TagServiceClient tagServiceClient,
        [FromRoute] Guid organizationId,
        [FromQuery] Guid? projectId,
        [FromBody] CreateTagRequest request,
        CancellationToken cancellationToken)
    {
        request.TenantId = organizationId.ToString();
        request.ProjectId = projectId?.ToString();

        var response = await tagServiceClient.CreateAsync(request, cancellationToken: cancellationToken);

        return Ok(response);
    }

    [HttpPut("{tagId:guid}")]
    public async Task<ActionResult<TagDto>> UpdateAsync(
        [FromServices] TagService.TagServiceClient tagServiceClient,
        [FromRoute] Guid organizationId,
        [FromRoute] Guid tagId,
        [FromBody] UpdateTagRequest request,
        CancellationToken cancellationToken)
    {
        request.TenantId = organizationId.ToString();
        request.TagId = tagId.ToString();
        
        var response = await tagServiceClient.UpdateAsync(request, cancellationToken: cancellationToken);
        
        return Ok(response);
    }

    [HttpDelete("{tagId:guid}")]
    public async Task<ActionResult> DeleteAsync(
        [FromServices] TagService.TagServiceClient tagServiceClient,
        [FromRoute] Guid organizationId,
        [FromRoute] Guid tagId,
        CancellationToken cancellationToken)
    {
        var request = new DeleteTagRequest
        {
            TenantId = organizationId.ToString(),
            TagId = tagId.ToString()
        };
        
        await tagServiceClient.DeleteAsync(request, cancellationToken: cancellationToken);
        
        return NoContent();
    }
}