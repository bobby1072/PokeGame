namespace PokeGame.Core.Common.GameInformationData;

public sealed record IntRange
{
    public int Min { get; init; }
    public int Max { get; init; }
    public IReadOnlyCollection<int> Extras { get; init; } = [];
}