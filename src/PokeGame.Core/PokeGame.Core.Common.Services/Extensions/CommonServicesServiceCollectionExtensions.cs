using System.Text.Json;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using PokeGame.Core.Common.Services.Abstract;
using PokeGame.Core.Common.Services.Concrete;

namespace PokeGame.Core.Common.Services.Extensions;

public static class CommonServicesServiceCollectionExtensions
{
    public static IServiceCollection AddCommonServices(this IServiceCollection services, IConfiguration config)
    {
        var foundFilePath = config.GetValue<string>("PokedexJsonFilePath")?.Replace("/", Path.DirectorySeparatorChar.ToString());

        if (string.IsNullOrEmpty(foundFilePath))
        {
            throw new ArgumentNullException(nameof(foundFilePath));
        }
        
        services.AddKeyedSingleton(Constants.ServiceKeys.PokedexJsonFilePath, foundFilePath);
        
        services.AddSingleton<IPokedexJsonFileControllerService, PokedexJsonFileControllerService>();

        services.AddKeyedSingleton(
            Constants.ServiceKeys.PokedexJsonFile,
            (sp, _) =>
            {
                var pokedexFileControllerService =
                    sp.GetRequiredService<IPokedexJsonFileControllerService>();

                return pokedexFileControllerService.GetPokedexJsonDocAsync().GetAwaiter().GetResult();
            }
        );

        return services;
    }
}
