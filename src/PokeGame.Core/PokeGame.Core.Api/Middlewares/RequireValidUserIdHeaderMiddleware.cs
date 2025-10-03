using System.Net;
using PokeGame.Core.Api.Attributes;
using PokeGame.Core.Common.Exceptions;
using PokeGame.Core.Domain.Services.User.Abstract;

namespace PokeGame.Core.Api.Middlewares;

public sealed class RequireValidUserIdHeaderMiddleware
{
    private readonly RequestDelegate _next;

    public RequireValidUserIdHeaderMiddleware(RequestDelegate next)
    {
        _next = next;
    }

    public async Task Invoke(HttpContext context, 
        ILogger<RequireValidUserIdHeaderMiddleware> logger)
    {
        var endpointMetadata = context.GetEndpoint()?.Metadata;

        if (endpointMetadata?.GetMetadata<RequireValidUserIdHeaderAttribute>() is not null)
        {
            var userIdHeader = context.Request.Headers["UserId"];

            if (string.IsNullOrEmpty(userIdHeader) || !Guid.TryParse(userIdHeader, out var userId))
            {
                throw new PokeGameApiUserException(HttpStatusCode.BadRequest, "Invalid user id header");
            }
            var userService = context.RequestServices.GetRequiredService<IUserProcessingManager>();

            var foundUser = await userService.GetUserAsync(userId);

            if (context.Items.TryAdd(userIdHeader, foundUser))
            {
                logger.LogInformation("Added user with id: {UserId} to request items", foundUser.Id);
            }
            else
            {
                logger.LogWarning("Failed to add user with id: {USerId} to request items", foundUser.Id);
            }
        }
        await _next.Invoke(context);
    }
}