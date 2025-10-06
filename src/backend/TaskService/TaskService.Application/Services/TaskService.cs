﻿using Google.Protobuf.WellKnownTypes;
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
    public override async Task<GetManyTasksByBoardIdResponse> GetManyByBoardId(GetManyTasksByBoardIdRequest request, ServerCallContext context)
    {
        var tenantId = Guid.Parse(request.TenantId);
        var projectId = Guid.Parse(request.ProjectId);
        var boardId = Guid.Parse(request.BoardId);
        var tasks = await dbContext.Tasks
            .Where(t => t.TenantId == tenantId && t.ProjectId == projectId && t.BoardId == boardId)
            .Include(x => x.TaskTags)
            .ThenInclude(x => x.Tag)
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
            .Include(x => x.TaskTags)
            .ThenInclude(x => x.Tag)
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
            .ThenInclude(x => x.Tag)
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
        
        await dbContext.Entry(task)
            .Collection(t => t.TaskTags)
            .Query()
            .Include(tt => tt.Tag)
            .LoadAsync(context.CancellationToken);
        
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