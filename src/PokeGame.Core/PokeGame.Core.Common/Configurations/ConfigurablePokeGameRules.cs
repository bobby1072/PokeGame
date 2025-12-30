namespace PokeGame.Core.Common.Configurations;

public sealed record ConfigurablePokeGameRules
{
    public const string Key = "PokeGameRules";
    public required decimal XpMultiplier { get; init; }
    public required decimal LegendaryXpMultiplier { get; init; }
    public required int BaseXpCeiling { get; init; }
    public required int RandomPokemonEncounterLikelyhoodPercentage { get; init; }
    public required StatCalculationStats StatCalculationStats { get; init; }
}
