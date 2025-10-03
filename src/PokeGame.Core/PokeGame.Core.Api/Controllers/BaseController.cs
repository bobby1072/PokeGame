using System.Net;
using Microsoft.AspNetCore.Mvc;
using PokeGame.Core.Common.Exceptions;
using PokeGame.Core.Domain.Services.User.Abstract;

namespace PokeGame.Core.Api.Controllers;

[ApiController]
[Route("Api/[controller]")]
public abstract class BaseController : ControllerBase
{
    protected virtual async Task<Schemas.Game.User> GetCurrentUser()
    {
        var userIdHeader = HttpContext.Request.Headers["UserId"];

        if (string.IsNullOrEmpty(userIdHeader) || !Guid.TryParse(userIdHeader, out var userId))
        {
            throw new PokeGameApiUserException(HttpStatusCode.BadRequest, "Invalid user id header");
        }
        var userService = HttpContext.RequestServices.GetRequiredService<IUserProcessingManager>();

        var foundUser = await userService.GetUserAsync(userId);
        
        return foundUser;
    }
}