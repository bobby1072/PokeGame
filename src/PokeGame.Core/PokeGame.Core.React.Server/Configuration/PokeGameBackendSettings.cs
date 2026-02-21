namespace PokeGame.Core.React.Server.Configuration;

internal sealed record PokeGameBackendSettings
{
    public required string PokeGameApiProdUri { get; init; }
    public required string PokeGameSignalRProdUri { get; init; }
}