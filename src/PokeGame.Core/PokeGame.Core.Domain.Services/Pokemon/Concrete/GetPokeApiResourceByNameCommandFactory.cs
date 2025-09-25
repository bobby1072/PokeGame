using PokeApiNet;
using PokeGame.Core.Domain.Services.Pokemon.Abstract;
using PokeGame.Core.Domain.Services.Pokemon.Commands;

namespace PokeGame.Core.Domain.Services.Pokemon.Concrete;

internal sealed class GetPokeApiResourceByNameCommandFactory : IGetPokeApiResourceByNameCommandFactory
{
    private readonly IAdvancedPokeApiClient _pokeApiClient;
    public GetPokeApiResourceByNameCommandFactory(IAdvancedPokeApiClient pokeApiClient)
    {
        _pokeApiClient = pokeApiClient;
    }
    
    public GetPokeApiResourceByNameCommand<TResource> CreateCommand<TResource>() where TResource : NamedApiResource => new(_pokeApiClient);
}