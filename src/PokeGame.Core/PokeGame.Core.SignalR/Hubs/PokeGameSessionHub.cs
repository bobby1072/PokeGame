using Microsoft.AspNetCore.SignalR;
using PokeGame.Core.Common;
using PokeGame.Core.Common.Exceptions;
using PokeGame.Core.Common.Extensions;
using PokeGame.Core.Domain.Services.Game.Abstract;
using PokeGame.Core.Domain.Services.User.Abstract;
using PokeGame.Core.Schemas.Game;
using PokeGame.Core.SignalR.Models;

namespace PokeGame.Core.SignalR.Hubs;

public sealed class PokeGameSessionHub : Hub
{
    private readonly IServiceProvider _serviceProvider;
    private readonly ILogger<PokeGameSessionHub> _logger;
    private const string EventKey = "EventKey";
    public PokeGameSessionHub(IServiceProvider serviceProvider, ILogger<PokeGameSessionHub> logger)
    {
        _serviceProvider = serviceProvider;
        _logger = logger;
    }
    #region  Hub Methods

    [HubMethodName(nameof(GetDeepOwnedPokemonInDeck))]
    public async Task GetDeepOwnedPokemonInDeck(CancellationToken ct = default)
    {
        try
        {
            var httpContext = Context.GetHttpContext();
            
            var foundUserIdQueryString = httpContext?.GetStringFromRequestQuery(
                Constants.ApiConstants.UserIdHeaderKey
            );

            if (
                string.IsNullOrWhiteSpace(foundUserIdQueryString)
                || !Guid.TryParse(foundUserIdQueryString, out var userId)
            )
            {
                _logger.LogInformation(
                    "Invalid user id included with connection request"
                );

                await Clients.Caller.SendAsync(
                    EventKeys.GetDeepOwnedPokemonInDeckFailed,
                    new SignalRClientEvent
                    {
                        ExceptionMessage = "Invalid user id attached to request query",
                        ExtraData = new Dictionary<string, object>
                        {
                            { EventKey, EventKeys.GetDeepOwnedPokemonInDeckFailed },
                        },
                    },
                    ct
                );

                return;
            }
            
            var userManager = _serviceProvider.GetRequiredService<IUserProcessingManager>();
            var foundUser = await userManager.GetUserAsync(userId);
            
            var gameSessionProcessingManager = _serviceProvider.GetRequiredService<IGameSessionProcessingManager>();

            var pokemonInDeck =
                await gameSessionProcessingManager.GetDeepOwnedPokemonInDeck(Context.ConnectionId, foundUser, ct);


            foreach (var poke in pokemonInDeck)
            {
                await Clients.Caller.SendAsync(
                    EventKeys.SingleDeepOwnedPokemonFromDeck,
                    new SignalRClientEvent<OwnedPokemon>
                    {
                        Data = poke,
                        ExtraData = new Dictionary<string, object> { { EventKey, EventKeys.SingleDeepOwnedPokemonFromDeck } }
                    },
                    ct
                );
            }
        }
        catch (PokeGameApiUserException ex)
        {
            _logger.LogInformation(ex, "Poke game user exception occurred during get deep owned pokemon");
            await Clients.Caller.SendAsync(
                EventKeys.GetDeepOwnedPokemonInDeckFailed,
                new SignalRClientEvent
                {
                    ExceptionMessage = $"Failed to get owned pokemon in deck. {ex.Message}",
                    ExtraData = new Dictionary<string, object> { { EventKey, EventKeys.GetDeepOwnedPokemonInDeckFailed } },
                },
                ct
            );   
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Unexpected exception occurred during game save...");
            await Clients.Caller.SendAsync(
                EventKeys.GetDeepOwnedPokemonInDeckFailed,
                new SignalRClientEvent
                {
                    ExceptionMessage = "Failed to get owned pokemon in deck",
                    ExtraData = new Dictionary<string, object> { { EventKey, EventKeys.GetDeepOwnedPokemonInDeckFailed } },
                },
                ct
            ); 
        }
    }
    [HubMethodName(nameof(SaveGame))]
    public async Task SaveGame(GameSaveData gameSaveData)
    {
        try
        {
            var httpContext = Context.GetHttpContext();
            
            var foundUserIdQueryString = httpContext?.GetStringFromRequestQuery(
                Constants.ApiConstants.UserIdHeaderKey
            );

            if (
                string.IsNullOrWhiteSpace(foundUserIdQueryString)
                || !Guid.TryParse(foundUserIdQueryString, out var userId)
            )
            {
                _logger.LogInformation(
                    "Invalid user id included with connection request"
                );

                await Clients.Caller.SendAsync(
                    EventKeys.GameSaveFailed,
                    new SignalRClientEvent
                    {
                        ExceptionMessage = "Invalid user id attached to request query",
                        ExtraData = new Dictionary<string, object>
                        {
                            { EventKey, EventKeys.GameSaveFailed },
                        },
                    }
                );

                return;
            }

            var userManager = _serviceProvider.GetRequiredService<IUserProcessingManager>();
            var foundUser = await userManager.GetUserAsync(userId);
            
            var gameSaveProcessingManager = _serviceProvider.GetRequiredService<IGameSaveProcessingManager>();
            await gameSaveProcessingManager.SaveGameDataAsync(gameSaveData, Context.ConnectionId, foundUser);
        }
        catch (PokeGameApiUserException ex)
        {
            _logger.LogInformation(ex, "Poke game user exception occurred during game save");
            await Clients.Caller.SendAsync(
                EventKeys.GameSaveFailed,
                new SignalRClientEvent
                {
                    ExceptionMessage = $"Failed to save game. {ex.Message}",
                    ExtraData = new Dictionary<string, object> { { EventKey, EventKeys.GameSaveFailed } },
                }
            );   
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Unexpected exception occurred during game save...");
            await Clients.Caller.SendAsync(
                EventKeys.GameSaveFailed,
                new SignalRClientEvent
                {
                    ExceptionMessage = "Failed to save game",
                    ExtraData = new Dictionary<string, object> { { EventKey, EventKeys.GameSaveFailed } },
                }
            ); 
        }
    }
    #endregion
    #region Connect & Disconnect
    public override async Task OnConnectedAsync()
    {
        await base.OnConnectedAsync();
        try
        {
            var httpContext = Context.GetHttpContext();

            var foundGameSaveIdQueryString = httpContext?.GetStringFromRequestQuery(
                Constants.ApiConstants.GameSaveIdHeaderKey
            );

            if (
                string.IsNullOrWhiteSpace(foundGameSaveIdQueryString)
                || !Guid.TryParse(foundGameSaveIdQueryString, out var foundGameSaveId)
            )
            {
                _logger.LogInformation(
                    "Invalid game save id included with connection request, aborting Signal R connection..."
                );

                await Clients.Caller.SendAsync(
                    EventKeys.GameSessionConnectionFailed,
                    new SignalRClientEvent
                    {
                        ExceptionMessage = "Invalid game save id attached to request query",
                        ExtraData = new Dictionary<string, object>
                        {
                            { EventKey, EventKeys.GameSessionConnectionFailed },
                        },
                    }
                );

                Context.Abort();
                return;
            }

            var foundUserIdQueryString = httpContext?.GetStringFromRequestQuery(
                Constants.ApiConstants.UserIdHeaderKey
            );

            if (
                string.IsNullOrWhiteSpace(foundUserIdQueryString)
                || !Guid.TryParse(foundUserIdQueryString, out var userId)
            )
            {
                _logger.LogInformation(
                    "Invalid user id included with connection request, aborting Signal R connection for connectionId: {ConnectionId}...",
                    Context.ConnectionId
                );

                await Clients.Caller.SendAsync(
                    EventKeys.GameSessionConnectionFailed,
                    new SignalRClientEvent
                    {
                        ExceptionMessage = "Invalid user id attached to request query",
                        ExtraData = new Dictionary<string, object>
                        {
                            { EventKey, EventKeys.GameSessionConnectionFailed },
                        },
                    }
                );

                Context.Abort();
                return;
            }

            var userManager = _serviceProvider.GetRequiredService<IUserProcessingManager>();
            var foundUser = await userManager.GetUserAsync(userId);

            var gameSessionManager =
                _serviceProvider.GetRequiredService<IGameSessionProcessingManager>();
            var newGameSession = await gameSessionManager.StartGameSession(
                foundGameSaveId,
                Context.ConnectionId,
                foundUser
            );

            _logger.LogInformation(
                "User with id: {UserId} successfully started a new game session with id: {GameSessionId} for connectionId: {ConnectionId}",
                userId,
                newGameSession.Id,
                Context.ConnectionId
            );

            await Clients.Caller.SendAsync(
                EventKeys.GameSessionStarted,
                new SignalRClientEvent<GameSession>
                {
                    Data = newGameSession,
                    ExtraData = new Dictionary<string, object>
                    {
                        { EventKey, EventKeys.GameSessionStarted },
                    },
                }
            );
        }
        catch (Exception ex)
        {
            await HandleSignalRException(ex, EventKeys.GameSessionConnectionFailed);
            Context.Abort();
        }
    }

    public override async Task OnDisconnectedAsync(Exception? exception)
    {
        await base.OnDisconnectedAsync(exception);
        try
        {
            if (exception is not null)
            {
                _logger.LogError(
                    exception,
                    "Unhandled exception is causing Signal R to abort connection for connectionId: {ConnectionId}",
                    Context.ConnectionId
                );
            }

            var gameSessionManager =
                _serviceProvider.GetRequiredService<IGameSessionProcessingManager>();

            await gameSessionManager.EndGameSession(Context.ConnectionId);
        }
        catch (Exception ex)
        {
            _logger.LogError(
                ex,
                "Exception occurred during Signal R disconnect attempt for connectionId: {ConnectionId}",
                Context.ConnectionId
            );
        }
    }
    #endregion
    #region Private Methods And Definitions
    private async Task HandleSignalRException(Exception exception, string eventKey)
    {
        if (exception is PokeGameApiUserException pokeGameApiUserException)
        {
            _logger.LogInformation(
                pokeGameApiUserException,
                "Poke game user exception occurred during Signal R invocation for connectionId: {ConnectionId}",
                Context.ConnectionId
            );
            await Clients.Caller.SendAsync(
                eventKey,
                new SignalRClientEvent
                {
                    ExceptionMessage = pokeGameApiUserException.Message,
                    ExtraData = new Dictionary<string, object> { { EventKey, eventKey } },
                }
            );
        }
        else if (exception is PokeGameApiServerException pokeGameApiServerException)
        {
            _logger.LogError(
                pokeGameApiServerException,
                "Poke game server exception occurred during Signal R invocation for connectionId: {ConnectionId}",
                Context.ConnectionId
            );
            await Clients.Caller.SendAsync(
                eventKey,
                new SignalRClientEvent
                {
                    ExceptionMessage = Constants.ExceptionConstants.InternalError,
                    ExtraData = new Dictionary<string, object> { { EventKey, eventKey } },
                }
            );
        }
    }

    private static class EventKeys
    {
        public const string GameSessionStarted = "GameSessionStarted";
        public const string GameSessionConnectionFailed = "GameSessionConnectionFailed";
        public const string GameSaveFailed = "GameSaveFailed";
        public const string GetDeepOwnedPokemonInDeckFailed = "GetDeepOwnedPokemonInDeckFailed";
        public const string SingleDeepOwnedPokemonFromDeck = "SingleDeepOwnedPokemonFromDeck";
    }
    #endregion
}
