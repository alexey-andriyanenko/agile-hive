using BoardService.Contracts;
using Google.Protobuf.WellKnownTypes;
using Grpc.Core;
using ProjectUserService.Contracts;
using Tag.Contracts;
using TaskAggregatorService.Application.Mappings;
using TaskAggregatorService.Contracts;

namespace TaskAggregatorService.Application.Services;

public class TaskAggregatorService(
    TagService.TagServiceClient tagServiceClient,
    TaskService.Contracts.TaskService.TaskServiceClient taskServiceClient,
    BoardColumnService.BoardColumnServiceClient boardColumnServiceClient,
    ProjectUserService.Contracts.ProjectUserService.ProjectUserServiceClient projectUserServiceClient
) : TaskAggregateService.TaskAggregateServiceBase
{
    public override async Task<GenerateCsvResponse> GenerateCsv(Empty request, ServerCallContext context)
    {
        var result = await taskServiceClient.GenerateCsvAsync(new Google.Protobuf.WellKnownTypes.Empty()).ResponseAsync;

        return new GenerateCsvResponse()
        {
            FileContent = result.FileContent,
            FileName = result.FileName
        };
    }

    public override async Task<GetManyByTenantIdResponse> GetManyByTenantId(GetManyByTenantIdRequest request, ServerCallContext context)
    {
        var result = await taskServiceClient.GetManyByTenantIdAsync(
            new TaskService.Contracts.GetManyTasksByTenantIdRequest
            {
                TenantId = request.TenantId,
            }).ResponseAsync;

        return new GetManyByTenantIdResponse()
        {
            TotalCount = result.TotalCount
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
                BoardId = request.BoardId,
                Page = request.Page,
                PageSize = request.PageSize,
                Search = request.Search
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
    
        var tagIds = tasksResponse.Tasks
            .SelectMany(t => t.TagIds)
            .Distinct()
            .ToList();
        var tagsResponse = await tagServiceClient.GetManyByIdsAsync(
            new GetManyTagsByIdsRequest
            {
                TenantId = request.TenantId,
                ProjectId = request.ProjectId,
                TagIds = { tagIds }
            }).ResponseAsync;
        var tagsById = tagsResponse.Tags.ToDictionary(t => t.Id, t => t);
        
        var result = new GetManyTasksByBoardIdResponse
        {
            Tasks =
            {
                tasksResponse.Tasks
                    .Select(task => MapTaskAggregateDto(task, boardColumnsById, projectUsersById, tagsById, request.ProjectId))
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

        return await BuildTaskAggregateDto(taskResponse, request.TenantId, request.ProjectId);
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

        return await BuildTaskAggregateDto(taskResponse, request.TenantId, request.ProjectId);
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

        return await BuildTaskAggregateDto(taskResponse, request.TenantId, request.ProjectId);
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
        string tenantId,
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
        
        var tagsResponse = await tagServiceClient.GetManyByIdsAsync(
            new GetManyTagsByIdsRequest
            {
                TenantId = tenantId,
                ProjectId = projectId,
                TagIds = { taskResponse.TagIds }
            }).ResponseAsync;

        return taskResponse.ToDto(boardColumnResponse, creatorUser, tagsResponse.Tags, assigneeUser);
    }
    
    private TaskAggregateDto MapTaskAggregateDto(
        TaskService.Contracts.TaskDto task,
        IDictionary<string, BoardColumnDto> boardColumnsById,
        IDictionary<string, ProjectUserDto> projectUsersById,
        IDictionary<string, TagDto> tagsByIds,
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
        
        var tags = task.TagIds
            .Where(tid => tagsByIds.TryGetValue(tid, out var tag))
            .Select(tid => tagsByIds[tid])
            .ToList();
    
        return task.ToDto(boardColumn!, creatorUser, tags, assigneeUser);
    }
}