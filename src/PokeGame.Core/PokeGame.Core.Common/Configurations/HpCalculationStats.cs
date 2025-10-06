namespace PokeGame.Core.Common.Configurations;

public sealed record HpCalculationStats
{
    public required decimal DefaultIV { get; init; }
    public required decimal DefaultEV { get; init; }
}