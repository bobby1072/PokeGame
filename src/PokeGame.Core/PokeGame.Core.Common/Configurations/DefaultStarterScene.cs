namespace PokeGame.Core.Common.Configurations;

public sealed record DefaultStarterScene
{
    public required string SceneName { get; init; }
    public required DefaultStarterSceneLocation  SceneLocation { get; init; }
}