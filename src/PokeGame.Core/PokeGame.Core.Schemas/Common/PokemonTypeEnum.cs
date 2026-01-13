using System.Text.Json.Serialization;

namespace PokeGame.Core.Schemas.Common;

[JsonConverter(typeof(JsonStringEnumConverter<PokemonTypeEnum>))]
public enum PokemonTypeEnum
{
    None = 1,
    Bug,
    Dark,
    Dragon,
    Electric,
    Fairy,
    Fighting,
    Fire,
    Flying,
    Ghost,
    Grass,
    Ground,
    Ice,
    Normal,
    Poison,
    Psychic,
    Rock,
    Steel,
    Water,
}
