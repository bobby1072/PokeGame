using BT.Common.Persistence.Shared.Repositories.Abstract;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Storage;
using Microsoft.Extensions.Logging;
using PokeGame.Core.Common.Configurations;
using PokeGame.Core.Persistence.Entities;
using PokeGame.Core.Schemas;

namespace PokeGame.Core.Persistence.Repositories.Abstract;

internal abstract class BasePokeGameRepository<TEnt, TEntId, TModel, TDbContext> : BaseRepository<TEnt, TEntId, TModel, TDbContext>, IRepository<TEnt, TEntId, TModel>
    where TEnt : BasePokeGameEntity<TEntId, TModel>
    where TModel : PersistableDomainModel<TModel, TEntId>
    where TDbContext : DbContext
{
    private readonly ILogger<BasePokeGameRepository<TEnt, TEntId, TModel, TDbContext>> _logger;

    public BasePokeGameRepository(
        IDbContextFactory<TDbContext> dbContextFactory,
        ILogger<BasePokeGameRepository<TEnt, TEntId, TModel, TDbContext>> logger,
        DbOperationRetrySettings retrySettings)
        : base(dbContextFactory, logger, retrySettings)
    {
        _logger = logger;
    }


    protected async Task AttemptToRollbackTransaction(IDbContextTransaction transaction)
    {
        try
        {
            await transaction.RollbackAsync();
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Failed to  rollback transaction...");
        }
    }
}