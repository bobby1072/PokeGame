using BT.Common.Api.Helpers.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using PokeGame.Core.Common.Attributes;
using PokeGame.Core.Domain.Services.Game.Abstract;
using PokeGame.Core.Schemas.Game;

namespace PokeGame.Core.Api.Controllers;

[AllowAnonymous]
[RequireValidUserIdHeader]
public sealed class GameSessionController: BaseController
{
    private readonly IGameSessionProcessingManager _gameSessionProcessingManager;

    public GameSessionController(IGameSessionProcessingManager gameSessionProcessingManager)
    {
        _gameSessionProcessingManager = gameSessionProcessingManager;
    }
    
    [HttpGet(nameof(GetOwnedShallowOwnedPokemonInDeck))]
    public async Task<ActionResult<WebOutcome<IReadOnlyCollection<OwnedPokemon>>>> GetOwnedShallowOwnedPokemonInDeck(Guid gameSessionId, CancellationToken cancellationToken = default)
    {
        var currentUser = await GetCurrentUserAsync();
        
        var result = await _gameSessionProcessingManager.GetShallowOwnedPokemonInDeck(gameSessionId, currentUser, cancellationToken);

        return new WebOutcome<IReadOnlyCollection<OwnedPokemon>>
        {
            Data = result,
        };
    }
}