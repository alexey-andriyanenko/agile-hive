using Google.Protobuf.WellKnownTypes;
using Grpc.Core;
using Microsoft.AspNetCore.Http;
using Microsoft.EntityFrameworkCore;
using ProjectService.Application.Mappings;
using ProjectService.gRPC;
using ProjectService.Infrastructure;
using ProjectService.Infrastructure.Data;
using ProjectVisibility = ProjectService.Domain.Enums.ProjectVisibility;

namespace ProjectService.Application.Services;

public class ProjectService(
    ApplicationDbContext dbContext,
    IHttpContextAccessor httpContextAccessor
    ) : gRPC.ProjectService.ProjectServiceBase
{
    public override async Task<ProjectDto> CreateProject(CreateProjectRequest request, ServerCallContext context)
    {
        var userContext = (UserContext)httpContextAccessor.HttpContext!.Items["UserContext"]!;

        var project = new Domain.Entities.Project
        {
            Id = Guid.NewGuid(),
            Name = request.Name,
            Slug = request.Slug,
            Visibility = (ProjectVisibility)request.Visibility,
            Description = request.Description,
            CreateByUserId = userContext.UserId,
            CreatedAt = DateTime.UtcNow
        };
        
        dbContext.Projects.Add(project);

        await dbContext.SaveChangesAsync();

        return project.ToDto();
    }

    public override async Task<ProjectDto> UpdateProject(UpdateProjectRequest request, ServerCallContext context)
    {
        var requestedId = Guid.Parse(request.Id);
        var project = await dbContext.Projects.SingleOrDefaultAsync(x => x.Id == requestedId);
        
        if (project is null)
        {
            throw new RpcException(new Status(StatusCode.NotFound, "Project not found"));
        }
        
        project.Name = request.Name;
        project.Slug = request.Slug;
        project.Description = request.Description;
        project.Visibility = (ProjectVisibility)request.Visibility;
        project.UpdatedAt = DateTime.UtcNow;
        
        await dbContext.SaveChangesAsync();
        
        return project.ToDto();
    }
    
    public override async Task<Empty> DeleteProject(DeleteProjectRequest request, ServerCallContext context)
    {
        var requestedId = Guid.Parse(request.Id);
        var project = await dbContext.Projects.SingleOrDefaultAsync(x => x.Id == requestedId);
        
        if (project is null)
        {
            throw new RpcException(new Status(StatusCode.NotFound, "Project not found"));
        }
        
        dbContext.Projects.Remove(project);
        await dbContext.SaveChangesAsync();
        
        return new Empty();
    }
}