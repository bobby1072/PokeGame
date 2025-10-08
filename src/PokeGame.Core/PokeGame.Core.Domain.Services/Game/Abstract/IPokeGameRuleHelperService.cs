using PokeGame.Core.Schemas.Game;

namespace PokeGame.Core.Domain.Services.Game.Abstract;

internal interface IPokeGameRuleHelperService
{
    int GetRandomPokemonNumberFromStandardPokedexRange();
    int GetRandomPokemonNumberFromLegendaryPokedexRange();
    OwnedPokemon AddXpToOwnedPokemon(OwnedPokemon ownedPokemon, int xpToAdd);
    OwnedPokemon RefillOwnedPokemonHp(OwnedPokemon ownedPokemon);
}
