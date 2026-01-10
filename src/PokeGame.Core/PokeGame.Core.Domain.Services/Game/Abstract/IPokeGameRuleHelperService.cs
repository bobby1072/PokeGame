using PokeGame.Core.Common.GameInformationData;
using PokeGame.Core.Schemas.Game;
using PokeGame.Core.Schemas.Game.PokemonRelated;
using PokeGame.Core.Schemas.PokeApi;

namespace PokeGame.Core.Domain.Services.Game.Abstract;

internal interface IPokeGameRuleHelperService
{
    (
        string? MoveOneResourceName,
        string? MoveTwoResourceName,
        string? MoveThreeResourceName,
        string? MoveFourResourceName
    ) GetRandomMoveSetFromPokemon(Pokemon pokemonSpecies, int level);
    OwnedPokemon AddXpToOwnedPokemon(OwnedPokemon ownedPokemon, int xpToAdd);
    OwnedPokemon RefillOwnedPokemonHp(OwnedPokemon ownedPokemon);
    int GetRandomNumberFromIntRange(IntRange intRange);
    int? GetRandomPokedexNumberFromIntRangeWithRandomEncounterIncluded(IntRange intRange);
    int CalculateStat(int level, int baseStat);
    int GetPokemonMaxHp(Pokemon poke, int pokemonLevel);
}
