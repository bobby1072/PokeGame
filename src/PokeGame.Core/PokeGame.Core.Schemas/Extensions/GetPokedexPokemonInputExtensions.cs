using PokeGame.Core.Schemas.Input;

namespace PokeGame.Core.Schemas.Extensions;

public static class GetPokedexPokemonInputExtensions
{
    public static bool HasInputProperties(this GetPokedexPokemonInput input)
    {
        return input.Id is not null 
               || input.EnglishName is not null
               || input.FrenchName is not null
               || input.ChineseName is not null
               || input.JapaneseName is not null;
    }

    public static Dictionary<string, object?> ToDictionary(this GetPokedexPokemonInput input)
    {
        return new Dictionary<string, object?>
        {
            { nameof(GetPokedexPokemonInput.Id), input.Id },
            { nameof(GetPokedexPokemonInput.EnglishName), input.EnglishName },
            { nameof(GetPokedexPokemonInput.FrenchName), input.FrenchName },
            { nameof(GetPokedexPokemonInput.JapaneseName), input.JapaneseName },
            { nameof(GetPokedexPokemonInput.ChineseName), input.ChineseName }
        };
    }
}