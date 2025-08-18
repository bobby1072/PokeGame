using System.Reflection;
using BT.Common.Helpers.Extensions;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using PokeGame.Core.Common;
using PokeGame.Core.Common.Configurations;
using PokeGame.Core.Domain.Services.Abstract;
using PokeGame.Core.Domain.Services.Concrete;
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

        services
            .AddHttpClient()
            .AddLogging()
            .AddDomainModelValidators()
            .AddPokeGamePersistence(configuration, environment.IsDevelopment())
            .ConfigureSingletonOptions<ServiceInfo>(serviceInfoSection);

        services
            .AddPokedexJsonDoc(configuration)
            .AddScoped<CreatePokedexPokemonCommand>()
            .AddScoped<GetPokedexPokemonCommand>()
            .AddScoped<SaveUserCommand>()
            .AddScoped<GetUserByEmailCommand>()
            .AddScoped<IScopedDomainServiceCommandExecutor, ScopedDomainServiceCommandExecutor>()
            .AddScoped<IUserProcessingManager, UserProcessingManager>()
            .AddHostedService<PokedexDataMigratorHostedService>();

        return services;
    }

    private static IServiceCollection AddPokedexJsonDoc(this IServiceCollection services, IConfiguration configuration)
    {
        var baseServicesDomain = Path.GetDirectoryName(Assembly.GetExecutingAssembly().Location) ?? string.Empty;
        var foundFilePath = Path.Combine(baseServicesDomain, "Pokedex", "Data", "Pokedex.json");;

        if (string.IsNullOrEmpty(foundFilePath))
        {
            throw new ArgumentNullException(nameof(foundFilePath));
        }
        
        services.AddKeyedSingleton(Constants.ServiceKeys.PokedexJsonFilePath, foundFilePath);
        
        services.AddSingleton<IPokedexJsonFactory, PokedexJsonFactory>();

        services.AddKeyedSingleton(
            Constants.ServiceKeys.PokedexJsonFile,
            (sp, _) =>
            {
                var pokedexFileControllerService =
                    sp.GetRequiredService<IPokedexJsonFactory>();

                return pokedexFileControllerService.GetPokedexJsonDocAsync().GetAwaiter().GetResult();
            }
        );
        
        return services;
    }
}
