using Google.Protobuf.WellKnownTypes;
using Grpc.Core;
using IdentityService.gRPC;
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
    UserService.UserServiceClient userServiceClient,
    IHttpContextAccessor httpContextAccessor
    ) : gRPC.ProjectService.ProjectServiceBase
{
    public override async Task<ProjectDto> Create(CreateProjectRequest request, ServerCallContext context)
    {
        var usersResponse = await userServiceClient.GetManyByIdsAsync(new GetManyUsersByIdsRequest()
        {
            UserIds =
            {
                request.Members.Select(x => x.UserId)
            }
        }).ResponseAsync;
        var usersById = usersResponse.Users.ToDictionary(x => x.Id, x => x);
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

        foreach (var member in request.Members)
        {
            var user = usersById[member.UserId];
            var projectMember = new Domain.Entities.ProjectMember
            {
                ProjectId = project.Id,
                UserId = Guid.Parse(user.Id),
                Role = (Domain.Enums.ProjectMemberRole)member.Role,
            };
            
            project.Members.Add(projectMember);
        }
        
        dbContext.Projects.Add(project);
        
        await dbContext.SaveChangesAsync();

        return project.ToDto();
    }

    public override async Task<ProjectDto> GetById(GetProjectByIdRequest request, ServerCallContext context)
    {
        if (!Guid.TryParse(request.Id, out var projectId))
        {
            throw new RpcException(new Status(StatusCode.InvalidArgument, $"ProjectId '{request.Id}' is not a valid GUID."));
        }

        var project = await dbContext.Projects
            .AsNoTracking()
            .SingleOrDefaultAsync(x => x.Id == projectId);

        if (project is null)
        {
            throw new RpcException(new Status(StatusCode.NotFound, $"Project with ID '{request.Id}' not found."));
        }
        
        return project.ToDto();
    }

    public override async Task<ProjectDto> Update(UpdateProjectRequest request, ServerCallContext context)
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
    
    public override async Task<Empty> Delete(DeleteProjectRequest request, ServerCallContext context)
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