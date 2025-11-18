namespace PokeGame.Core.Common.Configurations;

public sealed record DefaultStarterSceneLocation
{
    public required int X { get; init; }
    public required int Y { get; init; }
}