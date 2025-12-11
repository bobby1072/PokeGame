using PokeGame.Core.Common.Configurations;
using PokeGame.Core.Schemas.Game;

namespace PokeGame.Core.Domain.Services.Game.Abstract;

internal interface IPokeGameRuleHelperService
{
    OwnedPokemon AddXpToOwnedPokemon(OwnedPokemon ownedPokemon, int xpToAdd);
    OwnedPokemon RefillOwnedPokemonHp(OwnedPokemon ownedPokemon);
    int GetRandomPokemonNumberFromPokedexRange(IntRange intRange);
}
