﻿using BT.Common.Persistence.Shared.Repositories.Abstract;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using PokeGame.Core.Persistence.Contexts;
using PokeGame.Core.Persistence.Entities;
using PokeGame.Core.Persistence.Entities.Extensions;
using PokeGame.Core.Persistence.Repositories.Abstract;
using PokeGame.Core.Schemas;

namespace PokeGame.Core.Persistence.Repositories.Concrete;

internal sealed class PokedexPokemonRepository: BaseRepository<PokedexPokemonEntity, int, PokedexPokemon, PokeGameContext>, IPokedexPokemonRepository
{
    public PokedexPokemonRepository(
        IDbContextFactory<PokeGameContext> dbContextFactory,
        ILogger<PokedexPokemonRepository> logger
    ): base(dbContextFactory, logger) {}

    protected override PokedexPokemonEntity RuntimeToEntity(PokedexPokemon runtimeObj)
    {
        return runtimeObj.ToEntity();
    }
}