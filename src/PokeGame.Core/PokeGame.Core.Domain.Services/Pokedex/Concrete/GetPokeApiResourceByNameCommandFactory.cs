using PokeApiNet;
using PokeGame.Core.Domain.Services.Pokedex.Abstract;
using PokeGame.Core.Domain.Services.Pokedex.Commands;

namespace PokeGame.Core.Domain.Services.Pokedex.Concrete;

internal sealed class GetPokeApiResourceByNameCommandFactory : IGetPokeApiResourceByNameCommandFactory
{
    private readonly IAdvancedPokeApiClient _pokeApiClient;
    public GetPokeApiResourceByNameCommandFactory(IAdvancedPokeApiClient pokeApiClient)
    {
        _pokeApiClient = pokeApiClient;
    }
    
    public GetPokeApiResourceByNameCommand<TResource> CreateCommand<TResource>() where TResource : NamedApiResource => new(_pokeApiClient);
}