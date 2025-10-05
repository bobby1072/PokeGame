using BT.Common.Persistence.Shared.Utils;
using Microsoft.Extensions.Logging;
using PokeGame.Core.Domain.Services.Abstract;
using PokeGame.Core.Domain.Services.Models;
using PokeGame.Core.Persistence.Repositories.Abstract;

namespace PokeGame.Core.Domain.Services.Game.Commands;

internal sealed class RemoveGameSessionsByConnectionIdCommand
    : IDomainCommand<string, DomainCommandResult>
{
    public string CommandName => nameof(RemoveGameSessionsByConnectionIdCommand);

    private readonly IGameSessionRepository _gameSessionRepository;
    private readonly ILogger<RemoveGameSessionsByConnectionIdCommand> _logger;

    public RemoveGameSessionsByConnectionIdCommand(
        IGameSessionRepository gameSessionRepository,
        ILogger<RemoveGameSessionsByConnectionIdCommand> logger
    )
    {
        _gameSessionRepository = gameSessionRepository;
        _logger = logger;
    }

    public async Task<DomainCommandResult> ExecuteAsync(
        string connectionId,
        CancellationToken cancellationToken = default
    )
    {
        _logger.LogInformation(
            "About to remove all game sessions for connection ID: {ConnectionId}...",
            connectionId
        );

        await EntityFrameworkUtils.TryDbOperation(
            () => _gameSessionRepository.DeleteAllSessionsByConnectionIdAsync(connectionId),
            _logger
        );

        return new DomainCommandResult();
    }
}
