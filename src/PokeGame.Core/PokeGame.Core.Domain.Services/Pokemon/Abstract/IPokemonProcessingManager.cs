using PokeApiNet;

namespace PokeGame.Core.Domain.Services.Pokemon.Abstract;

public interface IPokemonProcessingManager
{
    Task<TResource> GetPokeApiResourceByNameAsync<TResource>(string name,
        CancellationToken cancellationToken = default) where TResource : NamedApiResource;
}