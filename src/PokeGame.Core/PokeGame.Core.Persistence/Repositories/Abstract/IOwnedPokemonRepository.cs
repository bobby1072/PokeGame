using BT.Common.Persistence.Shared.Repositories.Abstract;
using PokeGame.Core.Persistence.Entities;
using PokeGame.Core.Schemas.Game;

namespace PokeGame.Core.Persistence.Repositories.Abstract;

public interface IOwnedPokemonRepository : IRepository<OwnedPokemonEntity, Guid?, OwnedPokemon>
{ }