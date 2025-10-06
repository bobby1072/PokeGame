namespace PokeGame.Core.Common.Configurations;

public sealed record LegendaryPokemonPokedexRange
{
    public required int Min { get; init; }
    public required int Max { get; init; }
}