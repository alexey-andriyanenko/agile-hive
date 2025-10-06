using System.Security.Cryptography.Xml;
using BoardService.Contracts;
using Google.Protobuf.WellKnownTypes;
using Grpc.Core;
using ProjectUserService.Contracts;
using TaskAggregatorService.Application.Mappings;
using TaskAggregatorService.Contracts;

namespace TaskAggregatorService.Application.Services;

public class TaskAggregatorService(
    TaskService.Contracts.TagService.TagServiceClient tagServiceClient,
    TaskService.Contracts.TaskService.TaskServiceClient taskServiceClient,
    BoardColumnService.BoardColumnServiceClient boardColumnServiceClient,
    ProjectUserService.Contracts.ProjectUserService.ProjectUserServiceClient projectUserServiceClient
) : TaskAggregateService.TaskAggregateServiceBase
{
    public override async Task<GetTagsByProjectIdResponse> GetTagsByProjectId(GetTagsByProjectIdRequest request, ServerCallContext context)
    {
        var tags = await tagServiceClient.GetManyByProjectIdAsync(
            new TaskService.Contracts.GetManyTagsByProjectIdRequest
            {
                TenantId = request.TenantId,
                ProjectId = request.ProjectId
            }).ResponseAsync;

        return new GetTagsByProjectIdResponse()
        {
            Tags = { tags.Tags.Select(t => t.ToDto()) }
        };
    }

    public override async Task<GetManyTasksByBoardIdResponse> GetManyByBoardId(GetManyTasksByBoardIdRequest request,
        ServerCallContext context)
    {
        var boardColumnsResponse = await boardColumnServiceClient.GetManyByBoardIdAsync(
            new GetManyBoardColumnsByBoardIdRequest { BoardId = request.BoardId }).ResponseAsync;
        var boardColumnsById = boardColumnsResponse.BoardColumns.ToDictionary(c => c.Id, c => c);
    
        var tasksResponse = await taskServiceClient.GetManyByBoardIdAsync(
            new TaskService.Contracts.GetManyTasksByBoardIdRequest
            {
                TenantId = request.TenantId,
                ProjectId = request.ProjectId,
                BoardId = request.BoardId
            }).ResponseAsync;
    
        var assigneeIds = tasksResponse.Tasks
            .Select(t => t.AssigneeUserId)
            .Where(id => !string.IsNullOrEmpty(id))
            .Distinct()
            .ToList();
        var creatorIds = tasksResponse.Tasks
            .Select(t => t.CreatedByUserId)
            .Where(id => !string.IsNullOrEmpty(id))
            .Distinct()
            .ToList();
        var userIds = assigneeIds.Union(creatorIds).Distinct().ToList();
    
        var projectUsersResponse = await projectUserServiceClient.GetManyByIdsAsync(
            new ProjectUserService.Contracts.GetManyProjectUsersByIdsRequest
            {
                ProjectId = request.ProjectId,
                UserIds = { userIds }
            }).ResponseAsync;
        var projectUsersById = projectUsersResponse.Users.ToDictionary(u => u.Id, u => u);
    
        var result = new GetManyTasksByBoardIdResponse
        {
            Tasks =
            {
                tasksResponse.Tasks
                    .Select(task => MapTaskAggregateDto(task, boardColumnsById, projectUsersById, request.ProjectId))
                    .ToList()
            }
        };

        return result;
    }
    
    public override async Task<TaskAggregateDto> GetById(GetTaskByIdRequest request, ServerCallContext context)
    {
        var taskResponse = await taskServiceClient.GetByIdAsync(
            new TaskService.Contracts.GetTaskByIdRequest
            {
                TenantId = request.TenantId,
                ProjectId = request.ProjectId,
                TaskId = request.TaskId,
                BoardId = request.BoardId
            }).ResponseAsync;

        return await BuildTaskAggregateDto(taskResponse, request.ProjectId);
    }

    public override async Task<TaskAggregateDto> Create(CreateTaskRequest request, ServerCallContext context)
    {
        var taskResponse = await taskServiceClient.CreateAsync(
            new TaskService.Contracts.CreateTaskRequest
            {
                TenantId = request.TenantId,
                ProjectId = request.ProjectId,
                Title = request.Title,
                Description = request.Description,
                BoardId = request.BoardId,
                BoardColumnId = request.BoardColumnId,
                AssigneeUserId = request.AssigneeUserId,
                TagIds = { request.TagIds }
            }).ResponseAsync;

        return await BuildTaskAggregateDto(taskResponse, request.ProjectId);
    }

    public override async Task<TaskAggregateDto> Update(UpdateTaskRequest request, ServerCallContext context)
    {
        var taskResponse = await taskServiceClient.UpdateAsync(
            new TaskService.Contracts.UpdateTaskRequest
            {
                TenantId = request.TenantId,
                ProjectId = request.ProjectId,
                BoardId = request.BoardId,
                TaskId = request.TaskId,
                Title = request.Title,
                Description = request.Description,
                BoardColumnId = request.BoardColumnId,
                AssigneeUserId = request.AssigneeUserId,
                TagIds = { request.TagIds }
            }).ResponseAsync;

        return await BuildTaskAggregateDto(taskResponse, request.ProjectId);
    }

    public override async Task<Empty> Delete(DeleteTaskRequest request, ServerCallContext context)
    {
        await taskServiceClient.DeleteAsync(
            new TaskService.Contracts.DeleteTaskRequest
            {
                TenantId = request.TenantId,
                ProjectId = request.ProjectId,
                TaskId = request.TaskId
            });

        return new Empty();
    }

    private async Task<TaskAggregateDto> BuildTaskAggregateDto(
        TaskService.Contracts.TaskDto taskResponse,
        string projectId)
    {
        var boardColumnResponse = await boardColumnServiceClient.GetByIdAsync(
            new GetBoardColumnByIdRequest
            {
                BoardId = taskResponse.BoardId,
                BoardColumnId = taskResponse.BoardColumnId
            }).ResponseAsync;

        var userIds = new List<string> { taskResponse.CreatedByUserId };
        if (!string.IsNullOrEmpty(taskResponse.AssigneeUserId))
            userIds.Add(taskResponse.AssigneeUserId);

        var projectUsersResponse = await projectUserServiceClient.GetManyByIdsAsync(
            new ProjectUserService.Contracts.GetManyProjectUsersByIdsRequest
            {
                ProjectId = projectId,
                UserIds = { userIds }
            }).ResponseAsync;

        var projectUsersById = projectUsersResponse.Users.ToDictionary(u => u.Id, u => u);

        var creatorUser = projectUsersById.TryGetValue(taskResponse.CreatedByUserId, out var cu)
            ? cu
            : throw new RpcException(new Status(StatusCode.NotFound,
                $"Creator user with ID '{taskResponse.CreatedByUserId}' not found in project with ID '{projectId}'."));

        var assigneeUser = !string.IsNullOrEmpty(taskResponse.AssigneeUserId) &&
                           projectUsersById.TryGetValue(taskResponse.AssigneeUserId, out var au)
            ? au
            : null;

        return taskResponse.ToDto(boardColumnResponse, creatorUser, assigneeUser);
    }
    
    private TaskAggregateDto MapTaskAggregateDto(
        TaskService.Contracts.TaskDto task,
        IDictionary<string, BoardColumnDto> boardColumnsById,
        IDictionary<string, ProjectUserDto> projectUsersById,
        string projectId)
    {
        var creatorUser = projectUsersById.TryGetValue(task.CreatedByUserId, out var cu)
            ? cu
            : throw new RpcException(new Status(StatusCode.NotFound,
                $"Creator user with ID '{task.CreatedByUserId}' not found in project with ID '{projectId}'."));
    
        var assigneeUser = !string.IsNullOrEmpty(task.AssigneeUserId) &&
                           projectUsersById.TryGetValue(task.AssigneeUserId, out var au)
            ? au
            : null;
    
        var boardColumn = !string.IsNullOrEmpty(task.BoardColumnId) &&
                          boardColumnsById.TryGetValue(task.BoardColumnId, out var bc)
            ? bc
            : null;
    
        return task.ToDto(boardColumn!, creatorUser, assigneeUser);
    }
}