using Web.API.Contracts.Dtos;

namespace Web.API.Contracts;

public interface IOrganizationUserService
{
    Task<OrganizationUserDto> GetByIdAsync(Guid organizationId, Guid userId, CancellationToken cancellationToken = default);
    
    Task<OrganizationUserDto> AddToOrganizationAsync(Guid organizationId, Guid userId, CancellationToken cancellationToken = default);
}