namespace PokeGame.Core.Common.Configurations;

public sealed record StandardPokemonPokedexRange
{
    public required int Min { get; init; }
    public required int Max { get; init; }
    public IReadOnlyCollection<int> Extras { get; init; } = [];
}