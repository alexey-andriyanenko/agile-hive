using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using ProjectUserService.Contracts;
using Web.API.Parameters.ProjectUser;
using Web.API.Results.ProjectUser;

namespace Web.API.Controllers;

[ApiController]
[Route("api/v1/organization/{organizationId}/projects/{projectId}/users")]
[Authorize(Policy = "TenantAccess")]
public class ProjectUserController(ProjectUserService.Contracts.ProjectUserService.ProjectUserServiceClient projectUserServiceClient)
{
    [HttpGet("{userId}")]
    public async Task<ProjectUserDto> GetByIdAsync([FromRoute] Guid organizationId,
        [FromRoute] Guid projectId,
        [FromRoute] Guid userId)
    {
        return await projectUserServiceClient.GetByIdAsync(new ProjectUserService.Contracts.GetProjectUserByIdRequest()
        {
            ProjectId = projectId.ToString(),
            UserId = userId.ToString(),
        }).ResponseAsync;
    }

    [HttpGet("by-ids")]
    public async Task<GetManyProjectUsersByIdsResponse> GetManyByIdsAsync([FromRoute] Guid organizationId,
        [FromRoute] Guid projectId,
        [FromQuery] List<Guid> userIds)
    {
        return await projectUserServiceClient.GetManyByIdsAsync(new GetManyProjectUsersByIdsRequest()
        {
            ProjectId = projectId.ToString(),
            UserIds = { userIds.Select(x => x.ToString()) }
        }).ResponseAsync;
    }
    
    [HttpGet]
    public async Task<GetManyProjectUsersResponse> GetManyAsync([FromRoute] Guid organizationId,
        [FromRoute] Guid projectId)
    {
        return await projectUserServiceClient.GetManyAsync(new GetManyProjectUsersRequest()
        {
            ProjectId = projectId.ToString(),
        }).ResponseAsync;
    }
    
    [HttpPost]
    public async Task<ProjectUserDto> AddToProjectAsync([FromRoute] Guid organizationId,
        [FromRoute] Guid projectId,
        [FromBody] AddUserToProjectRequest parameters)
    {
        parameters.ProjectId = projectId.ToString();
        return await projectUserServiceClient.AddAsync(parameters);
    }

    [HttpPost("many")]
    public async Task<AddManyUsersToProjectResult> AddManyToProjectAsync([FromRoute] Guid organizationId,
        [FromRoute] Guid projectId,
        [FromBody] Parameters.ProjectUser.AddManyUsersToProjectRequest parameters)
    {
        parameters.ProjectId = projectId;
        
        var grpcParameters = parameters.ToGrpc();
        
        var result = await projectUserServiceClient.AddManyAsync(grpcParameters);

        return result.ToHttp();
    }

    [HttpPut("{userId}")]
    public async Task<ProjectUserDto> UpdateAsync([FromRoute] Guid organizationId,
        [FromRoute] Guid projectId,
        [FromRoute] Guid userId,
        [FromBody] UpdateProjectUserRequest parameters)
    {
        parameters.ProjectId = projectId.ToString();
        parameters.UserId = userId.ToString();
        return await projectUserServiceClient.UpdateAsync(parameters);
    }

    [HttpDelete("{userId}")]
    public async Task RemoveFromProjectAsync([FromRoute] Guid organizationId,
        [FromRoute] Guid projectId,
        [FromRoute] Guid userId)
    {
        await projectUserServiceClient.RemoveAsync(new RemoveUserFromProjectRequest()
        {
            ProjectId = projectId.ToString(),
            UserId = userId.ToString()
        });
    }

    [HttpDelete("many")]
    public async Task RemoveManyFromProjectAsync([FromRoute] Guid organizationId,
        [FromRoute] Guid projectId,
        [FromBody] RemoveManyUsersFromProjectRequest parameters)
    {
        parameters.ProjectId = projectId.ToString();
        await projectUserServiceClient.RemoveManyAsync(parameters);
    }
}