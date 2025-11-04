using PermissionService.Contracts;

namespace PermissionService.Application.Mappings;

public static class PermissionMappings
{
    public static PermissionDto ToDto(this Domain.Entities.PermissionEntity permission)
    {
        return new PermissionDto
        {
            TenantId = permission.TenantId.ToString(),
            Scope = permission.Scope.ToString(),
            Resource = permission.Resource.ToString(),
            Operation = permission.Operation.ToString(),
            Allowed = permission.Allowed
        };
    }
}