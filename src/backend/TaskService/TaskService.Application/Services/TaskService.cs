using Google.Protobuf.WellKnownTypes;
using Grpc.Core;
using Microsoft.AspNetCore.Http;
using Microsoft.EntityFrameworkCore;
using TaskService.Application.Mappings;
using TaskService.Contracts;
using TaskService.Domain.Entities;
using TaskService.Infrastructure;
using TaskService.Infrastructure.Data;

namespace TaskService.Application.Services;

public class TaskService(ApplicationDbContext dbContext, IHttpContextAccessor httpContextAccessor) : Contracts.TaskService.TaskServiceBase
{
        public override async Task<GetManyTasksByTenantIdResponse> GetManyByTenantId(GetManyTasksByTenantIdRequest request, ServerCallContext context)
    {
        var tasksCount = await dbContext.Tasks
            .Where(t => t.TenantId == Guid.Parse(request.TenantId))
            .CountAsync(context.CancellationToken);

        return new GetManyTasksByTenantIdResponse()
        {
            TotalCount = tasksCount,
        };
    }
    
    public override async Task<GenerateCsvResponse> GenerateCsv(Empty request, ServerCallContext context)
    {
        var tasks = await dbContext.Tasks.ToListAsync(context.CancellationToken);
        
        var byTenantId = tasks
            .GroupBy(t => t.TenantId)
            .ToDictionary(g => g.Key, g => g.ToList());

        var csv = "TenantId,ProjectId,BoardId,BoardColumnId\n";
        
        foreach (var tenantGroup in byTenantId)
        {
            var byProjectId = tenantGroup.Value
                .GroupBy(t => t.ProjectId)
                .ToDictionary(g => g.Key, g => g.ToList());
            foreach (var projectGroup in byProjectId)  
            {
                var byBoardId = projectGroup.Value
                    .GroupBy(t => t.BoardId)
                    .ToDictionary(g => g.Key, g => g.ToList());
                foreach (var boardGroup in byBoardId)
                {
                    var byBoardColumnId = boardGroup.Value
                        .GroupBy(t => t.BoardColumnId)
                        .ToDictionary(g => g.Key, g => g.ToList());
                    foreach (var boardColumnGroup in byBoardColumnId)
                    {
                        csv += $"{tenantGroup.Key},{projectGroup.Key},{boardGroup.Key},{boardColumnGroup.Key}\n";
                    }
                }
            }
        }
        
        return new GenerateCsvResponse()
        {
            FileName = "output.csv",
            FileContent = csv
        };
    }
     
    public override async Task<GetManyTasksByBoardIdResponse> GetManyByBoardId(GetManyTasksByBoardIdRequest request, ServerCallContext context)
    {
        var tenantId = Guid.Parse(request.TenantId);
        var projectId = Guid.Parse(request.ProjectId);
        var boardId = Guid.Parse(request.BoardId);
        var totalCount = await dbContext.Tasks
            .Where(t => t.TenantId == tenantId && t.ProjectId == projectId && t.BoardId == boardId)
            .CountAsync(context.CancellationToken);

        var tasksQuery = dbContext.Tasks.AsQueryable();

        if (string.IsNullOrWhiteSpace(request.Search))
        {
            tasksQuery = tasksQuery.Where(t => t.TenantId == tenantId && t.ProjectId == projectId && t.BoardId == boardId)
                .Include(x => x.TaskTags)
                .Skip(request.Page * request.PageSize)
                .Take(request.PageSize);
        }
        else
        {
            tasksQuery = tasksQuery.Where(t => t.TenantId == tenantId && t.ProjectId == projectId && t.BoardId == boardId && 
                                       (EF.Functions.ILike(t.Title, $"%{request.Search}%") || 
                                        EF.Functions.ILike(t.DescriptionAsPlainText!, $"%{request.Search}%")))
                .Include(x => x.TaskTags)
                .Skip(request.Page * request.PageSize)
                .Take(request.PageSize);
        }
        
        var tasks = await tasksQuery.ToListAsync(context.CancellationToken);

        return new GetManyTasksByBoardIdResponse()
        {
            Tasks = { tasks.Select(t => t.ToDto()) },
            Page = request.Page,
            PageSize = request.PageSize,
            TotalCount = totalCount
        };
    }


    public override async Task<TaskDto> GetById(GetTaskByIdRequest request, ServerCallContext context)
    {
        var tenantId = Guid.Parse(request.TenantId);
        var projectId = Guid.Parse(request.ProjectId);
        var taskId = Guid.Parse(request.TaskId);
        var task = await dbContext.Tasks
            .Include(x => x.TaskTags)
            .FirstOrDefaultAsync(t => t.TenantId == tenantId && t.ProjectId == projectId && t.Id == taskId, context.CancellationToken);

        if (task == null)
        {
            throw new RpcException(new Status(StatusCode.NotFound, $"Task with ID {taskId} not found."));
        }

        return task.ToDto();
    }

    public override async Task<TaskDto> Create(CreateTaskRequest request, ServerCallContext context)
    {
        var userContext = (UserContext)httpContextAccessor.HttpContext?.Items["UserContext"]!;
        var taskId = Guid.NewGuid();
        var newTask = new Domain.Entities.TaskEntity
        {
            Id = taskId,
            Title = request.Title,
            Description = request.Description,
            BoardId = Guid.Parse(request.BoardId),
            BoardColumnId = Guid.Parse(request.BoardColumnId),
            ProjectId = Guid.Parse(request.ProjectId),
            TenantId = Guid.Parse(request.TenantId),
            CreatedByUserId = userContext.UserId,
            AssigneeUserId = string.IsNullOrEmpty(request.AssigneeUserId) ? null : Guid.Parse(request.AssigneeUserId),
            CreatedAt = DateTime.UtcNow,
            UpdatedAt = null,
            TaskTags = request.TagIds.Select(x => new TaskTagEntity()
            {
                TagId = Guid.Parse(x),
                TaskId = taskId,
            }).ToList()
        };
        
        dbContext.Tasks.Add(newTask);
        await dbContext.SaveChangesAsync(context.CancellationToken);

        return newTask.ToDto();
    }
    
    public override async Task<TaskDto> Update(UpdateTaskRequest request, ServerCallContext context) 
    {
        var tenantId = Guid.Parse(request.TenantId);
        var projectId = Guid.Parse(request.ProjectId);
        var taskId = Guid.Parse(request.TaskId);
        var task = await dbContext.Tasks
            .Include(x => x.TaskTags)
            .FirstOrDefaultAsync(t => t.TenantId == tenantId && t.ProjectId == projectId && t.Id == taskId, context.CancellationToken);

        if (task == null)
        {
            throw new RpcException(new Status(StatusCode.NotFound, $"Task with ID {taskId} not found."));
        }
        
        task.Title = request.Title;
        task.Description = request.Description;
        task.BoardId = Guid.Parse(request.BoardId);
        task.BoardColumnId = Guid.Parse(request.BoardColumnId);
        task.AssigneeUserId = string.IsNullOrEmpty(request.AssigneeUserId) ? null : Guid.Parse(request.AssigneeUserId);
        
        var deletedTagIds = task.TaskTags
            .Where(tt => !request.TagIds.Contains(tt.TagId.ToString()))
            .Select(tt => tt.TagId)
            .ToList();
        var newTagIds = request.TagIds
            .Where(tid => task.TaskTags.All(tt => tt.TagId.ToString() != tid))
            .Select(Guid.Parse)
            .ToList();
        
        task.TaskTags = task.TaskTags
            .Where(tt => !deletedTagIds.Contains(tt.TagId))
            .ToList();
       
        task.TaskTags.AddRange(newTagIds.Select(tid => new TaskTagEntity()
        {
            TagId = tid,
            TaskId = task.Id,
        }));

        await dbContext.SaveChangesAsync(context.CancellationToken);
        
        return task.ToDto();
    }

    public override async Task<Empty> Delete(DeleteTaskRequest request, ServerCallContext context)
    {
        var tenantId = Guid.Parse(request.TenantId);
        var projectId = Guid.Parse(request.ProjectId);
        var taskId = Guid.Parse(request.TaskId);
        var task = await dbContext.Tasks
            .FirstOrDefaultAsync(t => t.TenantId == tenantId && t.ProjectId == projectId && t.Id == taskId, context.CancellationToken);

        if (task == null)
        {
            throw new RpcException(new Status(StatusCode.NotFound, $"Task with ID {taskId} not found."));
        }

        dbContext.Tasks.Remove(task);
        await dbContext.SaveChangesAsync(context.CancellationToken);

        return new Empty();
    }
}