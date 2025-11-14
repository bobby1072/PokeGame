using System.Net;
using BT.Common.Persistence.Shared.Utils;
using Microsoft.Extensions.Logging;
using PokeGame.Core.Common.Exceptions;
using PokeGame.Core.Domain.Services.Abstract;
using PokeGame.Core.Domain.Services.Models;
using PokeGame.Core.Persistence.Entities;
using PokeGame.Core.Persistence.Repositories.Abstract;
using PokeGame.Core.Schemas.Game;

namespace PokeGame.Core.Domain.Services.Game.Commands;

internal sealed class SaveGameDataCommand: IDomainCommand<(GameSaveData GameData, string ConnectionId, Schemas.Game.User CurrentUser),  DomainCommandResult<GameSaveData>>
{
    public string CommandName => nameof(SaveGameDataCommand);

    private readonly IGameSessionRepository _gameSessionRepository;
    private readonly IGameSaveDataRepository _gameSaveDataRepository;
    private readonly ILogger<SaveGameDataCommand> _logger;

    public SaveGameDataCommand(
        IGameSessionRepository gameSessionRepository,
        IGameSaveDataRepository gameSaveDataRepository,
        ILogger<SaveGameDataCommand> logger
    )
    {
        _gameSessionRepository = gameSessionRepository;
        _gameSaveDataRepository = gameSaveDataRepository;
        _logger = logger;
    }

    public async Task<DomainCommandResult<GameSaveData>> ExecuteAsync(
        (GameSaveData GameData, string ConnectionId, Schemas.Game.User CurrentUser) input,
        CancellationToken cancellationToken = default)
    {
        _logger.LogInformation("About to save game data...");

        var foundGameSession = await EntityFrameworkUtils
            .TryDbOperation(() =>
                _gameSessionRepository.GetOne(input.ConnectionId, nameof(GameSessionEntity.ConnectionId)), _logger)
            ?? throw new PokeGameApiServerException("Failed to fetch game session results");

        if (!foundGameSession.IsSuccessful || foundGameSession.Data is null)
        {
            throw new PokeGameApiUserException(HttpStatusCode.BadRequest,
                "Failed to find game session for that connection id");
        }

        if (foundGameSession.Data.UserId != input.CurrentUser.Id)
        {
            throw new PokeGameApiUserException(HttpStatusCode.Unauthorized,
                "The current user is not the owner of the game session");
        }
        input.GameData.GameSaveId = foundGameSession.Data.GameSaveId;
        if (input.GameData.Id is null)
        {
            _logger.LogInformation("Game data id is not present so attempting to fetch based on game save id: {GameSaveId}",
                foundGameSession.Data.GameSaveId);
            
            var foundExistingGameData = await EntityFrameworkUtils
                .TryDbOperation(() => _gameSaveDataRepository.GetOne(foundGameSession.Data.GameSaveId,
                    nameof(GameSaveDataEntity.GameSaveId)), _logger)
                ?? throw new PokeGameApiServerException("Failed to fetch game save data");

            if (!foundExistingGameData.IsSuccessful || foundExistingGameData.Data is null)
            {
                throw new PokeGameApiServerException("Failed to fetch game save data");
            }
            
            input.GameData.Id = foundExistingGameData.Data.Id;
        }

        var updatedGameSave = await EntityFrameworkUtils
            .TryDbOperation(() => _gameSaveDataRepository.Update(input.GameData), _logger)
            ?? throw new  PokeGameApiServerException("Failed to save game data");

        if (!updatedGameSave.IsSuccessful || updatedGameSave.Data is null)
        {
            throw new PokeGameApiServerException("Failed to save game data");
        }

        return new DomainCommandResult<GameSaveData>
        {
            CommandResult = updatedGameSave.FirstResult
        };
    }
}