using System.Text.Json.Serialization;

namespace PokeGame.Core.Schemas;

public sealed record PokedexPokemonRawJson
{
    [JsonPropertyName("id")]
    public required int Id { get; init; }
    [JsonPropertyName("name")]
    public required PokedexPokemonNameRawJson Name { get; init; }
    [JsonPropertyName("type")]
    public required IReadOnlyCollection<string> Type { get; init; }
    [JsonPropertyName("base")]
    public required PokedexPokemonStatsRawJson Base { get; init; }
}