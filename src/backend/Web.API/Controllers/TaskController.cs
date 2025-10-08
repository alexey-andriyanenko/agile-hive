using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using TaskAggregatorService.Contracts;
using Web.API.Dtos.Task;
using Web.API.Parameters.Task;

namespace Web.API.Controllers;

[ApiController]
[Route("api/v1/organizations/{organizationId:guid}/projects/{projectId:guid}/tasks")]
[Authorize]
public class TaskController(TaskAggregateService.TaskAggregateServiceClient taskAggregateServiceClient)
{
    [HttpGet("{taskId:guid}")]
    public async Task<Dtos.Task.TaskDto> GetByIdAsync([FromRoute] Guid organizationId,
        [FromRoute] Guid projectId,
        [FromRoute] Guid taskId,
        [FromQuery] Guid boardId)
    {
        var response = await taskAggregateServiceClient.GetByIdAsync(new GetTaskByIdRequest()
        {
            BoardId = boardId.ToString(),
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
        [FromBody] CreateTaskParameters parameters)
    {
        parameters.TenantId = organizationId;
        parameters.ProjectId = projectId;
        
        var response = await taskAggregateServiceClient.CreateAsync(parameters.ToGrpcRequest()).ResponseAsync;

        return response.ToHttp();
    }

    [HttpPut("{taskId:guid}")]
    public async Task<Dtos.Task.TaskDto> UpdateAsync([FromRoute] Guid organizationId,
        [FromRoute] Guid projectId,
        [FromRoute] Guid taskId,
        [FromBody] UpdateTaskParameters parameters)
    {
        parameters.TenantId = organizationId;
        parameters.ProjectId = projectId;
        parameters.TaskId = taskId;
        
        var response = await taskAggregateServiceClient.UpdateAsync(parameters.ToGrpcRequest()).ResponseAsync;

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