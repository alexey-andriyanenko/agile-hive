using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using ProjectService.Contracts;

namespace Web.API.Controllers;

[ApiController]
[Route("api/v1/organizations/{organizationId}/projects")]
[Authorize]
public class ProjectController(ProjectService.Contracts.ProjectService.ProjectServiceClient projectClient)
{
    [HttpPost]
    public async Task<ProjectDto> CreateAsync([FromRoute] string organizationId, [FromBody] CreateProjectRequest request)
    {
        request.OrganizationId = organizationId;
        return await projectClient.CreateAsync(request);
    }
    
    [HttpGet("{projectId}")]
    public async Task<ProjectDto> GetByIdAsync([FromRoute] string organizationId, [FromRoute] string projectId)
    {
        return await projectClient.GetByIdAsync(new GetProjectByIdRequest
        {
            OrganizationId = organizationId,
            ProjectId = projectId
        });
    }
    
    [HttpGet]
    public async Task<GetManyProjectsResponse> GetManyAsync([FromRoute] string organizationId, [FromQuery] string[] projectIds)
    {
        return await projectClient.GetManyAsync(new GetManyProjectsRequest()
        {
            OrganizationId = organizationId,
            ProjectIds = { projectIds }
        });
    }

    [HttpPut("{projectId}")]
    public async Task<ProjectDto> UpdateAsync([FromRoute] string organizationId, [FromRoute] string projectId, [FromBody] UpdateProjectRequest request)
    {
        request.ProjectId = projectId;
        request.OrganizationId = organizationId;
        return await projectClient.UpdateAsync(request);
    }

    [HttpDelete("{projectId}")]
    public async Task DeleteAsync([FromRoute] string organizationId, [FromRoute] string projectId)
    {
        await projectClient.DeleteAsync(new DeleteProjectRequest
        {
            OrganizationId = organizationId,
            ProjectId = projectId
        });
    }
}