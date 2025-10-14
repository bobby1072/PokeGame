using System.Text.Json;
using BT.Common.FastArray.Proto;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Diagnostics.HealthChecks;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using PokeGame.Core.Common;
using PokeGame.Core.Domain.Services.Pokedex.Abstract;
using PokeGame.Core.Persistence.Configurations;
using PokeGame.Core.Persistence.Migrations.Abstract;
using PokeGame.Core.Schemas;
using PokeGame.Core.Schemas.Extensions;
using PokeGame.Core.Schemas.Pokedex;

namespace PokeGame.Core.Domain.Services.Pokedex.Concrete;

internal sealed class PokedexDataMigratorHostedService : BackgroundService
{
    private readonly IServiceScopeFactory _scopeFactory;
    private readonly IDatabaseMigratorHealthCheck _databaseMigratorHealthCheck;
    private readonly IPokedexDataMigratorHealthCheck _pokedexDataMigratorHealthCheck;
    private readonly JsonDocument _pokedexJsonFile;
    private readonly DbMigrationSettings _dbMigrationSettings;
    private readonly ILogger<PokedexDataMigratorHostedService> _logger;

    public PokedexDataMigratorHostedService(
        IServiceScopeFactory scopeFactory,
        IDatabaseMigratorHealthCheck databaseMigratorHealthCheck,
        IPokedexDataMigratorHealthCheck pokedexDataMigratorHealthCheck,
        [FromKeyedServices(Constants.ServiceKeys.PokedexJsonFile)] JsonDocument pokedexJsonFile,
        DbMigrationSettings dbMigrationSettings,
        ILogger<PokedexDataMigratorHostedService> logger
    )
    {
        _scopeFactory = scopeFactory;
        _databaseMigratorHealthCheck = databaseMigratorHealthCheck;
        _pokedexDataMigratorHealthCheck = pokedexDataMigratorHealthCheck;
        _pokedexJsonFile = pokedexJsonFile;
        _dbMigrationSettings = dbMigrationSettings;
        _logger = logger;
    }

    protected override async Task ExecuteAsync(CancellationToken stoppingToken)
    {
        _logger.LogInformation("PokedexDataMigratorHostedService starting...");

        if (!_dbMigrationSettings.DoMigration)
        {
            _pokedexDataMigratorHealthCheck.SetDatabaseSeeded(true);
            _logger.LogInformation("Migrations are disabled...");
            return;
        }

        // Wait for database migration to complete
        while (
            (
                await _databaseMigratorHealthCheck.CheckHealthAsync(
                    new HealthCheckContext(),
                    stoppingToken
                )
            ).Status != HealthStatus.Healthy
            && !stoppingToken.IsCancellationRequested
        )
        {
            _logger.LogInformation("Waiting for database migration to complete...");
            await Task.Delay(TimeSpan.FromSeconds(0.5), stoppingToken);
        }

        if (stoppingToken.IsCancellationRequested)
        {
            _logger.LogWarning(
                "PokedexDataMigratorHostedService was cancelled before database migration completed"
            );
            return;
        }

        _logger.LogInformation("Database migration completed. Starting Pokedex data seeding...");

        await SeedPokedexDataAsync(stoppingToken);

        _pokedexDataMigratorHealthCheck.SetDatabaseSeeded(true);

        _logger.LogInformation("Pokedex data seeding completed successfully");
    }

    private async Task SeedPokedexDataAsync(CancellationToken cancellationToken)
    {
        PokedexPokemon[] pokedexPokemonList;
        try
        {
            pokedexPokemonList =
                _pokedexJsonFile
                    .Deserialize<PokedexPokemonRawJson[]>()
                    ?.FastArraySelect(x => x.ToRuntimeModel())
                    .ToArray() ?? [];
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error occurred while deserializing pokedex json");
            throw;
        }
        await using var scope = _scopeFactory.CreateAsyncScope();
        var pokedexService = scope.ServiceProvider.GetRequiredService<IPokedexService>();

        if (pokedexPokemonList.Length > 0)
        {
            await pokedexService.CreatePokedexPokemonAsync(pokedexPokemonList, cancellationToken);
        }
        else
        {
            throw new InvalidOperationException(
                "No Pokemon records were parsed/found in JSON file"
            );
        }
    }
}
