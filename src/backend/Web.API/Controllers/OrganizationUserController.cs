using Google.Protobuf.WellKnownTypes;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using OrganizationUserService.Contracts;

namespace Web.API.Controllers;

[ApiController]
[Route("api/v1/organizations/{organizationId}/users")]
[Authorize(Policy = "TenantAccess")]
public class OrganizationUserController(
    OrganizationUserService.Contracts.OrganizationUserService.OrganizationUserServiceClient organizationUserServiceClient
    )
{
    [HttpGet("{userId}")]
    public async Task<OrganizationUserDto> GetByIdAsync([FromRoute] Guid organizationId,
        [FromRoute] Guid userId)
    {
        return await organizationUserServiceClient.GetByIdAsync(new GetOrganizationUserByIdRequest()
        {
            OrganizationId = organizationId.ToString(),
            UserId = userId.ToString(),
        }).ResponseAsync;
    }

    [HttpGet("by-ids")]
    public async Task<GetManyOrganizationUsersByIdsResponse> GetManyByIdsAsync([FromRoute] Guid organizationId,
        [FromQuery] List<Guid> userIds)
    {
        return await organizationUserServiceClient.GetManyByIdsAsync(new GetManyOrganizationUsersByIdsRequest()
        {
            OrganizationId = organizationId.ToString(),
            UserIds = { userIds.Select(x => x.ToString()) }
        }).ResponseAsync;
    }
    
    [HttpGet]
    public async Task<GetManyOrganizationUsersResponse> GetManyAsync([FromRoute] Guid organizationId)
    {
        return await organizationUserServiceClient.GetManyAsync(new GetManyOrganizationUsersRequest()
        {
            OrganizationId = organizationId.ToString(),
        }).ResponseAsync;
    }

    [HttpPost]
    public async Task<OrganizationUserDto> CreateAsync([FromRoute] Guid organizationId,
        [FromBody] CreateOrganizationUserRequest parameters)
    {
        return await organizationUserServiceClient.CreateAsync(new CreateOrganizationUserRequest()
        {
            OrganizationId = organizationId.ToString(),
            FirstName = parameters.FirstName,
            LastName = parameters.LastName,
            UserName = parameters.UserName,
            Email = parameters.Email,
            Password = parameters.Password,
            Role = parameters.Role
        }).ResponseAsync;
    }
    
    [HttpPost("many")]
    public async Task<CreateManyOrganizationUsersResponse> CreateManyAsync([FromRoute] Guid organizationId,
        [FromBody] CreateManyOrganizationUsersRequest parameters)
    {
        return await organizationUserServiceClient.CreateManyAsync(new CreateManyOrganizationUsersRequest()
        {
            OrganizationId = organizationId.ToString(),
            Users = { parameters.Users }
        }).ResponseAsync;
    }
    
    [HttpPut("{userId}")]
    public async Task<OrganizationUserDto> UpdateAsync([FromRoute] string organizationId, [FromRoute] string userId,
        [FromBody] UpdateOrganizationUserRequest parameters)
    {
        return await organizationUserServiceClient.UpdateAsync(new UpdateOrganizationUserRequest()
        {
            OrganizationId = organizationId,
            UserId = userId,
            FirstName = parameters.FirstName,
            LastName = parameters.LastName,
            UserName = parameters.UserName,
            Email = parameters.Email,
            Role = parameters.Role
        }).ResponseAsync;
    }
    

    [HttpDelete("{userId}")]
    public async Task<Empty> RemoveFromOrganizationAsync([FromRoute] string organizationId, [FromRoute] string userId)
    {
        return await organizationUserServiceClient.RemoveAsync(new OrganizationUserService.Contracts.RemoveUserFromOrganizationRequest()
        {
            OrganizationId = organizationId,
            UserId = userId
        }).ResponseAsync;
    }

    [HttpDelete]
    public async Task<Empty> RemoveManyFromOrganizationAsync([FromRoute] string organizationId,
        [FromQuery] List<string> userIds)
    {
        return await organizationUserServiceClient.RemoveManyAsync(
            new OrganizationUserService.Contracts.RemoveManyUsersFromOrganizationRequest()
            {
                OrganizationId = organizationId,
                UserIds = { userIds }
            }).ResponseAsync;
    }
}