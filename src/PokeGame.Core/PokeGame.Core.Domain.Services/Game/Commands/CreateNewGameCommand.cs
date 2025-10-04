using System.Net;
using BT.Common.Persistence.Shared.Utils;
using FluentValidation;
using Microsoft.Extensions.Logging;
using PokeGame.Core.Common.Exceptions;
using PokeGame.Core.Domain.Services.Abstract;
using PokeGame.Core.Domain.Services.Models;
using PokeGame.Core.Persistence.Repositories.Abstract;
using PokeGame.Core.Schemas;
using PokeGame.Core.Schemas.Game;

namespace PokeGame.Core.Domain.Services.Game.Commands;

internal sealed class CreateNewGameCommand: IDomainCommand<(string CharacterName, Schemas.Game.User CurrentUser), DomainCommandResult<GameSave>>
{
    public string CommandName => nameof(CreateNewGameCommand);
    private readonly IGameSaveRepository _gameSaveRepository;
    private readonly IValidatorService _gameSaveValidator;
    private readonly ILogger<CreateNewGameCommand> _logger;
    
    public CreateNewGameCommand(IGameSaveRepository gameSaveRepository, 
        IValidatorService gameSaveValidator,
        ILogger<CreateNewGameCommand> logger)
    {
        _gameSaveRepository = gameSaveRepository;
        _gameSaveValidator = gameSaveValidator;
        _logger = logger;
    }

    public async Task<DomainCommandResult<GameSave>> ExecuteAsync(
        (string CharacterName, Schemas.Game.User CurrentUser) input,
        CancellationToken cancellationToken = default)
    {
        var newGameSave = new GameSave
        {
            Id = Guid.NewGuid(),
            CharacterName = input.CharacterName,
            UserId = (Guid)input.CurrentUser.Id!,
        };

        await _gameSaveValidator.ValidateAndThrowAsync(newGameSave, cancellationToken);

        var createdSave = await EntityFrameworkUtils
            .TryDbOperation(() => _gameSaveRepository.Create(newGameSave), _logger)
                ?? throw new  PokeGameApiServerException("Failed to save game save");
        
        _logger.LogDebug("Game save saved: {@GameSave}", newGameSave);
        
        if (!createdSave.IsSuccessful || createdSave.Data.Count == 0)
        {
            throw new PokeGameApiServerException("Failed to save game save");
        }


        return new DomainCommandResult<GameSave>
        {
            CommandResult = createdSave.FirstResult,
        };
    }
}