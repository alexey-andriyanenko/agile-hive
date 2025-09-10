using Google.Protobuf.WellKnownTypes;
using Grpc.Core;
using IdentityService.Contracts;
using Microsoft.AspNetCore.Http;
using Microsoft.EntityFrameworkCore;
using ProjectService.Application.Extensions;
using ProjectService.Application.Mappings;
using ProjectService.Contracts;
using ProjectService.Infrastructure;
using ProjectService.Infrastructure.Data;
using ProjectVisibility = ProjectService.Domain.Enums.ProjectVisibility;

namespace ProjectService.Application.Services;

public class ProjectService(
    ApplicationDbContext dbContext,
    UserService.UserServiceClient userServiceClient,
    IHttpContextAccessor httpContextAccessor
    ) : Contracts.ProjectService.ProjectServiceBase
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
            Slug = request.Name.ToSlug(),
            OrganizationId = Guid.Parse(request.OrganizationId),
            Visibility = (ProjectVisibility)request.Visibility,
            Description = request.Description,
            CreateByUserId = userContext.UserId,
            CreatedAt = DateTime.UtcNow
        };

        // Add the creating user as an owner if not already added
        if (usersById.ContainsKey(userContext.UserId.ToString()))
        {
            throw new RpcException(new Status(StatusCode.InvalidArgument, "The creating user cannot be added as a member explicitly."));
        }
        
        var memberUser = new Domain.Entities.ProjectMember
        {
            ProjectId = project.Id,
            UserId = userContext.UserId,
            Role = Domain.Enums.ProjectMemberRole.Owner,
        };
        project.Members.Add(memberUser);
        
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
        

        if (memberUser is null)
        {
            throw new RpcException(new Status(StatusCode.Internal, "Failed to retrieve project member for the creating user."));
        }

        return project.ToDto(memberUser);
    }

    public override async Task<ProjectDto> GetById(GetProjectByIdRequest request, ServerCallContext context)
    {
        var userContext = (UserContext)httpContextAccessor.HttpContext!.Items["UserContext"]!;
        var project = await dbContext.Projects
            .AsNoTracking()
            .SingleOrDefaultAsync(x => x.Id == Guid.Parse(request.ProjectId) && x.OrganizationId == Guid.Parse(request.OrganizationId));

        if (project is null)
        {
            throw new RpcException(new Status(StatusCode.NotFound, $"Project with ID '{request.ProjectId}' not found in organization with ID '{request.OrganizationId}'."));
        }
        
        var projectMember = await dbContext.ProjectMembers
            .AsNoTracking()
            .SingleOrDefaultAsync(x => x.ProjectId == project.Id && x.UserId == userContext.UserId);
        
        if (projectMember is null)
        {
            throw new RpcException(new Status(StatusCode.PermissionDenied, $"User with id '{userContext.UserId}' does not have access to project with ID '{request.ProjectId}'."));
        }
        
        return project.ToDto(projectMember);
    }

    public override async Task<ProjectDto> GetBySlug(GetProjectBySlugRequest request, ServerCallContext context)
    {
        var userContext = (UserContext)httpContextAccessor.HttpContext!.Items["UserContext"]!;
        var project = await dbContext.Projects
            .AsNoTracking()
            .SingleOrDefaultAsync(x => x.Slug == request.ProjectSlug && x.OrganizationId == Guid.Parse(request.OrganizationId));
        
        if (project is null)
        {
            throw new RpcException(new Status(StatusCode.NotFound, $"Project with slug '{request.ProjectSlug}' not found in organization with ID '{request.OrganizationId}'."));
        }
        
        var member = await dbContext.ProjectMembers
            .AsNoTracking()
            .SingleOrDefaultAsync(x => x.ProjectId == project.Id && x.UserId == userContext.UserId);
        
        if (member is null) 
        {
            throw new RpcException(new Status(StatusCode.PermissionDenied, $"User with id '{userContext.UserId}' does not have access to project with slug '{request.ProjectSlug}'."));
        }
        
        return project.ToDto(member);
    }

    public override async Task<GetManyProjectsResponse> GetMany(GetManyProjectsRequest request, ServerCallContext context)
    {
        var userContext = (UserContext)httpContextAccessor.HttpContext!.Items["UserContext"]!;
        
        var projectIdsAssociatedWithUser = await dbContext.ProjectMembers
            .AsNoTracking()
            .Where(x => x.UserId == userContext.UserId)
            .Select(x => x.ProjectId)
            .ToListAsync();
        var projectIds = request.ProjectIds.Count > 0
            ? request.ProjectIds.Select(Guid.Parse).Intersect(projectIdsAssociatedWithUser).ToList()
            : projectIdsAssociatedWithUser.ToList();
        
        var projects = await dbContext.Projects
            .Where(x => projectIds.Contains(x.Id) && x.OrganizationId == Guid.Parse(request.OrganizationId))
            .ToListAsync();
        
        var projectMembers = await dbContext.ProjectMembers
            .AsNoTracking()
            .Where(x => x.UserId == userContext.UserId && projectIds.Contains(x.ProjectId))
            .ToDictionaryAsync(x => x.ProjectId, x => x);
        
        return new GetManyProjectsResponse()
        {
            Projects = { projects.Select(p => p.ToDto(projectMembers[p.Id])) }
        };
    }

    public override async Task<ProjectDto> Update(UpdateProjectRequest request, ServerCallContext context)
    {
        var userContext = (UserContext)httpContextAccessor.HttpContext!.Items["UserContext"]!;
        var project = await dbContext.Projects.SingleOrDefaultAsync(x => x.Id == Guid.Parse(request.ProjectId) && x.OrganizationId == Guid.Parse(request.OrganizationId));
        
        if (project is null)
        {
            throw new RpcException(new Status(StatusCode.NotFound, "Project not found"));
        }
        
        var member = await dbContext.ProjectMembers
            .AsNoTracking()
            .SingleOrDefaultAsync(x => x.ProjectId == project.Id && x.UserId == userContext.UserId);
        
        if (member is null)
        {
            throw new RpcException(new Status(StatusCode.PermissionDenied, $"User with id '{userContext.UserId}' does not have access to project with ID '{request.ProjectId}'."));
        }
        
        project.Name = request.Name;
        project.Slug = request.Name.ToSlug();
        project.Description = request.Description;
        project.Visibility = (ProjectVisibility)request.Visibility;
        project.UpdatedAt = DateTime.UtcNow;
        
        await dbContext.SaveChangesAsync();
        
        return project.ToDto(member);
    }
    
    public override async Task<Empty> Delete(DeleteProjectRequest request, ServerCallContext context)
    {
        var userContext = (UserContext)httpContextAccessor.HttpContext!.Items["UserContext"]!;
        var project = await dbContext.Projects.SingleOrDefaultAsync(x => x.Id == Guid.Parse(request.ProjectId) && x.OrganizationId == Guid.Parse(request.OrganizationId));
        
        if (project is null)
        {
            throw new RpcException(new Status(StatusCode.NotFound, "Project not found"));
        }
        
        var member = await dbContext.ProjectMembers
            .AsNoTracking()
            .SingleOrDefaultAsync(x => x.ProjectId == project.Id && x.UserId == userContext.UserId);

        if (member is null)
        {
            throw new RpcException(new Status(StatusCode.PermissionDenied, $"User with id '{userContext.UserId}' does not have access to project with ID '{request.ProjectId}'."));
        }
        
        if (member.Role != Domain.Enums.ProjectMemberRole.Owner)
        {
            throw new RpcException(new Status(StatusCode.PermissionDenied, "Only project owners can delete the project."));
        }
        
        dbContext.Projects.Remove(project);
        await dbContext.SaveChangesAsync();
        
        return new Empty();
    }
}