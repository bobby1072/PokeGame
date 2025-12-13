using PokeGame.Core.Common.GameInformationData;
using PokeGame.Core.Schemas.Game;

namespace PokeGame.Core.Domain.Services.Game.Abstract;

internal interface IPokeGameRuleHelperService
{
    OwnedPokemon AddXpToOwnedPokemon(OwnedPokemon ownedPokemon, int xpToAdd);
    OwnedPokemon RefillOwnedPokemonHp(OwnedPokemon ownedPokemon);
    int GetRandomPokemonNumberFromPokedexRange(IntRange intRange);
    int? GetRandomPokemonNumberFromPokedexRangeWithRandomEncounterIncluded(IntRange intRange);
    int CalculateStat(int level, int baseStat);
}
