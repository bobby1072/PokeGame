using BT.Common.Polly.Models.Concrete;

namespace PokeGame.Core.Common.Configurations;

public sealed record PokeApiSettings: PollyRetrySettings
{
    public static readonly string Key = nameof(PokeApiSettings);
    public required string BaseUrl { get; init; }
}