using Google.Protobuf.WellKnownTypes;
using IdentityService.Contracts;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace Web.API.Controllers;

[ApiController]
[Authorize]
[Route("api/v1/user")]
public class UserController(UserService.UserServiceClient userClient)
{
    [HttpGet("me")]
    public async Task<ActionResult<UserDto>> GetMeAsync()
    {
        return await userClient.GetMeAsync(new Empty());
    }

    [HttpGet("{userId}")]
    public async Task<ActionResult<UserDto>> GetByIdAsync([FromRoute] string userId)
    {
        return await userClient.GetByIdAsync(new GetUserByIdRequest
        {
            UserId = userId
        });
    }

    [HttpGet]
    public async Task<ActionResult<GetManyUsersByIdsResponse>> GetManyByIdsAsync(
        [FromQuery] List<string> userIds)
    {
        return await userClient.GetManyByIdsAsync(new GetManyUsersByIdsRequest()
        {
            UserIds = { userIds }
        });
    }
}