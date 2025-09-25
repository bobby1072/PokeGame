using PokeApiNet;
using PokeGame.Core.Domain.Services.Pokemon.Commands;

namespace PokeGame.Core.Domain.Services.Pokemon.Abstract;

internal interface IGetPokeApiResourceByNameCommandFactory
{
    GetPokeApiResourceByNameCommand<TResource> CreateCommand<TResource>() where TResource: NamedApiResource;
}