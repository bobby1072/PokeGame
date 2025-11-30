using System.Net;
using BT.Common.Persistence.Shared.Utils;
using BT.Common.Services.Concrete;
using Microsoft.Extensions.Logging;
using PokeGame.Core.Common.Exceptions;
using PokeGame.Core.Domain.Services.Abstract;
using PokeGame.Core.Domain.Services.Models;
using PokeGame.Core.Persistence.Entities;
using PokeGame.Core.Persistence.Repositories.Abstract;

namespace PokeGame.Core.Domain.Services.Game.Commands;

internal sealed class EndGameSessionCommand : IDomainCommand<string, DomainCommandResult>
{
    public string CommandName => nameof(EndGameSessionCommand);

    private readonly IGameSessionRepository _gameSessionRepository;
    private readonly ILogger<EndGameSessionCommand> _logger;

    public EndGameSessionCommand(
        IGameSessionRepository gameSessionRepository,
        ILogger<EndGameSessionCommand> logger
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
        using var activity = TelemetryHelperService.ActivitySource.StartActivity(CommandName);
        activity?.SetTag("connectionId", connectionId);

        _logger.LogInformation(
            "About to end game session for connection Id: {ConnectionId}",
            connectionId
        );

        var foundGameSession =
            await EntityFrameworkUtils.TryDbOperation(
                () =>
                    _gameSessionRepository.GetOne(
                        connectionId,
                        nameof(GameSessionEntity.ConnectionId)
                    ),
                _logger
            ) ?? throw new PokeGameApiServerException("Failed to fetch game session");

        if (foundGameSession.Data is null)
        {
            throw new PokeGameApiUserException(
                HttpStatusCode.BadRequest,
                "Game session with that id doesn't exist"
            );
        }
        var endedResult =
            await EntityFrameworkUtils.TryDbOperation(
                () => _gameSessionRepository.EndGameSession(foundGameSession.Data),
                _logger
            ) ?? throw new PokeGameApiServerException("Failed to end game session");

        if (!endedResult.IsSuccessful)
        {
            throw new PokeGameApiServerException("Failed to end game session");
        }

        return new DomainCommandResult();
    }
}
