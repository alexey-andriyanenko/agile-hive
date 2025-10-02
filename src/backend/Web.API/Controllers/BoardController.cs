using BoardService.Contracts;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Web.API.Dtos.Board;
using Web.API.Parameters.Board;
using Web.API.Results.Board;

namespace Web.API.Controllers;

[ApiController]
[Route("api/v1/organizations/{organizationId:guid}/projects/{projectId:guid}/boards")]
[Authorize]
public class BoardController(BoardService.Contracts.BoardService.BoardServiceClient boardServiceClient) : ControllerBase
{
    [HttpGet("{boardId:guid}")]
    public async Task<Dtos.Board.BoardDto> GetByIdAsync([FromRoute] Guid organizationId,
        [FromRoute] Guid projectId,
        [FromRoute] Guid boardId)
    {
        var response = await boardServiceClient.GetByIdAsync(new GetBoardByIdRequest()
        {
            ProjectId = projectId.ToString(),
            BoardId = boardId.ToString()
        }).ResponseAsync;

        return response.ToHttp();
    }
    
    [HttpGet]
    public async Task<GetManyBoardsResult> GetManyAsync([FromRoute] Guid organizationId,
        [FromRoute] Guid projectId)
    {
        var response = await boardServiceClient.GetManyAsync(new GetManyBoardsRequest()
        {
            ProjectId = projectId.ToString(),
        }).ResponseAsync;
        
        return new GetManyBoardsResult()
        {
            Boards = response.Boards.Select(x => x.ToHttp()).ToList(),
        };
    }
    
    [HttpPost]
    public async Task<Dtos.Board.BoardDto> CreateAsync([FromRoute] Guid organizationId,
        [FromRoute] Guid projectId,
        [FromBody] Parameters.Board.CreateBoardParameters parameters)
    {
        parameters.TenantId = organizationId;
        parameters.ProjectId = projectId;
        
        var response = await boardServiceClient.CreateAsync(parameters.ToGrpc()).ResponseAsync;

        return response.ToHttp();
    }

    [HttpPut("{boardId:guid}")]
    public async Task<Dtos.Board.BoardDto> UpdateAsync([FromRoute] Guid organizationId,
        [FromRoute] Guid projectId,
        [FromRoute] Guid boardId,
        [FromBody] Parameters.Board.UpdateBoardParameters parameters)
    {
        parameters.TenantId = organizationId;
        parameters.ProjectId = projectId;
        parameters.BoardId = boardId;
        
        var response = await boardServiceClient.UpdateAsync(parameters.ToGrpc()).ResponseAsync;

        return response.ToHttp();
    }
    
    [HttpDelete("{boardId:guid}")]
    public async Task DeleteAsync([FromRoute] Guid organizationId,
        [FromRoute] Guid projectId,
        [FromRoute] Guid boardId)
    {
        await boardServiceClient.DeleteAsync(new DeleteBoardRequest()
        {
            ProjectId = projectId.ToString(),
            BoardId = boardId.ToString(),
        });
    }

    [HttpGet("types")]
    public async Task<GetManyBoardTypesResult> GetManyTypesAsync([FromRoute] Guid organizationId,
        [FromRoute] Guid projectId)
    {
        var response = await boardServiceClient.GetManyTypesAsync(new GetManyBoardTypesRequest()
        {
            TenantId = organizationId.ToString(),
            ProjectId = projectId.ToString(),
        }).ResponseAsync;
        
        return new GetManyBoardTypesResult()
        {
            BoardTypes = response.BoardTypes.Select(x => x.ToHttp()).ToList(),
        };
    }
}