using Google.Protobuf.WellKnownTypes;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using OrganizationService.Contracts;

namespace Web.API.Controllers;

[ApiController]
[Route("api/v1/organizations")]
[Authorize]
public class OrganizationController(OrganizationService.Contracts.OrganizationService.OrganizationServiceClient organizationClient)
{
    [HttpPost]
    public async Task<OrganizationDto> CreateOrganizationAsync([FromBody] CreateOrganizationRequest request)
    {
        return await organizationClient.CreateAsync(request).ResponseAsync;
    }

    [HttpGet("{organizationId}")]
    public async Task<OrganizationDto> GetOrganizationByIdAsync([FromRoute] string organizationId)
    {
        return await organizationClient.GetByIdAsync(new GetOrganizationByIdRequest()
        {
            OrganizationId = organizationId
        }).ResponseAsync;
    }
    
    [HttpGet("by-slug/{organizationSlug}")]
    public async Task<OrganizationDto> GetOrganizationBySlugAsync([FromRoute] string organizationSlug)
    {
        return await organizationClient.GetBySlugAsync(new GetOrganizationBySlugRequest() 
        {
            OrganizationSlug = organizationSlug
        }).ResponseAsync;
    }
    
    [HttpGet]
    public async Task<GetManyOrganizationsResponse> GetManyOrganizationsAsync([FromQuery] List<string> organizationIds)
    {
        return await organizationClient.GetManyAsync(new GetManyOrganizationsRequest()
        {
            OrganizationIds = { organizationIds }
        }).ResponseAsync;
    }

    [HttpPut("{organizationId}")]
    public async Task<OrganizationDto> UpdateOrganizationAsync([FromRoute] string organizationId,
        [FromBody] UpdateOrganizationRequest request)
    {
        request.OrganizationId = organizationId;
        return await organizationClient.UpdateAsync(request).ResponseAsync;
    }

    [HttpDelete("{organizationId}")]
    public async Task<Empty> DeleteOrganizationAsync([FromRoute] string organizationId)
    {
        return await organizationClient.DeleteAsync(new DeleteOrganizationRequest()
        {
            OrganizationId = organizationId
        }).ResponseAsync;
    }
    
    [HttpDelete]
    public async Task<Empty> DeleteManyOrganizationsAsync([FromQuery] List<string> organizationIds)
    {
        return await organizationClient.DeleteManyAsync(new DeleteManyOrganizationsRequest()
        {
            OrganizationIds = { organizationIds }
        }).ResponseAsync;
    }
}