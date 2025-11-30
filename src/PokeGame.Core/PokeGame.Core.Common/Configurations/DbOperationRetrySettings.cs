using BT.Common.Polly.Models.Concrete;

namespace PokeGame.Core.Common.Configurations;

public sealed record DbOperationRetrySettings : PollyRetrySettings
{
    public static string Key = nameof(DbOperationRetrySettings);
}