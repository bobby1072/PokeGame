namespace PokeGame.Core.Common.Configurations;

public sealed record HpCalculationStats
{
    public required int DefaultIV { get; init; }
    public required int DefaultEV { get; init; }
}