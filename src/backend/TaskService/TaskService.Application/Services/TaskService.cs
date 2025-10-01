using Google.Protobuf.WellKnownTypes;
using Grpc.Core;
using Microsoft.AspNetCore.Http;
using Microsoft.EntityFrameworkCore;
using TaskService.Application.Mappings;
using TaskService.Contracts;
using TaskService.Infrastructure;
using TaskService.Infrastructure.Data;

namespace TaskService.Application.Services;

public class TaskService(ApplicationDbContext dbContext, IHttpContextAccessor httpContextAccessor) : Contracts.TaskService.TaskServiceBase
{
    public override async Task<GetManyTasksByBoardIdResponse> GetManyByBoardId(GetManyTasksByBoardIdRequest request, ServerCallContext context)
    {
        var tenantId = Guid.Parse(request.TenantId);
        var projectId = Guid.Parse(request.ProjectId);
        var boardId = Guid.Parse(request.BoardId);
        var tasks = await dbContext.Tasks
            .Where(t => t.TenantId == tenantId && t.ProjectId == projectId && t.BoardId == boardId)
            .ToListAsync(context.CancellationToken);

        return new GetManyTasksByBoardIdResponse()
        {
            Tasks = { tasks.Select(t => t.ToDto()) }
        };
    }

    public override async Task<TaskDto> GetById(GetTaskByIdRequest request, ServerCallContext context)
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

        return task.ToDto();
    }

    public override async Task<TaskDto> Create(CreateTaskRequest request, ServerCallContext context)
    {
        var userContext = (UserContext)httpContextAccessor.HttpContext?.Items["UserContext"]!;
        var newTask = new Domain.Entities.TaskEntity
        {
            Id = Guid.NewGuid(),
            Title = request.Title,
            Description = request.Description,
            BoardId = Guid.Parse(request.BoardId),
            BoardColumnId = Guid.Parse(request.BoardColumnId),
            CreatedByUserId = userContext.UserId,
            AssigneeUserId = string.IsNullOrEmpty(request.AssigneeUserId) ? null : Guid.Parse(request.AssigneeUserId),
            CreatedAt = DateTime.UtcNow,
            UpdatedAt = null
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

        dbContext.Tasks.Update(task);
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