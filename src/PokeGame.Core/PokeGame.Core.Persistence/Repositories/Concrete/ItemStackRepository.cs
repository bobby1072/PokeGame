using BT.Common.Persistence.Shared.Repositories.Abstract;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using PokeGame.Core.Persistence.Contexts;
using PokeGame.Core.Persistence.Entities;
using PokeGame.Core.Persistence.Entities.Extensions;
using PokeGame.Core.Persistence.Repositories.Abstract;
using PokeGame.Core.Schemas;

namespace PokeGame.Core.Persistence.Repositories.Concrete;

internal sealed class ItemStackRepository : BaseRepository<ItemStackEntity, Guid?, ItemStack, PokeGameContext>, IItemStackRepository
{
    public ItemStackRepository(
        IDbContextFactory<PokeGameContext> dbContextFactory,
        ILogger<ItemStackRepository> logger
    ) : base(dbContextFactory, logger) { }

    protected override ItemStackEntity RuntimeToEntity(ItemStack itemStack)
    {
        return itemStack.ToEntity();
    }
}