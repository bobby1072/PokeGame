using System.Net;
using BT.Common.FastArray.Proto;
using BT.Common.Persistence.Shared.Utils;
using BT.Common.Services.Concrete;
using Microsoft.Extensions.Logging;
using PokeGame.Core.Common.Exceptions;
using PokeGame.Core.Domain.Services.Abstract;
using PokeGame.Core.Domain.Services.Models;
using PokeGame.Core.Persistence.Entities;
using PokeGame.Core.Persistence.Repositories.Abstract;
using PokeGame.Core.Schemas.Game;

namespace PokeGame.Core.Domain.Services.Game.Commands;

internal sealed class SaveGameDataCommand
    : IDomainCommand<
        (GameSaveData GameData, string ConnectionId, Schemas.Game.User CurrentUser),
        DomainCommandResult<GameSaveData>
    >
{
    public string CommandName => nameof(SaveGameDataCommand);

    private readonly IGameSessionRepository _gameSessionRepository;
    private readonly IGameSaveDataRepository _gameSaveDataRepository;
    private readonly IOwnedPokemonRepository _ownedPokemonRepository;
    private readonly IValidatorService _validatorService;
    private readonly ILogger<SaveGameDataCommand> _logger;

    public SaveGameDataCommand(
        IGameSessionRepository gameSessionRepository,
        IGameSaveDataRepository gameSaveDataRepository,
        IOwnedPokemonRepository ownedPokemonRepository,
        IValidatorService validatorService,
        ILogger<SaveGameDataCommand> logger
    )
    {
        _gameSessionRepository = gameSessionRepository;
        _gameSaveDataRepository = gameSaveDataRepository;
        _ownedPokemonRepository = ownedPokemonRepository;
        _validatorService = validatorService;
        _logger = logger;
    }

    public async Task<DomainCommandResult<GameSaveData>> ExecuteAsync(
        (GameSaveData GameData, string ConnectionId, Schemas.Game.User CurrentUser) input,
        CancellationToken cancellationToken = default
    )
    {
        using var activity = TelemetryHelperService.ActivitySource.StartActivity(CommandName);
        activity?.SetTag("connectionId", input.ConnectionId);
        activity?.SetTag("userId", input.CurrentUser.Id?.ToString());
        activity?.SetTag("gameSaveId", input.GameData.GameSaveId.ToString());

        _logger.LogInformation("About to save game data...");

        await _validatorService.ValidateAndThrowAsync(input.GameData, cancellationToken);

        var foundGameSession = await GetGameSession(input.ConnectionId, input.CurrentUser);
        var foundExistingGameData = await GetExistingGameSaveData(foundGameSession);

        input.GameData.GameSaveId = foundGameSession.GameSaveId;
        input.GameData.Id = foundExistingGameData.Id;

        await ValidateNewGameSaveDataAgainstOld(
            input.GameData,
            foundExistingGameData,
            input.CurrentUser
        );

        var updatedGameSave = await UpdateGameSave(input.GameData);

        return new DomainCommandResult<GameSaveData> { CommandResult = updatedGameSave };
    }

    private async Task ValidateNewGameSaveDataAgainstOld(
        GameSaveData newGameSaveData,
        GameSaveData oldGameSaveData,
        Schemas.Game.User currentUser
    )
    {
        var oldPokemonOwnedIds = oldGameSaveData.GameData.DeckPokemon.FastArraySelect(x =>
            x.OwnedPokemonId
        );

        var newPokemon = newGameSaveData
            .GameData.DeckPokemon.FastArrayWhere(x =>
                !oldPokemonOwnedIds.Contains(x.OwnedPokemonId)
            )
            .ToArray();
        if (newPokemon.Length > 0)
        {
            var foundOwnedPokemon =
                await EntityFrameworkUtils.TryDbOperation(
                    () =>
                        _ownedPokemonRepository.GetMany(
                            newPokemon
                                .FastArraySelect<GameSaveDataActualDeckPokemon, Guid?>(x =>
                                    x.OwnedPokemonId
                                )
                                .ToArray(),
                            relations: nameof(OwnedPokemonEntity.GameSave)
                        )
                ) ?? throw new PokeGameApiServerException("Failed to fetch owned pokemon");

            if (foundOwnedPokemon.Data.Count < 1 || !foundOwnedPokemon.IsSuccessful)
            {
                throw new PokeGameApiUserException(
                    HttpStatusCode.BadRequest,
                    "Failed to find the pokemon you're adding to you deck"
                );
            }

            if (foundOwnedPokemon.Data.Any(x => x.GameSave?.UserId != currentUser.Id))
            {
                throw new PokeGameApiUserException(
                    HttpStatusCode.Unauthorized,
                    "The pokemon in your deck have to belong to you"
                );
            }
        }
    }

    private async Task<GameSaveData> GetExistingGameSaveData(GameSession foundGameSession)
    {
        var foundExistingGameData =
            await EntityFrameworkUtils.TryDbOperation(
                () =>
                    _gameSaveDataRepository.GetOne(
                        foundGameSession.GameSaveId,
                        nameof(GameSaveDataEntity.GameSaveId)
                    ),
                _logger
            ) ?? throw new PokeGameApiServerException("Failed to fetch game save data");

        if (!foundExistingGameData.IsSuccessful || foundExistingGameData.Data is null)
        {
            throw new PokeGameApiServerException("Failed to fetch game save data");
        }

        return foundExistingGameData.Data;
    }

    private async Task<GameSession> GetGameSession(
        string connectionId,
        Schemas.Game.User currentUser
    )
    {
        var foundGameSession =
            await EntityFrameworkUtils.TryDbOperation(
                () =>
                    _gameSessionRepository.GetOne(
                        connectionId,
                        nameof(GameSessionEntity.ConnectionId)
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

        if (foundGameSession.Data.UserId != currentUser.Id)
        {
            throw new PokeGameApiUserException(
                HttpStatusCode.Unauthorized,
                "The current user is not the owner of the game session"
            );
        }

        return foundGameSession.Data;
    }

    private async Task<GameSaveData> UpdateGameSave(GameSaveData newGameData)
    {
        var updatedGameSave =
            await EntityFrameworkUtils.TryDbOperation(
                () => _gameSaveDataRepository.Update(newGameData),
                _logger
            ) ?? throw new PokeGameApiServerException("Failed to save game data");

        if (!updatedGameSave.IsSuccessful || updatedGameSave.Data is null)
        {
            throw new PokeGameApiServerException("Failed to save game data");
        }

        return updatedGameSave.FirstResult;
    }
}
