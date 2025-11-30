using BT.Common.Persistence.Shared.Repositories.Abstract;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using PokeGame.Core.Common.Configurations;
using PokeGame.Core.Persistence.Contexts;
using PokeGame.Core.Persistence.Entities;
using PokeGame.Core.Persistence.Entities.Extensions;
using PokeGame.Core.Persistence.Repositories.Abstract;
using PokeGame.Core.Schemas.Game;

namespace PokeGame.Core.Persistence.Repositories.Concrete;

internal sealed class OwnedOwnedItemRepository : BasePokeGameRepository<OwnedItemEntity, Guid?, OwnedItem, PokeGameContext>, IOwnedItemRepository
{
    public OwnedOwnedItemRepository(
        IDbContextFactory<PokeGameContext> dbContextFactory,
        ILogger<OwnedOwnedItemRepository> logger,
        DbOperationRetrySettings retrySettings
    ) : base(dbContextFactory, logger, retrySettings) { }

    protected override OwnedItemEntity RuntimeToEntity(OwnedItem ownedItem)
    {
        return ownedItem.ToEntity();
    }
}