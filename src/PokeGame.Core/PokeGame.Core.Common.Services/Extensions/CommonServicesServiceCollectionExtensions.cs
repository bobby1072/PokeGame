using System.Text.Json;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using PokeGame.Core.Common.Services.Abstract;
using PokeGame.Core.Common.Services.Concrete;

namespace PokeGame.Core.Common.Services.Extensions;

public static class CommonServicesServiceCollectionExtensions
{
    public static IServiceCollection AddCommonServices(this IServiceCollection services)
    {

        services
            .AddSingleton<IPokedexJsonFileControllerService, PokedexJsonFileControllerService>(sp =>
            {
                return new PokedexJsonFileControllerService(
                    $"..{Path.DirectorySeparatorChar}PokeGame.Core.Common.Services{Path.DirectorySeparatorChar}Data{Path.DirectorySeparatorChar}Pokedex.json",
                    sp.GetRequiredService<ILoggerFactory>().CreateLogger<PokedexJsonFileControllerService>()
                );
            });

        services.AddKeyedSingleton<JsonDocument>(Constants.ServiceKeys.PokedexJsonFile, (sp, ob) =>
        {
           var pokedexFileControllerService = sp.GetRequiredService<IPokedexJsonFileControllerService>();
           
           return pokedexFileControllerService.GetPokedexAsync().GetAwaiter().GetResult(); 
        });
        
        return services;
    }
}