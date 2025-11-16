using Microsoft.Extensions.DependencyInjection;
using PokeGame.Core.Common.GameInformationData;

namespace PokeGame.Core.Common.Extensions;

public static class CommonServiceCollectionExtensions
{
    public static IServiceCollection AddGameInformationServices(this IServiceCollection services)
    {
        services
            .AddKeyedSingleton(Constants.ServiceKeys.ValidGameSceneList, 
                (_,_) => new ValidGameSceneList());
        
        
        return services;
    }
}