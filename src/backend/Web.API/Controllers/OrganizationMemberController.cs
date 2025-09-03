using Google.Protobuf.WellKnownTypes;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using OrganizationService.Contracts;

namespace Web.API.Controllers;

[ApiController]
[Route("api/v1/organizations/{organizationId}/members")]
[Authorize]
public class OrganizationMemberController(OrganizationMemberService.OrganizationMemberServiceClient organizationMemberClient)
{
    [HttpGet("{userId}")]
    public async Task<OrganizationMemberDto> GetOrganizationMemberAsync([FromRoute] string organizationId,
        [FromRoute] string userId)
    {
        return await organizationMemberClient.GetAsync(new GetOrganizationMemberRequest()
        {
            OrganizationId = organizationId,
            UserId = userId
        }).ResponseAsync;
    }

    [HttpGet]
    public async Task<GetManyOrganizationMembersResponse> GetManyOrganizationMembersAsync(
        [FromRoute] string organizationId,
        [FromQuery] List<string> userIds)
    {
        return await organizationMemberClient.GetManyAsync(new GetManyOrganizationMembersRequest()
        {
            OrganizationId = organizationId,
            UserIds = { userIds }
        }).ResponseAsync;
    }
    
    [HttpPost("{userId}")]
    public async Task<Empty> AddToOrganizationAsync([FromRoute] string organizationId, [FromRoute] string userId,
        [FromBody] AddUserToOrganizationRequest request)
    {
        request.OrganizationId = organizationId;
        request.UserId = userId;
        return await organizationMemberClient.AddToOrganizationAsync(request).ResponseAsync;
    }
    
    [HttpPost]
    public async Task<Empty> AddManyToOrganizationAsync([FromRoute] string organizationId,
        [FromBody] AddManyUsersToOrganizationRequest request)
    {
        request.OrganizationId = organizationId;
        return await organizationMemberClient.AddManyToOrganizationAsync(request).ResponseAsync;
    }

    [HttpDelete("{userId}")]
    public async Task<Empty> RemoveFromOrganizationAsync([FromRoute] string organizationId, [FromRoute] string userId)
    {
        return await organizationMemberClient.RemoveFromOrganizationAsync(new RemoveUserFromOrganizationRequest()
        {
            OrganizationId = organizationId,
            UserId = userId
        }).ResponseAsync;
    }

    [HttpDelete]
    public async Task<Empty> RemoveManyFromOrganizationAsync([FromRoute] string organizationId,
        [FromQuery] List<string> userIds)
    {
        return await organizationMemberClient.RemoveManyFromOrganizationAsync(
            new RemoveManyUsersFromOrganizationRequest()
            {
                OrganizationId = organizationId,
                UserIds = { userIds }
            }).ResponseAsync;
    }
}