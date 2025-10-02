using BT.Common.Persistence.Shared.Repositories.Abstract;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using PokeGame.Core.Persistence.Contexts;
using PokeGame.Core.Persistence.Entities;
using PokeGame.Core.Persistence.Entities.Extensions;
using PokeGame.Core.Persistence.Repositories.Abstract;
using PokeGame.Core.Schemas;
using PokeGame.Core.Schemas.Game;

namespace PokeGame.Core.Persistence.Repositories.Concrete;

internal sealed class OwnedPokemonRepository : BaseRepository<OwnedPokemonEntity, Guid?, OwnedPokemon, PokeGameContext>, IOwnedPokemonRepository
{
    public OwnedPokemonRepository(
        IDbContextFactory<PokeGameContext> dbContextFactory,
        ILogger<OwnedPokemonRepository> logger
    ) : base(dbContextFactory, logger) { }

    protected override OwnedPokemonEntity RuntimeToEntity(OwnedPokemon ownedPokemon)
    {
        return ownedPokemon.ToEntity();
    }
}