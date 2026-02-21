namespace PokeGame.Core.React.Server.Configuration;

internal sealed record HstsOptions
{
    public static string Key = nameof(HstsOptions);
    public required bool Preload { get; init; }
    public required bool IncludeSubDomains { get; init; }
    public required int MaxAgeInSeconds { get; init; }
}