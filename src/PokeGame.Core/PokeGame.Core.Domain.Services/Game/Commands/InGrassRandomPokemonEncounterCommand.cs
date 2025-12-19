using System.Net;
using BT.Common.Persistence.Shared.Utils;
using BT.Common.Services.Concrete;
using Microsoft.Extensions.Logging;
using PokeGame.Core.Common.Exceptions;
using PokeGame.Core.Common.Extensions;
using PokeGame.Core.Common.GameInformationData;
using PokeGame.Core.Common.Permissions;
using PokeGame.Core.Domain.Services.Abstract;
using PokeGame.Core.Domain.Services.Game.Abstract;
using PokeGame.Core.Domain.Services.Models;
using PokeGame.Core.Persistence.Repositories.Abstract;
using PokeGame.Core.Schemas.Game;
using PokeGame.Core.Schemas.PokeApi;

namespace PokeGame.Core.Domain.Services.Game.Commands;

internal sealed class InGrassRandomPokemonEncounterCommand
    : IDomainCommand<
        (string SceneName, string ConnectionId, Schemas.Game.User CurrentUser),
        DomainCommandResult<WildPokemon?>
    >
{
    public string CommandName => nameof(InGrassRandomPokemonEncounterCommand);

    private readonly IGameSessionRepository _gameSessionRepository;
    private readonly IPokeApiClient _pokeApiClient;
    private readonly IPokeGameRuleHelperService _pokeGameRuleHelperService;
    private readonly ILogger<InGrassRandomPokemonEncounterCommand> _logger;

    public InGrassRandomPokemonEncounterCommand(
        IGameSessionRepository gameSessionRepository,
        IPokeApiClient pokeApiClient,
        IPokeGameRuleHelperService pokeGameRuleHelperService,
        ILogger<InGrassRandomPokemonEncounterCommand> logger
    )
    {
        _gameSessionRepository = gameSessionRepository;
        _pokeApiClient = pokeApiClient;
        _pokeGameRuleHelperService = pokeGameRuleHelperService;
        _logger = logger;
    }

    public async Task<DomainCommandResult<WildPokemon?>> ExecuteAsync(
        (string SceneName, string ConnectionId, Schemas.Game.User CurrentUser) input,
        CancellationToken cancelationToken = default
    )
    {
        _logger.LogInformation("About to attempt random pokemon in the grass encounter");

        using var activity = TelemetryHelperService.ActivitySource.StartActivity();
        activity?.SetTag(nameof(input.SceneName), input.SceneName);
        activity?.SetTag(nameof(input.ConnectionId), input.ConnectionId);
        activity?.SetTag("userId", input.CurrentUser.Id);

        if (
            !GameConstants.PokemonGameRules.SceneWildPokemonRange.TryGetValue(
                input.SceneName,
                out var sceneWildPokemonRange
            )
        )
        {
            throw new PokeGameApiUserException(
                HttpStatusCode.BadRequest,
                "Invalid pokegame scene name"
            );
        }

        var randomPokemonId =
            _pokeGameRuleHelperService.GetRandomPokedexNumberFromIntRangeWithRandomEncounterIncluded(
                sceneWildPokemonRange.PokedexRange
            );

        if (randomPokemonId is null)
        {
            return new DomainCommandResult<WildPokemon?> { CommandResult = null };
        }

        var gameData = await GetGameDataFromSession(input.ConnectionId, input.CurrentUser);

        if (!gameData.GameData.Abilities.Can(input.SceneName, PermissionAbilityPermissionType.Read))
        {
            throw new PokeGameApiUserException(
                HttpStatusCode.BadRequest,
                "You do not have access to this pokegame scene"
            );
        }

        var pokemonApi = await _pokeApiClient.GetResourceAsync<Pokemon>(
            (int)randomPokemonId,
            cancelationToken
        );

        var wildPokemonLevel = _pokeGameRuleHelperService.GetRandomNumberFromIntRange(
            sceneWildPokemonRange.PokemonLevelRange
        );

        var wildPokemonMoveSetResourceNames =
            _pokeGameRuleHelperService.GetRandomMoveSetFromPokemon(pokemonApi, wildPokemonLevel);

        throw new NotImplementedException();
    }

    private async Task<GameSaveData> GetGameDataFromSession(
        string connectionId,
        Schemas.Game.User currentUser
    )
    {
        var foundGameSession =
            await EntityFrameworkUtils.TryDbOperation(
                () =>
                    _gameSessionRepository.GetOneWithGameSaveAndDataByConnectionIdAsync(
                        connectionId
                    ),
                _logger
            ) ?? throw new PokeGameApiServerException("Failed to fetch game session results");

        if (!foundGameSession.IsSuccessful || foundGameSession.Data is null)
        {
            throw new PokeGameApiUserException(
                HttpStatusCode.BadRequest,
                "Failed to find game session for that connection id"
            );
        }

        if (foundGameSession.Data.GameSave?.GameSaveData is null)
        {
            throw new PokeGameApiServerException(
                "Failed to properly fetch game session with game save data"
            );
        }

        if (foundGameSession.Data.UserId != currentUser.Id)
        {
            throw new PokeGameApiUserException(
                HttpStatusCode.Unauthorized,
                "The current user is not the owner of the game session"
            );
        }

        _logger.LogInformation("Successfully fetched existing game session and game data");

        return foundGameSession.Data.GameSave.GameSaveData;
    }
}
