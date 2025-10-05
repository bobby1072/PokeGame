using Microsoft.AspNetCore.SignalR;
using PokeGame.Core.Common;
using PokeGame.Core.Common.Extensions;
using PokeGame.Core.Domain.Services.Game.Abstract;
using PokeGame.Core.Domain.Services.User.Abstract;

namespace PokeGame.Core.SignalR.Hubs;

public sealed class PokeGameSessionHub: Hub
{
    private readonly IServiceProvider _serviceProvider;
    private readonly ILogger<PokeGameSessionHub> _logger;

    public PokeGameSessionHub(IServiceProvider serviceProvider, ILogger<PokeGameSessionHub> logger)
    {
        _serviceProvider = serviceProvider;
        _logger = logger;
    }
    
    public override async Task OnConnectedAsync()
    {
        await base.OnConnectedAsync();
        
        var httpContext = Context.GetHttpContext();

        var foundGameSaveIdQueryString = httpContext?
            .GetStringFromRequestQuery(Constants.ApiConstants.GameSaveIdHeaderKey);

        if (string.IsNullOrWhiteSpace(foundGameSaveIdQueryString) ||
            !Guid.TryParse(foundGameSaveIdQueryString, out var foundGameSaveId))
        {
            Context.Abort();
            return;
        }

        var foundUserIdQueryString = httpContext?
            .GetStringFromRequestQuery(Constants.ApiConstants.UserIdHeaderKey);

        if (string.IsNullOrWhiteSpace(foundUserIdQueryString) || 
            !Guid.TryParse(foundUserIdQueryString, out var userId))
        {
            Context.Abort();
            return;
        }
        
        var userManager = _serviceProvider.GetRequiredService<IUserProcessingManager>();
        var foundUser = await userManager.GetUserAsync(userId);
        
        
        var gameSessionManager = _serviceProvider.GetRequiredService<IGameSessionProcessingManager>();
        var newGameSession = await gameSessionManager.StartGameSession(foundGameSaveId, foundUser);
        
        
    }
}