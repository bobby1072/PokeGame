using BT.Common.Persistence.Shared.Models;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using PokeGame.Core.Common.Configurations;
using PokeGame.Core.Persistence.Contexts;
using PokeGame.Core.Persistence.Entities;
using PokeGame.Core.Persistence.Entities.Extensions;
using PokeGame.Core.Persistence.Repositories.Abstract;
using PokeGame.Core.Schemas.Game;

namespace PokeGame.Core.Persistence.Repositories.Concrete;

internal sealed class GameSessionRepository
    : BasePokeGameRepository<GameSessionEntity, Guid?, GameSession, PokeGameContext>,
        IGameSessionRepository
{
    private readonly ILogger<GameSessionRepository> _logger;

    public GameSessionRepository(
        IDbContextFactory<PokeGameContext> dbContextFactory,
        ILogger<GameSessionRepository> logger,
        DbOperationRetrySettings retrySettings
    )
        : base(dbContextFactory, logger, retrySettings)
    {
        _logger = logger;
    }

    protected override GameSessionEntity RuntimeToEntity(GameSession gameSession)
    {
        return gameSession.ToEntity();
    }

    public async Task<DbGetOneResult<GameSession>> GetOneWithGameSaveAndDataByGameSessionIdAsync(
        Guid gameSessionId
    )
    {
        await using var dbContext = await ContextFactory.CreateDbContextAsync();

        var result = await TimeAndLogDbOperation(
            () =>
                dbContext
                    .GameSessions.Include(x => x.GameSave)
                    .ThenInclude(x => x!.GameSaveData)
                    .FirstOrDefaultAsync(gs => gs.Id == gameSessionId),
            nameof(GetOneWithGameSaveAndDataByConnectionIdAsync)
        );

        return new DbGetOneResult<GameSession>(result?.ToModel());
    }

    public async Task<DbGetOneResult<GameSession>> GetOneWithGameSaveAndDataByConnectionIdAsync(
        string connectionId
    )
    {
        await using var dbContext = await ContextFactory.CreateDbContextAsync();

        var result = await TimeAndLogDbOperation(
            () =>
                dbContext
                    .GameSessions.Include(x => x.GameSave)
                    .ThenInclude(x => x!.GameSaveData)
                    .FirstOrDefaultAsync(gs => gs.ConnectionId == connectionId),
            nameof(GetOneWithGameSaveAndDataByConnectionIdAsync)
        );

        return new DbGetOneResult<GameSession>(result?.ToModel());
    }

    public async Task<DbResult> EndGameSession(GameSession gameSession)
    {
        await using var dbContext = await ContextFactory.CreateDbContextAsync();
        await using var transaction = await dbContext.Database.BeginTransactionAsync();

        try
        {
            var deleteGameSessionJob = TimeAndLogDbOperation(
                () =>
                    dbContext.GameSessions.Where(x => x.Id == gameSession.Id).ExecuteDeleteAsync(),
                $"{nameof(EndGameSession)}-DeleteGameSession"
            );

            var updateLatestPlayedJob = TimeAndLogDbOperation(
                () =>
                    dbContext
                        .GameSaves.Where(x => x.Id == gameSession.GameSaveId)
                        .ExecuteUpdateAsync(x => x.SetProperty(y => y.LastPlayed, DateTime.UtcNow)),
                $"{nameof(EndGameSession)}-UpdateGameSave"
            );

            await Task.WhenAll(deleteGameSessionJob, updateLatestPlayedJob);

            await dbContext.SaveChangesAsync();
            await transaction.CommitAsync();

            return new DbResult(true);
        }
        catch (Exception ex)
        {
            _logger.LogError(
                ex,
                "Unexcepted exception occurred during EndGameSession transaction..."
            );

            await AttemptToRollbackTransaction(transaction);

            return new DbResult(false);
        }
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
