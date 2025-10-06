using System.Net;
using Microsoft.AspNetCore.Mvc;
using PokeGame.Core.Common.Exceptions;
using PokeGame.Core.Common.Extensions;
using PokeGame.Core.Domain.Services.User.Abstract;
using PokeGame.Core.Schemas.Game;

namespace PokeGame.Core.Api.Controllers;

[ApiController]
[Route("Api/[controller]")]
public abstract class BaseController : ControllerBase
{
    protected async Task<User> GetCurrentUserAsync()
    {
        var userIdHeader = HttpContext.Request.Headers["UserId"];

        if (string.IsNullOrEmpty(userIdHeader) || !Guid.TryParse(userIdHeader, out var userId))
        {
            throw new PokeGameApiUserException(HttpStatusCode.BadRequest, "Invalid user id header");
        }

        if (!HttpContext.Items.TryGetValue(userIdHeader, out var foundUser))
        {
            var userService = HttpContext.RequestServices.GetRequiredService<IUserProcessingManager>();
            
            foundUser = await userService.GetUserAsync(userId);
            
            HttpContext.TryAddToItems(userIdHeader.ToString(), foundUser);
        }
        
        return foundUser as User ?? throw new PokeGameApiServerException("Failed to parse user from items");
    }
}