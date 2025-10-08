namespace PokeGame.Core.Common.Configurations;

public sealed record PokeGameRules
{
    public static readonly string Key = nameof(PokeGameRules);
    public required decimal XpMultiplier { get; init; }
    public required decimal LegendaryXpMultiplier { get; init; }
    public required int BaseXpCeiling { get; init; }
    public required HpCalculationStats HpCalculationStats { get; init; }
    public required PokedexRange StandardPokemonPokedexRange { get; init; }
    public required PokedexRange LegendaryPokemonPokedexRange { get; init; }
}