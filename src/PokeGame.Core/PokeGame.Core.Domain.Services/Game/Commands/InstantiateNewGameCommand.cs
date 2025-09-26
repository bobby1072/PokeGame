using FluentValidation;
using Microsoft.Extensions.Logging;
using PokeGame.Core.Domain.Services.Abstract;
using PokeGame.Core.Domain.Services.Models;
using PokeGame.Core.Persistence.Repositories.Abstract;
using PokeGame.Core.Schemas;
using PokeGame.Core.Schemas.Input;

namespace PokeGame.Core.Domain.Services.Game.Commands;

internal sealed class InstantiateNewGameCommand: IDomainCommand<GameSaveInput, DomainCommandResult<GameSave>>
{
    public string CommandName => nameof(InstantiateNewGameCommand);
    private readonly IGameSaveRepository _gameSaveRepository;
    private readonly IValidator<GameSave> _gameSaveValidator;
    private readonly ILogger<InstantiateNewGameCommand> _logger;
    
    public InstantiateNewGameCommand(IGameSaveRepository gameSaveRepository, 
        IValidator<GameSave> gameSaveValidator,
        ILogger<InstantiateNewGameCommand> logger)
    {
        _gameSaveRepository = gameSaveRepository;
        _gameSaveValidator = gameSaveValidator;
        _logger = logger;
    }

    public async Task<DomainCommandResult<GameSave>> ExecuteAsync(GameSaveInput input,
        CancellationToken cancellationToken = default)
    {
        throw new NotImplementedException();
    }
}