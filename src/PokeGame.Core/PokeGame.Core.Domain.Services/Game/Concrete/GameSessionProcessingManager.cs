using PokeGame.Core.Domain.Services.Abstract;
using PokeGame.Core.Domain.Services.Game.Abstract;
using PokeGame.Core.Domain.Services.Game.Commands;
using PokeGame.Core.Domain.Services.Models;
using PokeGame.Core.Schemas.Game;

namespace PokeGame.Core.Domain.Services.Game.Concrete;

internal sealed class GameSessionProcessingManager : IGameSessionProcessingManager
{
    private readonly IDomainServiceCommandExecutor _domainServiceCommandExecutor;

    public GameSessionProcessingManager(IDomainServiceCommandExecutor domainServiceCommandExecutor)
    {
        _domainServiceCommandExecutor = domainServiceCommandExecutor;
    }

    public async Task<GameSession> StartGameSession(
        Guid gameSaveId,
        string connectionId,
        Schemas.Game.User user
    ) =>
        (
            await _domainServiceCommandExecutor.RunCommandAsync<
                StartGameSessionCommand,
                (Guid GameSaveId, string ConnectionId, Schemas.Game.User CurrentUser),
                DomainCommandResult<GameSession>
            >((gameSaveId, connectionId, user))
        ).CommandResult;

    public Task DeleteAllGameSessionsByConnectionId(string connectionId) =>
        _domainServiceCommandExecutor.RunCommandAsync<
            RemoveGameSessionsByConnectionIdCommand,
            string,
            DomainCommandResult
        >(connectionId);

    public Task DeleteAllGameSessionsByGameSave(Guid gameSaveId) =>
        _domainServiceCommandExecutor.RunCommandAsync<
            RemoveGameSessionByGameSaveIdCommand,
            Guid,
            DomainCommandResult
        >(gameSaveId);
}
