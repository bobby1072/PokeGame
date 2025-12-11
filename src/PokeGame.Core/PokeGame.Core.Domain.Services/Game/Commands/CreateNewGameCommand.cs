using System.Net;
using BT.Common.FastArray.Proto;
using BT.Common.Persistence.Shared.Utils;
using BT.Common.Services.Concrete;
using Microsoft.Extensions.Logging;
using PokeGame.Core.Common.Exceptions;
using PokeGame.Core.Common.GameInformationData;
using PokeGame.Core.Domain.Services.Abstract;
using PokeGame.Core.Domain.Services.Models;
using PokeGame.Core.Persistence.Repositories.Abstract;
using PokeGame.Core.Schemas.Game;

namespace PokeGame.Core.Domain.Services.Game.Commands;

internal sealed class CreateNewGameCommand
    : IDomainCommand<
        (string CharacterName, Schemas.Game.User CurrentUser),
        DomainCommandResult<GameSave>
    >
{
    public string CommandName => nameof(CreateNewGameCommand);
    private readonly IGameSaveRepository _gameSaveRepository;
    private readonly IValidatorService _gameSaveValidator;
    private readonly ILogger<CreateNewGameCommand> _logger;

    public CreateNewGameCommand(
        IGameSaveRepository gameSaveRepository,
        IValidatorService gameSaveValidator,
        ILogger<CreateNewGameCommand> logger
    )
    {
        _gameSaveRepository = gameSaveRepository;
        _gameSaveValidator = gameSaveValidator;
        _logger = logger;
    }

    public async Task<DomainCommandResult<GameSave>> ExecuteAsync(
        (string CharacterName, Schemas.Game.User CurrentUser) input,
        CancellationToken cancellationToken = default
    )
    {
        using var activity = TelemetryHelperService.ActivitySource.StartActivity(CommandName);
        activity?.SetTag("characterName", input.CharacterName);
        activity?.SetTag("userId", input.CurrentUser.Id?.ToString());

        var newGameSave = new GameSave
        {
            Id = Guid.NewGuid(),
            CharacterName = input.CharacterName,
            UserId = (Guid)input.CurrentUser.Id!,
        };
        var newGameSaveData = CreateNewGameSaveData((Guid)newGameSave.Id!);

        await _gameSaveValidator.ValidateAndThrowAsync(newGameSave, cancellationToken);

        var gameSaveCount =
            await EntityFrameworkUtils.TryDbOperation(
                () => _gameSaveRepository.GetCount(x => x.UserId == input.CurrentUser.Id)
            ) ?? throw new PokeGameApiServerException("Failed to get game save count");

        if (gameSaveCount.Data >= 5)
        {
            throw new PokeGameApiUserException(
                HttpStatusCode.BadRequest,
                "You can only have 5 game saves"
            );
        }

        var createdSave =
            await EntityFrameworkUtils.TryDbOperation(
                () => _gameSaveRepository.CreateGameSaveWithData(newGameSave, newGameSaveData),
                _logger
            ) ?? throw new PokeGameApiServerException("Failed to save game save");

        if (!createdSave.IsSuccessful)
        {
            throw new PokeGameApiServerException("Failed to save game save");
        }

        newGameSave.GameSaveData = newGameSaveData;
        _logger.LogDebug("Game save saved: {@GameSave}", newGameSave);

        return new DomainCommandResult<GameSave> { CommandResult = newGameSave };
    }

    private static GameSaveData CreateNewGameSaveData(Guid gameSaveId)
    {
        return new GameSaveData
        {
            GameSaveId = gameSaveId,
            GameData = new GameSaveDataActual
            {
                LastPlayedScene = GameConstants.NewGameInfo.StartingScene,
                LastPlayedLocationX = GameConstants.NewGameInfo.StartingLocationX,
                LastPlayedLocationY = GameConstants.NewGameInfo.StartingLocationY,
                UnlockedGameResources = GameConstants.NewGameInfo.BaseUnlockedSceneNames.FastArraySelect(x => new GameDataActualUnlockedGameResource
                { 
                    Type = GameDataActualUnlockedGameResourceType.Scene,
                    ResourceName = x
                }).ToList()
            },
        };
    }
}
