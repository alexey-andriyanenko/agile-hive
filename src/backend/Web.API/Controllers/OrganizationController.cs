using Google.Protobuf.WellKnownTypes;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using OrganizationService.Contracts;
using TenantContextService.Contracts;
using Web.API.Dtos.Organization;

namespace Web.API.Controllers;

[ApiController]
[Route("api/v1/organizations")]
public class OrganizationController(
    TenantContextService.Contracts.TenantContextService.TenantContextServiceClient organizationClient)
{
    [HttpPost]
    [Authorize]
    public async Task<Dtos.Organization.OrganizationDto> CreateOrganizationAsync([FromBody] CreateOrganizationRequest request)
    {
        var response = await organizationClient.CreateTenantAsync(new CreateTenantRequest()
        {
            Name = request.OrganizationName
        }).ResponseAsync;

        return response.ToDto();
    }

    [HttpGet("{organizationId}")]
    [Authorize(Policy = "TenantAccess")]
    public async Task<Dtos.Organization.OrganizationDto> GetOrganizationByIdAsync([FromRoute] string organizationId)
    {
        var response = await organizationClient.GetTenantByIdAsync(new GetTenantByIdRequest()
        {
            Id = organizationId
        }).ResponseAsync;

        return response.ToDto();
    }

    [HttpGet("by-slug/{organizationSlug}")]
    [Authorize]
    public async Task<Dtos.Organization.OrganizationDto> GetOrganizationBySlugAsync([FromRoute] string organizationSlug)
    {
        var response = await organizationClient.GetTenantBySlugAsync(new GetTenantBySlugRequest()
        {
            Slug = organizationSlug
        }).ResponseAsync;
        
        return response.ToDto();
    }

    [HttpGet]
    [Authorize]
    public async Task<Results.Organization.GetManyOrganizationsResponse> GetManyOrganizationsAsync([FromQuery] List<string> organizationIds)
    {
        var response = await organizationClient.GetManyTenantsAsync(new GetManyTenantsRequest()
        {
            Ids = { organizationIds }
        }).ResponseAsync;

        return new Results.Organization.GetManyOrganizationsResponse()
        {
            Organizations = response.Tenants.Select(t => t.ToDto()).ToList()
        };
    }

    [HttpPut("{organizationId}")]
    [Authorize(Policy = "TenantAccess")]
    public async Task<Dtos.Organization.OrganizationDto> UpdateOrganizationAsync([FromRoute] string organizationId,
        [FromBody] UpdateOrganizationRequest request)
    {
        request.OrganizationId = organizationId;
        var response = await organizationClient.UpdateTenantAsync(new UpdateTenantRequest()
        {
            Id = request.OrganizationId,
            Name = request.OrganizationName
        }).ResponseAsync;

        return response.ToDto();
    }

    [HttpDelete("{organizationId}")]
    [Authorize(Policy = "TenantAccess")]
    public async Task<Empty> DeleteOrganizationAsync([FromRoute] string organizationId)
    {
        return await organizationClient.DeleteTenantAsync(new DeleteTenantRequest()
        {
            Id = organizationId
        }).ResponseAsync;
    }
}