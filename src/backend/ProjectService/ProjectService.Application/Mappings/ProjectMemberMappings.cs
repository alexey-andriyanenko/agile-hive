using ProjectService.gRPC;

namespace ProjectService.Application.Mappings;

public static class ProjectMemberMappings
{
    public static ProjectMemberDto ToDto(this Domain.Entities.ProjectMember projectMember) =>
        new()
        {
            ProjectId = projectMember.ProjectId.ToString(),
            UserId = projectMember.UserId.ToString(),
            Role = (ProjectMemberRole)projectMember.Role,
        };
}