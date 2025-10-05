using Microsoft.AspNetCore.SignalR;

namespace PokeGame.Core.SignalR.Hubs;

public sealed class PokeGameSessionHub: Hub
{
    private readonly ILogger<PokeGameSessionHub> _logger;

    public PokeGameSessionHub(ILogger<PokeGameSessionHub> logger)
    {
        _logger = logger;
    }
    
    public override async Task OnConnectedAsync()
    {
        await base.OnConnectedAsync();
    }
}