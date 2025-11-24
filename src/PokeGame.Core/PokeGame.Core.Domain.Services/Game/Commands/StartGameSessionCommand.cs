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

internal sealed class StartGameSessionCommand
    : IDomainCommand<
        (Guid GameSaveId, string ConnectionId, Schemas.Game.User CurrentUser),
        DomainCommandResult<GameSession>
    >
{
    public string CommandName => nameof(StartGameSessionCommand);
    private readonly IGameSessionRepository _gameSessionRepository;
    private readonly IGameSaveRepository _gameSaveRepository;
    private readonly ILogger<StartGameSessionCommand> _logger;

    public StartGameSessionCommand(
        IGameSessionRepository gameSessionRepository,
        IGameSaveRepository gameSaveRepository,
        ILogger<StartGameSessionCommand> logger
    )
    {
        _gameSessionRepository = gameSessionRepository;
        _gameSaveRepository = gameSaveRepository;
        _logger = logger;
    }

    public async Task<DomainCommandResult<GameSession>> ExecuteAsync(
        (Guid GameSaveId, string ConnectionId, Schemas.Game.User CurrentUser) input,
        CancellationToken cancellationToken = default
    )
    {
        _logger.LogInformation(
            "About to start new game session for user with id: {UserId}...",
            input.CurrentUser.Id
        );

        var foundExistingGameSave =
            await EntityFrameworkUtils.TryDbOperation(
                () => _gameSaveRepository.GetOne(input.GameSaveId),
                _logger
            ) ?? throw new PokeGameApiServerException("Failed to fetch game save");

        if (!foundExistingGameSave.IsSuccessful || foundExistingGameSave.Data is null)
        {
            throw new PokeGameApiUserException(
                HttpStatusCode.BadRequest,
                "Invalid game save id provided"
            );
        }

        if (foundExistingGameSave.Data.UserId != input.CurrentUser.Id)
        {
            throw new PokeGameApiUserException(
                HttpStatusCode.BadRequest,
                "Invalid game save id provided"
            );
        }
        
        var newSession = new GameSession
        {
            GameSaveId = input.GameSaveId,
            UserId = (Guid)input.CurrentUser.Id!,
            ConnectionId = input.ConnectionId,
        };

        var savedSession =
            (
                await EntityFrameworkUtils.TryDbOperation(
                    () => _gameSessionRepository.Create(newSession),
                    _logger
                )
            )?.FirstResult
            ?? throw new PokeGameApiServerException("Failed to add new game session");

        if (savedSession is null)
        {
            throw new PokeGameApiUserException(
                HttpStatusCode.InternalServerError,
                "Failed to save new game session"
            );
        }

        return new DomainCommandResult<GameSession> { CommandResult = savedSession };
    }
}
