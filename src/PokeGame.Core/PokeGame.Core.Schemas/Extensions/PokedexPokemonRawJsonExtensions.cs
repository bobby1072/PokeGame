using BT.Common.FastArray.Proto;
using PokeGame.Core.Schemas.Pokedex;

namespace PokeGame.Core.Schemas.Extensions;

public static class PokedexPokemonRawJsonExtensions
{
    public static PokedexPokemon ToRuntimeModel(this PokedexPokemonRawJson pokedexPokemonRawJson)
    {
        var foundFirstType = pokedexPokemonRawJson.Type.FastArrayFirstOrDefault();
        var foundSecondType = pokedexPokemonRawJson.Type.FastArraySecondOrDefault();

        return new PokedexPokemon
        {
            Id = pokedexPokemonRawJson.Id,
            EnglishName = pokedexPokemonRawJson.Name.English,
            FrenchName = pokedexPokemonRawJson.Name.French,
            ChineseName = pokedexPokemonRawJson.Name.Chinese,
            JapaneseName = pokedexPokemonRawJson.Name.Japanese,
            Stats = pokedexPokemonRawJson.Base.ToRuntimeModel(),
            Type = new PokedexPokemonType
            {
                Type1 = foundFirstType is null
                    ? PokemonType.None
                    : Enum.Parse<PokemonType>(foundFirstType),
                Type2 = foundSecondType is null ? null : Enum.Parse<PokemonType>(foundSecondType),
            },
        };
    }

    public static PokedexPokemonStats ToRuntimeModel(
        this PokedexPokemonStatsRawJson pokedexPokemonStatsRawJson
    )
    {
        return new PokedexPokemonStats
        {
            Hp = pokedexPokemonStatsRawJson.Hp,
            Attack = pokedexPokemonStatsRawJson.Attack,
            Defence = pokedexPokemonStatsRawJson.Defence,
            Speed = pokedexPokemonStatsRawJson.Speed,
            SpecialAttack = pokedexPokemonStatsRawJson.SpecialAttack,
            SpecialDefence = pokedexPokemonStatsRawJson.SpecialDefence,
        };
    }
}
