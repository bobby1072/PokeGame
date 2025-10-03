using PokeApiNet;

namespace PokeGame.Core.Domain.Services.Abstract;

internal interface IPokeApiClient : IDisposable
{
    Task<T> GetApiResourceAsync<T>(string name, CancellationToken cancellationToken)
        where T : NamedApiResource;

    Task<T> GetApiResourceAsync<T>(UrlNavigation<T> urlResource, CancellationToken cancellationToken)
        where T : ResourceBase;
}