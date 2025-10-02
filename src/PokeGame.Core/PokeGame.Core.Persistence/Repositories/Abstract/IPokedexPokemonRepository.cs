using BT.Common.Persistence.Shared.Repositories.Abstract;
using PokeGame.Core.Persistence.Entities;
using PokeGame.Core.Schemas;
using PokeGame.Core.Schemas.Pokedex;

namespace PokeGame.Core.Persistence.Repositories.Abstract;

public interface IPokedexPokemonRepository: IRepository<PokedexPokemonEntity, int, PokedexPokemon>
{ }