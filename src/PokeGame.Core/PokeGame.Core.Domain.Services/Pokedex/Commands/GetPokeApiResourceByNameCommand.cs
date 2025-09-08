using PokeApiNet;
using PokeGame.Core.Domain.Services.Abstract;
using PokeGame.Core.Domain.Services.Pokedex.Abstract;

namespace PokeGame.Core.Domain.Services.Pokedex.Commands;

internal sealed class GetPokeApiResourceByNameCommand<TResource>: IDomainCommand<string, TResource> where TResource : NamedApiResource
{
    public string CommandName => nameof(GetPokeApiResourceByNameCommand<TResource>);
    
    private readonly IAdvancedPokeApiClient _advancedPokeApiClient;

    public GetPokeApiResourceByNameCommand(IAdvancedPokeApiClient advancedPokeApiClient)
    {
        _advancedPokeApiClient = advancedPokeApiClient;
    }

    public Task<TResource> ExecuteAsync(string resourceName, CancellationToken cancellationToken = default)
        => _advancedPokeApiClient.GetApiResourceAsync<TResource>(resourceName, cancellationToken);
}