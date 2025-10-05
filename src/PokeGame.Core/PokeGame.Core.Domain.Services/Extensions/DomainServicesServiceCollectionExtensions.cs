using System.Reflection;
using BT.Common.Api.Helpers.Models;
using BT.Common.Helpers.Extensions;
using BT.Common.Services.Extensions;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using PokeGame.Core.Common;
using PokeGame.Core.Domain.Services.Abstract;
using PokeGame.Core.Domain.Services.Concrete;
using PokeGame.Core.Domain.Services.Game.Abstract;
using PokeGame.Core.Domain.Services.Game.Commands;
using PokeGame.Core.Domain.Services.Game.Concrete;
using PokeGame.Core.Domain.Services.Pokedex.Abstract;
using PokeGame.Core.Domain.Services.Pokedex.Commands;
using PokeGame.Core.Domain.Services.Pokedex.Concrete;
using PokeGame.Core.Domain.Services.User.Abstract;
using PokeGame.Core.Domain.Services.User.Commands;
using PokeGame.Core.Domain.Services.User.Concrete;
using PokeGame.Core.Persistence.Extensions;
using PokeGame.Core.Schemas.Extensions;

namespace PokeGame.Core.Domain.Services.Extensions;

public static class DomainServicesServiceCollectionExtensions
{
    public static IServiceCollection AddPokeGameApplicationServices(
        this IServiceCollection services,
        IConfiguration configuration,
        IHostEnvironment environment
    )
    {
        var serviceInfoSection = configuration.GetSection(ServiceInfo.Key);

        if (!serviceInfoSection.Exists())
        {
            throw new ArgumentNullException(ServiceInfo.Key);
        }

        var healthCheckBuilder = services.AddHealthChecks();

        services
            .AddHttpClient()
            .AddLogging()
            .AddDistributedCachingService()
            .AddDomainModelValidators()
            .AddPokeGamePersistence(configuration, healthCheckBuilder, environment.IsDevelopment())
            .ConfigureSingletonOptions<ServiceInfo>(serviceInfoSection);

        services
            .AddUserServices()
            .AddPokemonServices(healthCheckBuilder)
            .AddGameServices()
            .AddScoped<IDomainServiceCommandExecutor, DomainServiceCommandExecutor>()
            .AddScoped<IValidatorService, ValidatorService>();

        return services;
    }

    private static IServiceCollection AddGameServices(this IServiceCollection services)
    {
        services
            .AddScoped<CreateNewGameCommand>()
            .AddScoped<GetGameSavesByUserCommand>()
            .AddScoped<StartGameSessionCommand>()
            .AddScoped<RemoveGameSessionCommand>()
            .AddScoped<IGameSessionProcessingManager, GameSessionProcessingManager>()
            .AddScoped<IGameSaveProcessingManager, GameSaveProcessingManager>();
        
        return services;
    }
    private static IServiceCollection AddUserServices(this IServiceCollection services)
    {
        services
            .AddScoped<SaveUserCommand>()
            .AddScoped<GetUserByEmailCommand>()
            .AddScoped<GetUserByIdCommand>()
            .AddScoped<IUserProcessingManager, UserProcessingManager>();

        return services;
    }

    private static IServiceCollection AddPokemonServices(
        this IServiceCollection services,
        IHealthChecksBuilder healthCheckBuilder
    )
    {
        services
            .AddPokedexJsonDoc()
            .AddScoped<CreateDbPokedexPokemonCommand>()
            .AddScoped<GetDbPokedexPokemonCommand>()
            .AddSingleton<IPokedexDataMigratorHealthCheck, PokedexDataMigratorHealthCheck>()
            .AddHostedService<PokedexDataMigratorHostedService>();

        services
            .AddHttpClient<IPokeApiClient, PokeApiClient>();
        
        healthCheckBuilder.AddCheck<IPokedexDataMigratorHealthCheck>(
            nameof(PokedexDataMigratorHealthCheck)
        );

        return services;
    }

    private static IServiceCollection AddPokedexJsonDoc(this IServiceCollection services)
    {
        var baseServicesDomain =
            Path.GetDirectoryName(Assembly.GetExecutingAssembly().Location) ?? string.Empty;
        var foundFilePath = Path.Combine(baseServicesDomain, "Pokedex", "Data", "Pokedex.json");

        if (string.IsNullOrEmpty(foundFilePath))
        {
            throw new ArgumentNullException(nameof(foundFilePath));
        }

        services.AddSingleton<IPokedexJsonFactory>(sp =>
        {
            var loggerFactory = sp.GetRequiredService<ILoggerFactory>();

            return new PokedexJsonFactory(
                foundFilePath,
                loggerFactory.CreateLogger<PokedexJsonFactory>()
            );
        });

        services.AddKeyedSingleton(
            Constants.ServiceKeys.PokedexJsonFile,
            (sp, _) =>
            {
                var pokedexFileControllerService = sp.GetRequiredService<IPokedexJsonFactory>();

                return pokedexFileControllerService
                    .GetPokedexJsonDocAsync()
                    .GetAwaiter()
                    .GetResult();
            }
        );

        return services;
    }
}
