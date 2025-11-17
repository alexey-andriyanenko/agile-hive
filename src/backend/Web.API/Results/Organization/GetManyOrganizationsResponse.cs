using Web.API.Dtos.Organization;

namespace Web.API.Results.Organization;

public class GetManyOrganizationsResponse
{
    public IReadOnlyList<OrganizationDto> Organizations { get; set; } = [];
}