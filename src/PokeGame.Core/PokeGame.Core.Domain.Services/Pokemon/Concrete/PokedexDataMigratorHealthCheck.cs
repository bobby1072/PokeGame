using Microsoft.Extensions.Diagnostics.HealthChecks;
using PokeGame.Core.Domain.Services.Pokemon.Abstract;

namespace PokeGame.Core.Domain.Services.Pokemon.Concrete;

internal sealed class PokedexDataMigratorHealthCheck: IPokedexDataMigratorHealthCheck
{
    private bool _isMigrated;
    
    public void SetDatabaseSeeded(bool isMigrated)
    {
        _isMigrated = isMigrated;
    }

    public Task<HealthCheckResult> CheckHealthAsync(HealthCheckContext context,
        CancellationToken cancellationToken = default)
    {
        return Task.FromResult(_isMigrated ? HealthCheckResult.Healthy() : HealthCheckResult.Unhealthy());
    }
}