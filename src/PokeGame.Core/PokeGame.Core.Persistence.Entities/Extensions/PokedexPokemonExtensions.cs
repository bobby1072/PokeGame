using PokeGame.Core.Schemas.Pokedex;

namespace PokeGame.Core.Persistence.Entities.Extensions;

public static class PokedexPokemonExtensions
{
    public static PokedexPokemonEntity ToEntity(this PokedexPokemon pokemon)
    {
        return new PokedexPokemonEntity
        {
            Id = pokemon.Id,
            Attack = pokemon.Stats.Attack,
            Defence = pokemon.Stats.Defence,
            Speed = pokemon.Stats.Speed,
            SpecialAttack = pokemon.Stats.SpecialAttack,
            SpecialDefence = pokemon.Stats.SpecialDefence,
            Hp = pokemon.Stats.Hp,
            Type1 = pokemon.Type.Type1.ToString(),
            Type2 = pokemon.Type.Type2?.ToString(),
            ChineseName = pokemon.ChineseName,
            EnglishName = pokemon.EnglishName,
            FrenchName = pokemon.FrenchName,
            JapaneseName = pokemon.JapaneseName,
        };
    }
}
