using System.Text.Json.Serialization;

namespace PokeGame.Core.Schemas.Common;

[JsonConverter(typeof(JsonStringEnumConverter<PokemonStatEnum>))]
public enum PokemonStatEnum
{
    HP = 1,
    Attack,
    Defense,
    SpecialAttack,
    SpecialDefense,
    Speed,
}
