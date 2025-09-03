using ProjectService.Contracts;

namespace ProjectService.Application.Mappings;

public static class ProjectMappings
{
    public static ProjectDto ToDto(this Domain.Entities.Project project)
    {
        return new ProjectDto()
        {
            Id = project.Id.ToString(),
            Name = project.Name,
            Slug = project.Slug,
            Description = project.Description,
            OrganizationId = project.OrganizationId.ToString(),
        };
    }
}