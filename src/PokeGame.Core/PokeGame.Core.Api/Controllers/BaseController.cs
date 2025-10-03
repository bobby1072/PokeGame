using System.Net;
using Microsoft.AspNetCore.Mvc;
using PokeGame.Core.Common.Exceptions;
using PokeGame.Core.Domain.Services.User.Abstract;
using PokeGame.Core.Schemas.Game;

namespace PokeGame.Core.Api.Controllers;

[ApiController]
[Route("Api/[controller]")]
public abstract class BaseController : ControllerBase
{
    protected virtual Schemas.Game.User GetCurrentUser()
    {
        var userIdHeader = HttpContext.Request.Headers["UserId"];

        if (string.IsNullOrEmpty(userIdHeader) || !Guid.TryParse(userIdHeader, out var userId))
        {
            throw new PokeGameApiUserException(HttpStatusCode.BadRequest, "Invalid user id header");
        }

        if (!HttpContext.Items.TryGetValue(userIdHeader, out var foundUser))
        {
            throw new PokeGameApiUserException(HttpStatusCode.BadRequest, "Invalid user id");
        }
        
        return foundUser as User ?? throw new PokeGameApiServerException("Failed to parse user from items");
    }
}