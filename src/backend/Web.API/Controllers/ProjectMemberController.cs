using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using OrganizationService.Contracts;
using ProjectService.Contracts;

namespace Web.API.Controllers;

[ApiController]
[Route("api/v1/organizations/{organizationId}/project/{projectId}/members")]
[Authorize]
public class ProjectMemberController(ProjectMemberService.ProjectMemberServiceClient projectMemberServiceClient)
{
    [HttpGet("userId")]
    public async Task<ProjectMemberDto> GetAsync([FromRoute] string organizationId, [FromRoute] string projectId, [FromQuery] string userId)
    {
        return await projectMemberServiceClient.GetAsync(new GetProjectMemberRequest()
        {
            ProjectId = projectId,
            UserId = userId
        });
    }

    [HttpGet]
    public async Task<GetManyProjectMembersResponse> GetManyAsync([FromRoute] string organizationId,
        [FromRoute] string projectId, [FromQuery] string[] userIds)
    {
        return await projectMemberServiceClient.GetManyAsync(new GetManyProjectMembersRequest()
        {
            ProjectId = projectId,
            UserIds = { userIds }
        });
    }

    [HttpPost("{userId}")]
    public async Task AddToProjectAsync([FromRoute] string organizationId, [FromRoute] string projectId,
        [FromRoute] string userId, [FromBody] AddUserToProjectRequest request)
    {
        request.ProjectId = projectId;
        request.UserId = userId;
        
        await projectMemberServiceClient.AddToProjectAsync(request);
    }
    
    [HttpPost]
    public async Task AddManyToProjectAsync([FromRoute] string organizationId, [FromRoute] string projectId,
        [FromBody] AddManyUsersToProjectRequest request)
    {
        request.ProjectId = projectId;
        
        await projectMemberServiceClient.AddManyToProjectAsync(request);
    }

    [HttpDelete("{userId}")]
    public async Task RemoveFromProjectAsync([FromRoute] string organizationId, [FromRoute] string projectId,
        [FromRoute] string userId)
    {
        await projectMemberServiceClient.RemoveFromProjectAsync(new RemoveUserFromProjectRequest()
        {
            ProjectId = projectId,
            UserId = userId
        });
    }

    [HttpDelete]
    public async Task RemoveManyFromProjectAsync([FromRoute] string organizationId, [FromRoute] string projectId,
        [FromBody] RemoveManyUsersFromProjectRequest request)
    {
        request.ProjectId = projectId;
        await projectMemberServiceClient.RemoveManyFromProjectAsync(request);
    }
}