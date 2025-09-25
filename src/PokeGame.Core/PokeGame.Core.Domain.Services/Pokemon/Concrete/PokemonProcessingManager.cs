using Microsoft.Extensions.DependencyInjection;
using PokeApiNet;
using PokeGame.Core.Domain.Services.Abstract;
using PokeGame.Core.Domain.Services.Models;
using PokeGame.Core.Domain.Services.Pokemon.Abstract;
using PokeGame.Core.Domain.Services.Pokemon.Commands;

namespace PokeGame.Core.Domain.Services.Pokemon.Concrete;

internal sealed class PokemonProcessingManager: IPokemonProcessingManager
{
    private readonly IScopedDomainServiceCommandExecutor _scopedDomainServiceCommandExecutor;

    public PokemonProcessingManager(IScopedDomainServiceCommandExecutor scopedDomainServiceCommandExecutor)
    {
        _scopedDomainServiceCommandExecutor = scopedDomainServiceCommandExecutor;
    }

    public async Task<TResource> GetPokeApiResourceByNameAsync<TResource>(string name,
        CancellationToken cancellationToken = default) where TResource : NamedApiResource
        => (await _scopedDomainServiceCommandExecutor
            .RunCommandAsync<GetPokeApiResourceByNameCommand<TResource>, string, DomainCommandResult<TResource>>(name,
                sp => sp.GetRequiredService<GetPokeApiResourceByNameCommandFactory>().CreateCommand<TResource>())).CommandResult;
}