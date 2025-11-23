using Google.Protobuf.WellKnownTypes;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using OrganizationService.Contracts;

namespace Web.API.Controllers;

[ApiController]
[Route("api/v1/organizations")]
public class OrganizationController(
    OrganizationService.Contracts.OrganizationService.OrganizationServiceClient organizationClient)
{
    [HttpPost]
    [Authorize]
    public async Task<OrganizationDto> CreateOrganizationAsync([FromBody] CreateOrganizationRequest request)
    {
        return await organizationClient.CreateAsync(request).ResponseAsync;
    }

    [HttpGet("{organizationId}")]
    [Authorize(Policy = "TenantAccess")]
    public async Task<OrganizationDto> GetOrganizationByIdAsync([FromRoute] string organizationId)
    {
        return await organizationClient.GetByIdAsync(new GetOrganizationByIdRequest()
        {
            OrganizationId = organizationId
        }).ResponseAsync;
    }

    [HttpGet("by-slug/{organizationSlug}")]
    [Authorize]
    public async Task<OrganizationDto> GetOrganizationBySlugAsync([FromRoute] string organizationSlug)
    {
        return await organizationClient.GetBySlugAsync(new GetOrganizationBySlugRequest()
        {
            OrganizationSlug = organizationSlug
        }).ResponseAsync;
    }

    [HttpGet]
    [Authorize]
    public async Task<GetManyOrganizationsResponse> GetManyOrganizationsAsync([FromQuery] List<string> organizationIds)
    {
        return await organizationClient.GetManyAsync(new GetManyOrganizationsRequest()
        {
            OrganizationIds = { organizationIds }
        }).ResponseAsync;
    }

    [HttpPut("{organizationId}")]
    [Authorize(Policy = "TenantAccess")]
    public async Task<OrganizationDto> UpdateOrganizationAsync([FromRoute] string organizationId,
        [FromBody] UpdateOrganizationRequest request)
    {
        request.OrganizationId = organizationId;
        return await organizationClient.UpdateAsync(request).ResponseAsync;
    }

    [HttpDelete("{organizationId}")]
    [Authorize(Policy = "TenantAccess")]
    public async Task<Empty> DeleteOrganizationAsync([FromRoute] string organizationId)
    {
        return await organizationClient.DeleteAsync(new DeleteOrganizationRequest()
        {
            OrganizationId = organizationId
        }).ResponseAsync;
    }

    [HttpDelete]
    [Authorize]
    public async Task<Empty> DeleteManyOrganizationsAsync([FromQuery] List<string> organizationIds)
    {
        return await organizationClient.DeleteManyAsync(new DeleteManyOrganizationsRequest()
        {
            OrganizationIds = { organizationIds }
        }).ResponseAsync;
    }
}