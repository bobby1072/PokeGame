using System.Net;
using BT.Common.FastArray.Proto;
using BT.Common.Persistence.Shared.Utils;
using BT.Common.Services.Concrete;
using Microsoft.Extensions.Logging;
using PokeGame.Core.Common.Exceptions;
using PokeGame.Core.Domain.Services.Abstract;
using PokeGame.Core.Domain.Services.Game.Abstract;
using PokeGame.Core.Domain.Services.Models;
using PokeGame.Core.Persistence.Repositories.Abstract;
using PokeGame.Core.Schemas.Game;

namespace PokeGame.Core.Domain.Services.Game.Commands;

internal abstract class GetOwnedPokemonInDeckCommandBase<TInput>
    : IDomainCommand<TInput, DomainCommandResult<IReadOnlyCollection<OwnedPokemon>>>
{
    public abstract string CommandName { get; }
    private readonly IGameAndPokeApiResourceManagerService _gameAndPokeApiResourceManagerService;
    private readonly IOwnedPokemonRepository _ownedPokemonRepository;
    private readonly ILogger<GetOwnedPokemonInDeckCommandBase<TInput>> _logger;

    public GetOwnedPokemonInDeckCommandBase(
        IGameAndPokeApiResourceManagerService gameAndPokeApiResourceManagerService,
        IOwnedPokemonRepository ownedPokemonRepository,
        ILogger<GetOwnedPokemonInDeckCommandBase<TInput>> logger
    )
    {
        _gameAndPokeApiResourceManagerService = gameAndPokeApiResourceManagerService;
        _ownedPokemonRepository = ownedPokemonRepository;
        _logger = logger;
    }

    public abstract Task<DomainCommandResult<IReadOnlyCollection<OwnedPokemon>>> ExecuteAsync(
        TInput input,
        CancellationToken cancellationToken
    );

    protected async Task<IReadOnlyCollection<OwnedPokemon>> FetchPokemon(
        GameSession? gameSession,
        bool deepVersion,
        Schemas.Game.User currentUser,
        CancellationToken cancellationToken
    )
    {
        using var activity = TelemetryHelperService.ActivitySource.StartActivity(
            nameof(FetchPokemon)
        );
        activity?.SetTag("deepVersion", deepVersion);
        activity?.SetTag("userId", currentUser.Id?.ToString());
        activity?.SetTag("gameSessionId", gameSession?.Id.ToString());

        if (gameSession is null)
        {
            throw new PokeGameApiUserException(
                HttpStatusCode.BadRequest,
                "Failed to find game save data"
            );
        }

        if (gameSession.UserId != currentUser.Id)
        {
            throw new PokeGameApiUserException(
                HttpStatusCode.Unauthorized,
                "User does not have permission to access this deck"
            );
        }
        if (gameSession.GameSave?.GameSaveData?.GameData.DeckPokemon is null)
        {
            throw new PokeGameApiUserException(
                HttpStatusCode.BadRequest,
                "Empty pokemon deck for game save"
            );
        }
        if (gameSession.GameSave.GameSaveData.GameData.DeckPokemon.Count < 1)
        {
            return [];
        }

        _logger.LogInformation(
            "Going to fetch {PokemonInDeckCount} OwnedPokemon from deck for game session: {GameSessionId} and game save: {GameSaveId}",
            gameSession.GameSave.GameSaveData.GameData.DeckPokemon.Count,
            gameSession.Id,
            gameSession.GameSaveId
        );

        if (!deepVersion)
        {
            _logger.LogInformation("Simply getting shallow owned pokemon in deck from db");
            var allPokemon = await GetShallowDeck(
                gameSession
                    .GameSave.GameSaveData.GameData.DeckPokemon.FastArraySelect(x =>
                        (Guid?)x.OwnedPokemonId
                    )
                    .ToArray()
            );

            return allPokemon;
        }
        else
        {
            _logger.LogInformation("Fetching deep owned pokemon in deck from db and poke api");
            var allPokemon = await _gameAndPokeApiResourceManagerService.GetFullOwnedPokemon(
                gameSession
                    .GameSave.GameSaveData.GameData.DeckPokemon.FastArraySelect(x =>
                        x.OwnedPokemonId
                    )
                    .ToArray(),
                cancellationToken
            );

            return allPokemon;
        }
    }

    private async Task<IReadOnlyCollection<OwnedPokemon>> GetShallowDeck(
        IReadOnlyCollection<Guid?> deckPokemonIds
    )
    {
        var foundPokemon =
            await EntityFrameworkUtils.TryDbOperation(
                () => _ownedPokemonRepository.GetMany(entityIds: deckPokemonIds),
                _logger
            ) ?? throw new PokeGameApiServerException("Failed to fetch own pokemon in deck");

        if (foundPokemon.Data.Count < 1 || !foundPokemon.IsSuccessful)
        {
            throw new PokeGameApiServerException("Failed to fetch own pokemon in deck");
        }

        return foundPokemon.Data;
    }
}
