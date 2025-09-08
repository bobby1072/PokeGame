using System.Text.Json;
using BT.Common.FastArray.Proto;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using PokeGame.Core.Common;
using PokeGame.Core.Domain.Services.Abstract;
using PokeGame.Core.Domain.Services.Pokedex.Commands;
using PokeGame.Core.Persistence.Migrations.Abstract;
using PokeGame.Core.Schemas;
using PokeGame.Core.Schemas.Extensions;

namespace PokeGame.Core.Domain.Services.Pokedex.Concrete;

internal sealed class PokedexDataMigratorHostedService : BackgroundService
{
    private readonly IServiceScopeFactory _scopeFactory;
    private readonly IDatabaseMigratorHealthCheck _databaseMigratorHealthCheck;
    private readonly JsonDocument _pokedexJsonFile;
    private readonly ILogger<PokedexDataMigratorHostedService> _logger;

    public PokedexDataMigratorHostedService(
        IServiceScopeFactory scopeFactory,
        IDatabaseMigratorHealthCheck databaseMigratorHealthCheck,
        [FromKeyedServices(Constants.ServiceKeys.PokedexJsonFile)] JsonDocument pokedexJsonFile,
        ILogger<PokedexDataMigratorHostedService> logger)
    {
        _scopeFactory = scopeFactory;
        _databaseMigratorHealthCheck = databaseMigratorHealthCheck;
        _pokedexJsonFile = pokedexJsonFile;
        _logger = logger;
    }

    protected override async Task ExecuteAsync(CancellationToken stoppingToken)
    {
        _logger.LogInformation("PokedexDataMigratorHostedService starting...");

        // Wait for database migration to complete
        while (!_databaseMigratorHealthCheck.MigrationCompleted && !stoppingToken.IsCancellationRequested)
        {
            _logger.LogInformation("Waiting for database migration to complete...");
            await Task.Delay(TimeSpan.FromSeconds(2), stoppingToken);
        }

        if (stoppingToken.IsCancellationRequested)
        {
            _logger.LogWarning("PokedexDataMigratorHostedService was cancelled before database migration completed");
            return;
        }

        _logger.LogInformation("Database migration completed. Starting Pokedex data seeding...");

        await SeedPokedexDataAsync(stoppingToken);
        _logger.LogInformation("Pokedex data seeding completed successfully");
    }

    private async Task SeedPokedexDataAsync(CancellationToken cancellationToken)
    {
        PokedexPokemon[] pokedexPokemonList;
        try
        {
            pokedexPokemonList = _pokedexJsonFile
                .Deserialize<PokedexPokemonRawJson[]>()
                ?.FastArraySelect(x => x.ToRuntimeModel())
                .ToArray() ?? [];
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error occurred while seeding Pokedex data");

            throw;
        }
        await using var scope = _scopeFactory.CreateAsyncScope();
        var commandExecutor = scope.ServiceProvider.GetRequiredService<IScopedDomainServiceCommandExecutor>();
        
        if (pokedexPokemonList.Length > 0)
        {
            await commandExecutor.RunCommandAsync<CreateDbPokedexPokemonCommand, IReadOnlyCollection<PokedexPokemon>, IReadOnlyCollection<PokedexPokemon>>(pokedexPokemonList);
        }
        else
        {
            throw new InvalidOperationException("No Pokemon records were parsed/found in JSON file");
        }
    }
}
