using BT.Common.Persistence.Shared.Repositories.Abstract;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using PokeGame.Core.Persistence.Contexts;
using PokeGame.Core.Persistence.Entities;
using PokeGame.Core.Persistence.Entities.Extensions;
using PokeGame.Core.Persistence.Repositories.Abstract;
using PokeGame.Core.Schemas;

namespace PokeGame.Core.Persistence.Repositories.Concrete;

internal sealed class GameSaveRepository: BaseRepository<GameSaveEntity, Guid?, GameSave, PokeGameContext>, IGameSaveRepository
{
    public GameSaveRepository(
        IDbContextFactory<PokeGameContext> dbContextFactory,
        ILogger<GameSaveRepository> logger
    ): base(dbContextFactory, logger) {}

    protected override GameSaveEntity RuntimeToEntity(GameSave gameSave)
    {
        return gameSave.ToEntity();
    }
}