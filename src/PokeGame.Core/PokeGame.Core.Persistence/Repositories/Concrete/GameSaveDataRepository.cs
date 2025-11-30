using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using PokeGame.Core.Common.Configurations;
using PokeGame.Core.Persistence.Contexts;
using PokeGame.Core.Persistence.Entities;
using PokeGame.Core.Persistence.Entities.Extensions;
using PokeGame.Core.Persistence.Repositories.Abstract;
using PokeGame.Core.Schemas.Game;

namespace PokeGame.Core.Persistence.Repositories.Concrete;

internal sealed class GameSaveDataRepository: BasePokeGameRepository<GameSaveDataEntity, long?, GameSaveData, PokeGameContext>, IGameSaveDataRepository
{
    public GameSaveDataRepository(
        IDbContextFactory<PokeGameContext> dbContextFactory,
        ILogger<GameSaveDataRepository> logger,
        DbOperationRetrySettings retrySettings
    ) : base(dbContextFactory, logger, retrySettings){}

    protected override GameSaveDataEntity RuntimeToEntity(GameSaveData runtimeObj)
    {
        return runtimeObj.ToGameSaveDataEntity();
    }
}