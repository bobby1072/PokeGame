using PokeApiNet;
using PokeGame.Core.Domain.Services.Pokedex.Commands;

namespace PokeGame.Core.Domain.Services.Pokedex.Abstract;

internal interface IGetPokeApiResourceByNameCommandFactory
{
    GetPokeApiResourceByNameCommand<TResource> CreateCommand<TResource>() where TResource: NamedApiResource;
}