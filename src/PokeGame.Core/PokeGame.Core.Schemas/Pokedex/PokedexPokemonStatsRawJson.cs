using System.Text.Json.Serialization;

namespace PokeGame.Core.Schemas.Pokedex;

public sealed record PokedexPokemonStatsRawJson
{
    [JsonPropertyName("HP")]
    public int Hp { get; init; }

    [JsonPropertyName("Attack")]
    public int Attack { get; init; }

    [JsonPropertyName("Defense")]
    public int Defence { get; init; }

    [JsonPropertyName("Sp. Attack")]
    public int SpecialAttack { get; init; }

    [JsonPropertyName("Sp. Defense")]
    public int SpecialDefence { get; init; }

    [JsonPropertyName("Speed")]
    public int Speed { get; init; }
}
