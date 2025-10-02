using Microsoft.Extensions.Diagnostics.HealthChecks;

namespace PokeGame.Core.Domain.Services.Pokemon.Abstract;

public interface IPokedexDataMigratorHealthCheck: IHealthCheck
{
    public void SetDatabaseSeeded(bool isMigrated);
}