using System.Text.Json.Serialization;

namespace PokeGame.Core.Schemas.Pokedex;

public sealed record PokedexPokemonNameRawJson
{
    [JsonPropertyName("english")]
    public required string English { get; init; }
    [JsonPropertyName("french")]
    public required string French { get; init; }
    [JsonPropertyName("chinese")]
    public required string Chinese { get; init; }
    [JsonPropertyName("japanese")]
    public required string Japanese { get; init; }
}