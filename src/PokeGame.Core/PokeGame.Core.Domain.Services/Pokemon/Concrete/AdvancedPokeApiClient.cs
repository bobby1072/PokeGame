using Microsoft.Extensions.Logging;
using PokeApiNet;
using PokeGame.Core.Domain.Services.Pokemon.Abstract;

namespace PokeGame.Core.Domain.Services.Pokemon.Concrete;

internal sealed class AdvancedPokeApiClient: PokeApiClient, IAdvancedPokeApiClient
{
    private readonly ILogger<AdvancedPokeApiClient> _logger;

    public AdvancedPokeApiClient(ILogger<AdvancedPokeApiClient> logger)
    {
        _logger = logger;
    }

    public async Task<T> GetApiResourceAsync<T>(string name, CancellationToken cancellationToken)
        where T : NamedApiResource
    {
        _logger.LogInformation("Making request to get api resource of: {ResourceType} with name: {Name}",
            typeof(T).Name,
            name);

        return await GetResourceAsync<T>(name, cancellationToken);
    }

    public async Task<T> GetApiResourceAsync<T>(UrlNavigation<T> urlResource, CancellationToken cancellationToken)
        where T : ResourceBase
    {
        _logger.LogInformation("Making request to get api resource of: {ResourceType}", typeof(T).Name);
        
        return await GetResourceAsync(urlResource, cancellationToken);
    }
}