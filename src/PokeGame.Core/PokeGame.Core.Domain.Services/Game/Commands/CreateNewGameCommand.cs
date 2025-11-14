using BT.Common.Persistence.Shared.Utils;
using Microsoft.Extensions.Logging;
using PokeGame.Core.Common.Configurations;
using PokeGame.Core.Common.Exceptions;
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
    private readonly PokeGameRules _pokeGameRules;
    private readonly ILogger<CreateNewGameCommand> _logger;

    public CreateNewGameCommand(
        IGameSaveRepository gameSaveRepository,
        IValidatorService gameSaveValidator,
        PokeGameRules pokeGameRules,
        ILogger<CreateNewGameCommand> logger
    )
    {
        _gameSaveRepository = gameSaveRepository;
        _gameSaveValidator = gameSaveValidator;
        _pokeGameRules = pokeGameRules;
        _logger = logger;
    }

    public async Task<DomainCommandResult<GameSave>> ExecuteAsync(
        (string CharacterName, Schemas.Game.User CurrentUser) input,
        CancellationToken cancellationToken = default
    )
    {
        var newGameSave = new GameSave
        {
            Id = Guid.NewGuid(),
            CharacterName = input.CharacterName,
            UserId = (Guid)input.CurrentUser.Id!,
        };
        var newGameSaveData = CreateNewGameSaveData((Guid)newGameSave.Id!);
        
        await _gameSaveValidator.ValidateAndThrowAsync(newGameSave, cancellationToken);

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
    private GameSaveData CreateNewGameSaveData(Guid gameSaveId)
    {
        return new GameSaveData
        {
            GameSaveId = gameSaveId,
            GameData = new GameSaveDataActual
            {
                LastPlayedScene = _pokeGameRules.DefaultStarterScene.SceneName,
                LastPlayedLocationX = _pokeGameRules.DefaultStarterScene.SceneLocation.X,
                LastPlayedLocationY = _pokeGameRules.DefaultStarterScene.SceneLocation.Y
            }
        };
    }
}
