using PokeGame.Core.Schemas.Input;
using PokeGame.Core.Schemas.Pokedex;

namespace PokeGame.Core.Domain.Services.Pokedex.Abstract;

internal interface IPokedexService
{
    Task<IReadOnlyCollection<PokedexPokemon>> CreatePokedexPokemonAsync(IReadOnlyCollection<PokedexPokemon> pokemonToCreate, CancellationToken cancellationToken = default);
    Task<IReadOnlyCollection<PokedexPokemon>> GetPokedexPokemonAsync(GetPokedexPokemonInput input, CancellationToken cancellationToken = default);
}