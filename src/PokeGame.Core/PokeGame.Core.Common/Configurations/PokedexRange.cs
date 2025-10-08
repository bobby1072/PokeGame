namespace PokeGame.Core.Common.Configurations;

public sealed record PokedexRange
{
    public required int Min { get; init; }
    public required int Max { get; init; }
    public IReadOnlyCollection<int> Extras { get; init; } = [];
}