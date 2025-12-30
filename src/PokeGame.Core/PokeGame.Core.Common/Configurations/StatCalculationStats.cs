namespace PokeGame.Core.Common.Configurations;

public sealed record StatCalculationStats
{
    public required int DefaultIV { get; init; }
    public required int DefaultEV { get; init; }
}