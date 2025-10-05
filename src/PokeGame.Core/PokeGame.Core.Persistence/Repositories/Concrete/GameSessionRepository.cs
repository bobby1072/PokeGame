using BT.Common.Persistence.Shared.Repositories.Abstract;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using PokeGame.Core.Persistence.Contexts;
using PokeGame.Core.Persistence.Entities;
using PokeGame.Core.Persistence.Entities.Extensions;
using PokeGame.Core.Persistence.Repositories.Abstract;
using PokeGame.Core.Schemas.Game;

namespace PokeGame.Core.Persistence.Repositories.Concrete;

internal sealed class GameSessionRepository
    : BaseRepository<GameSessionEntity, Guid?, GameSession, PokeGameContext>,
        IGameSessionRepository
{
    public GameSessionRepository(
        IDbContextFactory<PokeGameContext> dbContextFactory,
        ILogger<GameSessionRepository> logger
    )
        : base(dbContextFactory, logger) { }

    protected override GameSessionEntity RuntimeToEntity(GameSession gameSession)
    {
        return gameSession.ToEntity();
    }

    public async Task DeleteAllSessionsByGameSaveIdAsync(Guid gameSaveId)
    {
        await using var dbContext = await ContextFactory.CreateDbContextAsync();

        var result = await TimeAndLogDbOperation(
            () =>
                dbContext
                    .GameSessions.Where(gs => gs.GameSaveId == gameSaveId)
                    .ExecuteDeleteAsync(),
            nameof(DeleteAllSessionsByGameSaveIdAsync)
        );
    }

    public async Task DeleteAllSessionsByConnectionIdAsync(string connectionId)
    {
        await using var dbContext = await ContextFactory.CreateDbContextAsync();

        var result = await TimeAndLogDbOperation(
            () =>
                dbContext
                    .GameSessions.Where(gs => gs.ConnectionId == connectionId)
                    .ExecuteDeleteAsync(),
            nameof(DeleteAllSessionsByConnectionIdAsync)
        );
    }
}
