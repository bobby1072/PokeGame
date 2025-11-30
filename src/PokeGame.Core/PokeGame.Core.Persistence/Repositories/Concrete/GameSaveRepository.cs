using BT.Common.Persistence.Shared.Models;
using BT.Common.Polly.Extensions;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using PokeGame.Core.Common.Configurations;
using PokeGame.Core.Persistence.Contexts;
using PokeGame.Core.Persistence.Entities;
using PokeGame.Core.Persistence.Entities.Extensions;
using PokeGame.Core.Persistence.Repositories.Abstract;
using PokeGame.Core.Schemas.Game;

namespace PokeGame.Core.Persistence.Repositories.Concrete;

internal sealed class GameSaveRepository: BasePokeGameRepository<GameSaveEntity, Guid?, GameSave, PokeGameContext>, IGameSaveRepository
{
    private readonly  ILogger<GameSaveRepository> _logger;
    private readonly DbOperationRetrySettings _retrySettings;
    public GameSaveRepository(
        IDbContextFactory<PokeGameContext> dbContextFactory,
        ILogger<GameSaveRepository> logger,
        DbOperationRetrySettings retrySettings
    ) : base(dbContextFactory, logger, retrySettings)
    {
        _logger = logger;
        _retrySettings = retrySettings;
    }

    protected override GameSaveEntity RuntimeToEntity(GameSave gameSave)
    {
        return gameSave.ToEntity();
    }


    public async Task<DbResult> CreateGameSaveWithData(GameSave gameSave, GameSaveData data)
    {
        return await _retrySettings.ToPipeline().ExecuteAsync(async ct => await CreateGameSaveWithDataTransaction(gameSave, data, ct));
    }

    private async Task<DbResult> CreateGameSaveWithDataTransaction(GameSave gameSave, GameSaveData data, CancellationToken ct)
    {
        await using var dbContext = await ContextFactory.CreateDbContextAsync(ct);
        await using var transaction = await dbContext.Database.BeginTransactionAsync(ct);
        try
        {
            var gameSaveEnt = RuntimeToEntity(gameSave);
            var gameSaveDataEnt = data.ToGameSaveDataEntity();
                
            await dbContext.GameSaves.AddAsync(gameSaveEnt, ct);
            await dbContext.SaveChangesAsync(ct);
                
            var createdGameSave = dbContext.GameSaves.Local.First(x => x.Id == gameSaveEnt.Id);
            gameSaveDataEnt.GameSaveId = (Guid)createdGameSave.Id!;
                
            await dbContext.GameSaveData.AddAsync(gameSaveDataEnt, ct);
            await dbContext.SaveChangesAsync(ct);
                
            await transaction.CommitAsync(ct);
                
            return new DbResult(true);
        }
        catch (Exception e)
        {
            _logger.LogError(e, "Unhandled exception occurred during db transaction...");
            await AttemptToRollbackTransaction(transaction);
                
            return new DbResult(false);
        }
    }
}