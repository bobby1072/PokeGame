using BT.Common.Services.Abstract;
using BT.Common.Services.Models;
using Microsoft.Extensions.Logging;
using PokeApiNet;
using PokeGame.Core.Domain.Services.Abstract;

namespace PokeGame.Core.Domain.Services.Concrete;

internal sealed class PokeApiClient : PokeApiNet.PokeApiClient, IPokeApiClient
{
    private readonly ILogger<PokeApiClient> _logger;
    private readonly ICachingService _cachingService;
    public PokeApiClient(ILogger<PokeApiClient> logger, ICachingService cachingService, HttpClient httpClient) : base(httpClient)
    {
        _logger = logger;
        _cachingService = cachingService;
    }

    public async Task<T> GetApiResourceAsync<T>(string name, CancellationToken cancellationToken)
        where T : NamedApiResource
    {
        var resourceType = typeof(T);

        var resourceCacheKey = GetApiResourceCacheKey(resourceType.Name, name);

        var foundResourceFromCache = await _cachingService.TryGetObjectAsync<T>(resourceCacheKey, cancellationToken);

        if (foundResourceFromCache is not null)
        {
            _logger.LogDebug("Found resource {ResourceTypeName} with url: {Url} in cache", resourceType.Name, name);

            return foundResourceFromCache;
        }

        _logger.LogDebug("Making request to get api resource of: {ResourceType} with name: {Name}",
            resourceType.Name,
            name);

        var apiResult = await GetResourceAsync<T>(name, cancellationToken);

        await _cachingService.SetObjectAsync(resourceCacheKey, apiResult, CacheObjectTimeToLiveInSeconds.OneHour, cancellationToken);

        return apiResult;
    }

    public async Task<T> GetApiResourceAsync<T>(UrlNavigation<T> urlResource, CancellationToken cancellationToken)
        where T : ResourceBase
    {
        var resourceType = typeof(T);

        var resourceCacheKey = GetApiResourceCacheKey(resourceType.Name, urlResource.Url);

        var foundResourceFromCache = await _cachingService.TryGetObjectAsync<T>(resourceCacheKey, cancellationToken);

        if (foundResourceFromCache is not null)
        {
            _logger.LogDebug("Found resource {ResourceTypeName} with url: {Url} in cache", resourceType.Name, urlResource.Url);

            return foundResourceFromCache;
        }

        _logger.LogDebug("Making request to get api resource of: {ResourceType} with url: {Url}", typeof(T).Name, urlResource.Url);

        var apiResult = await GetResourceAsync(urlResource, cancellationToken);

        await _cachingService.SetObjectAsync(resourceCacheKey, apiResult, CacheObjectTimeToLiveInSeconds.OneHour, cancellationToken);

        return apiResult;
    }

    private static string GetApiResourceCacheKey(string resourceType, string searchParamVal)
        => $"{resourceType}.{searchParamVal}";
}