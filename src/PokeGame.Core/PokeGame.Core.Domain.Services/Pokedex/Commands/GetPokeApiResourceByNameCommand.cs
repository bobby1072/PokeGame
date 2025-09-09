using PokeApiNet;
using PokeGame.Core.Domain.Services.Abstract;
using PokeGame.Core.Domain.Services.Models;
using PokeGame.Core.Domain.Services.Pokedex.Abstract;

namespace PokeGame.Core.Domain.Services.Pokedex.Commands;

internal sealed class GetPokeApiResourceByNameCommand<TResource>: IDomainCommand<string, DomainCommandResult<TResource>> where TResource : NamedApiResource
{
    public string CommandName => nameof(GetPokeApiResourceByNameCommand<TResource>);
    
    private readonly IAdvancedPokeApiClient _advancedPokeApiClient;

    public GetPokeApiResourceByNameCommand(IAdvancedPokeApiClient advancedPokeApiClient)
    {
        _advancedPokeApiClient = advancedPokeApiClient;
    }

    public async Task<DomainCommandResult<TResource>> ExecuteAsync(string email, CancellationToken cancellationToken = default)
    {
        var result = await _advancedPokeApiClient.GetApiResourceAsync<TResource>(email, cancellationToken);
        return new DomainCommandResult<TResource>
        {
            CommandResult = result
        };
    } 
}