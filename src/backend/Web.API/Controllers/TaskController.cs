using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using TaskAggregatorService.Contracts;
using Web.API.Dtos.Task;

namespace Web.API.Controllers;

[ApiController]
[Route("api/v1/organizations/{organizationId:guid}/projects/{projectId:guid}/tasks")]
[Authorize]

public class TaskController(TaskAggregateService.TaskAggregateServiceClient taskAggregateServiceClient)
{
    [HttpGet("{taskId:guid}")]   
    public async Task<Dtos.Task.TaskDto> GetByIdAsync([FromRoute] Guid organizationId,
        [FromRoute] Guid projectId,
        [FromRoute] Guid taskId)
    {
        var response = await taskAggregateServiceClient.GetByIdAsync(new GetTaskByIdRequest()
        {
            TenantId = organizationId.ToString(),
            ProjectId = projectId.ToString(),
            TaskId = taskId.ToString()
        }).ResponseAsync;

        return response.ToHttp();
    }
    
    [HttpGet("by-board")]
    public async Task<Results.Task.GetManyTasksByBoardIdResponse> GetManyByBoardAsync([FromRoute] Guid organizationId,
        [FromRoute] Guid projectId,
        [FromQuery] Guid boardId)
    {
        var response = await taskAggregateServiceClient.GetManyByBoardIdAsync(new GetManyTasksByBoardIdRequest()
        {
            TenantId = organizationId.ToString(),
            ProjectId = projectId.ToString(),
            BoardId = boardId.ToString()
        }).ResponseAsync;
        
        return new Results.Task.GetManyTasksByBoardIdResponse()
        {
            Tasks = response.Tasks.Select(x => x.ToHttp()).ToList(),
        };
    }
    
    [HttpPost]
    public async Task<Dtos.Task.TaskDto> CreateAsync([FromRoute] Guid organizationId,
        [FromRoute] Guid projectId,
        [FromBody] CreateTaskRequest request)
    {
        var response = await taskAggregateServiceClient.CreateAsync(new CreateTaskRequest()
        {
            TenantId = organizationId.ToString(),
            ProjectId = projectId.ToString(),
            BoardId = request.BoardId,
            Title = request.Title,
            Description = request.Description,
            BoardColumnId = request.BoardColumnId,
            AssigneeUserId = request.AssigneeUserId,
        }).ResponseAsync;

        return response.ToHttp();
    }

    [HttpPut("{taskId:guid}")]
    public async Task<Dtos.Task.TaskDto> UpdateAsync([FromRoute] Guid organizationId,
        [FromRoute] Guid projectId,
        [FromRoute] Guid taskId,
        [FromBody] UpdateTaskRequest request)
    {
        var response = await taskAggregateServiceClient.UpdateAsync(new UpdateTaskRequest()
        {
            TenantId = organizationId.ToString(),
            ProjectId = projectId.ToString(),
            TaskId = taskId.ToString(),
            Title = request.Title,
            Description = request.Description,
            BoardColumnId = request.BoardColumnId,
            AssigneeUserId = request.AssigneeUserId,
        }).ResponseAsync;

        return response.ToHttp();
    }

    [HttpDelete("{taskId:guid}")]
    public async Task DeleteAsync([FromRoute] Guid organizationId,
        [FromRoute] Guid projectId,
        [FromRoute] Guid taskId)
    {
        await taskAggregateServiceClient.DeleteAsync(new DeleteTaskRequest()
        {
            TenantId = organizationId.ToString(),
            ProjectId = projectId.ToString(),
            TaskId = taskId.ToString()
        });
    }
}