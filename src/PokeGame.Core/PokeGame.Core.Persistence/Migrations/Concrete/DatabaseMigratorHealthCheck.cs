using Microsoft.Extensions.Diagnostics.HealthChecks;
using PokeGame.Core.Persistence.Migrations.Abstract;

namespace PokeGame.Core.Persistence.Migrations.Concrete;

internal class DatabaseMigratorHealthCheck : IDatabaseMigratorHealthCheck
{
    public bool MigrationCompleted { get; set; } = false;

    public Task<HealthCheckResult> CheckHealthAsync(
        HealthCheckContext context,
        CancellationToken cancellationToken = default)
    {
        return Task.FromResult(MigrationCompleted ? HealthCheckResult.Healthy("The database migrator is finished.") : HealthCheckResult.Unhealthy("The database migrator is still running."));
    }
}