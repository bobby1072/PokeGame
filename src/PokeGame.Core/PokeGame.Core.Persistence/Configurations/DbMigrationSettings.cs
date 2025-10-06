using BT.Common.Polly.Models.Concrete;

namespace PokeGame.Core.Persistence.Configurations;

public sealed record DbMigrationSettings : PollyRetrySettings
{
    public static readonly string Key = nameof(DbMigrationSettings);
    public required string StartVersion { get; init; }
    public bool DoMigration { get; init; }
}