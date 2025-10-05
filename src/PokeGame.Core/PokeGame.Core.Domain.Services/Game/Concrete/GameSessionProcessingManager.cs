using PokeGame.Core.Domain.Services.Abstract;
using PokeGame.Core.Domain.Services.Game.Abstract;
using PokeGame.Core.Domain.Services.Game.Commands;
using PokeGame.Core.Schemas.Game;
using PokeGame.Core.Domain.Services.Models;

namespace PokeGame.Core.Domain.Services.Game.Concrete;

internal sealed class GameSessionProcessingManager: IGameSessionProcessingManager
{
    private readonly IDomainServiceCommandExecutor _domainServiceCommandExecutor;

    public GameSessionProcessingManager(IDomainServiceCommandExecutor domainServiceCommandExecutor)
    {
        _domainServiceCommandExecutor = domainServiceCommandExecutor;
    }

    public async Task<GameSession> StartGameSession(Guid gameSaveId, Schemas.Game.User user) =>
        (await _domainServiceCommandExecutor
            .RunCommandAsync<StartGameSessionCommand, (Guid GameSaveId, Schemas.Game.User CurrentUser),
                DomainCommandResult<GameSession>>
            (
                (gameSaveId, user)
            )).CommandResult;

    public Task DeleteAllGameSessionsForGameSave(Guid gameSaveId) => _domainServiceCommandExecutor
        .RunCommandAsync<RemoveGameSessionCommand, Guid, DomainCommandResult>(gameSaveId);
}