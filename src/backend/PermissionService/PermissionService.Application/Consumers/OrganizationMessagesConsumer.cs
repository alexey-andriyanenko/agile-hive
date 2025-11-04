using MassTransit;
using OrganizationMessages.Messages;
using PermissionService.Domain.Entities;
using PermissionService.Domain.Enums;
using PermissionService.Infrastructure.Data;

namespace PermissionService.Application.Consumers;

public class OrganizationMessagesConsumer(ApplicationDbContext dbContext, IPublishEndpoint publishEndpoint)
    : IConsumer<OrganizationCreationSucceededMessage>
{
    public async Task Consume(ConsumeContext<OrganizationCreationSucceededMessage> context)
    {
        var message = context.Message;
        
        var permissions = GenerateDefaultPermissionsForTenant(message.OrganizationId);
        
        dbContext.Permissions.AddRange(permissions);
        await dbContext.SaveChangesAsync();
    }

    private static IReadOnlyList<PermissionEntity> GenerateDefaultPermissionsForTenant(Guid tenantId)
    {
        var permissions = new List<PermissionEntity>();

        // ------------------------
        // ORGANIZATION SCOPE ROLES
        // ------------------------
        var orgResources = new[]
        { 
            ResourceType.Organization,
            ResourceType.OrganizationMember,
            ResourceType.Project,
            ResourceType.Tag
        };

        foreach (OrganizationMemberRole role in Enum.GetValues(typeof(OrganizationMemberRole)))
        {
            foreach (var resource in orgResources)
            {
                foreach (OperationType op in Enum.GetValues(typeof(OperationType)))
                {
                    var allowed = IsOperationAllowedForOrganizationRole(role, resource, op);

                    permissions.Add(new PermissionEntity
                    {
                        Id = Guid.NewGuid(),
                        TenantId = tenantId,
                        Scope = ScopeType.Organization,
                        Role = role.ToString(),
                        Resource = resource,
                        Operation = op,
                        Allowed = allowed
                    });
                }
            }
        }

        // --------------------
        // PROJECT SCOPE ROLES
        // --------------------
        var projectResources = new[]
        {
            ResourceType.ProjectMember,
            ResourceType.Board,
            ResourceType.BoardColumn,
            ResourceType.Task,
            ResourceType.TaskComment,
            ResourceType.Tag
        };

        foreach (ProjectMemberRole role in Enum.GetValues(typeof(ProjectMemberRole)))
        {
            foreach (var resource in projectResources)
            {
                foreach (OperationType op in Enum.GetValues(typeof(OperationType)))
                {
                    var allowed = IsOperationAllowedForProjectRole(role, resource, op);

                    permissions.Add(new PermissionEntity
                    {
                        Id = Guid.NewGuid(),
                        TenantId = tenantId,
                        Scope = ScopeType.Project,
                        Role = role.ToString(),
                        Resource = resource,
                        Operation = op,
                        Allowed = allowed
                    });
                }
            }
        }

        return permissions;
    }

    // --------------------------
    // Permission rules per role
    // --------------------------

    private static bool IsOperationAllowedForOrganizationRole(OrganizationMemberRole role, ResourceType resource,
        OperationType op)
    {
        switch (role)
        {
            case OrganizationMemberRole.Owner:
                return true; // Full control

            case OrganizationMemberRole.Admin:
                // Everything except billing/ownership transfer (not modeled here)
                return true;

            case OrganizationMemberRole.Manager:
                // Can manage projects and members, not org settings
                if (resource == ResourceType.Project || resource == ResourceType.OrganizationMember)
                    return op != OperationType.Delete;
                if (resource == ResourceType.Tag)
                    return op != OperationType.Delete;
                return op == OperationType.Read;

            case OrganizationMemberRole.Member:
                // Read-only access to org/project metadata
                return op == OperationType.Read;

            case OrganizationMemberRole.Guest:
                // Guests can only read tags and project info
                return resource == ResourceType.Tag && op == OperationType.Read;
        }

        return false;
    }

    private static bool IsOperationAllowedForProjectRole(ProjectMemberRole role, ResourceType resource,
        OperationType op)
    {
        switch (role)
        {
            case ProjectMemberRole.Owner:
                return true; // Full control in project scope

            case ProjectMemberRole.Admin:
                // Can manage all project-level entities except deleting boards
                if (resource == ResourceType.Board && op == OperationType.Delete)
                    return false;
                return true;

            case ProjectMemberRole.Contributor:
                // Can read/create/update tasks, comments, tags, but not delete
                if (resource is ResourceType.Task or ResourceType.TaskComment or ResourceType.Tag)
                    return op != OperationType.Delete;
                return op == OperationType.Read;

            case ProjectMemberRole.Reader:
                // Read-only
                return op == OperationType.Read;
        }

        return false;
    }
}