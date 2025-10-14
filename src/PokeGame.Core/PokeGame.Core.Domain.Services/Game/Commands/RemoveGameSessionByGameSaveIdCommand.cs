using BT.Common.Persistence.Shared.Utils;
using Microsoft.Extensions.Logging;
using PokeGame.Core.Domain.Services.Abstract;
using PokeGame.Core.Domain.Services.Models;
using PokeGame.Core.Persistence.Repositories.Abstract;

namespace PokeGame.Core.Domain.Services.Game.Commands;

internal sealed class RemoveGameSessionByGameSaveIdCommand
    : IDomainCommand<Guid, DomainCommandResult>
{
    public string CommandName => nameof(RemoveGameSessionByGameSaveIdCommand);

    private readonly IGameSessionRepository _gameSessionRepository;
    private readonly ILogger<RemoveGameSessionByGameSaveIdCommand> _logger;

    public RemoveGameSessionByGameSaveIdCommand(
        IGameSessionRepository gameSessionRepository,
        ILogger<RemoveGameSessionByGameSaveIdCommand> logger
    )
    {
        _gameSessionRepository = gameSessionRepository;
        _logger = logger;
    }

    public async Task<DomainCommandResult> ExecuteAsync(
        Guid gameSaveId,
        CancellationToken cancellationToken = default
    )
    {
        _logger.LogInformation(
            "About to remove all game sessions for game save with id: {GameSaveId}...",
            gameSaveId
        );

        await EntityFrameworkUtils.TryDbOperation(
            () => _gameSessionRepository.DeleteAllSessionsByGameSaveIdAsync(gameSaveId),
            _logger
        );

        return new DomainCommandResult();
    }
}
