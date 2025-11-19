using BT.Common.Persistence.Shared.Models;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using PokeGame.Core.Persistence.Contexts;
using PokeGame.Core.Persistence.Entities;
using PokeGame.Core.Persistence.Entities.Extensions;
using PokeGame.Core.Persistence.Repositories.Abstract;
using PokeGame.Core.Schemas.Game;

namespace PokeGame.Core.Persistence.Repositories.Concrete;

internal sealed class GameSaveRepository: BasePokeGameRepository<GameSaveEntity, Guid?, GameSave, PokeGameContext>, IGameSaveRepository
{
    private readonly  ILogger<GameSaveRepository> _logger;

    public GameSaveRepository(
        IDbContextFactory<PokeGameContext> dbContextFactory,
        ILogger<GameSaveRepository> logger
    ) : base(dbContextFactory, logger)
    {
        _logger = logger;
    }

    protected override GameSaveEntity RuntimeToEntity(GameSave gameSave)
    {
        return gameSave.ToEntity();
    }


    public async Task<DbResult> CreateGameSaveWithData(GameSave gameSave, GameSaveData data)
    {
        await using var dbContext = await ContextFactory.CreateDbContextAsync();
        await using var transaction = await dbContext.Database.BeginTransactionAsync();
        try
        {
            var gameSaveEnt = RuntimeToEntity(gameSave);
            var gameSaveDataEnt = data.ToGameSaveDataEntity();
            
            await dbContext.GameSaves.AddAsync(gameSaveEnt);
            await dbContext.SaveChangesAsync();
            
            var createdGameSave = dbContext.GameSaves.Local.First(x => x.Id == gameSaveEnt.Id);
            gameSaveDataEnt.GameSaveId = (Guid)createdGameSave.Id!;
            
            await dbContext.GameSaveData.AddAsync(gameSaveDataEnt);
            await dbContext.SaveChangesAsync();
            
            await transaction.CommitAsync();
            
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