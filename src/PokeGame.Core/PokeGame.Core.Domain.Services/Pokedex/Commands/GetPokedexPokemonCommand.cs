using Microsoft.Extensions.Logging;
using PokeGame.Core.Domain.Services.Abstract;
using PokeGame.Core.Persistence.Repositories.Abstract;
using PokeGame.Core.Schemas;
using PokeGame.Core.Schemas.Input;

namespace PokeGame.Core.Domain.Services.Pokedex.Commands;

internal sealed class GetPokedexPokemonCommand: IDomainCommand<GetPokedexPokemonInput, IReadOnlyCollection<PokedexPokemon>>
{
    public string CommandName => nameof(GetPokedexPokemonCommand);
    private readonly IPokedexPokemonRepository _pokedexPokemonRepository;
    private readonly ILogger<GetPokedexPokemonCommand> _logger;

    public GetPokedexPokemonCommand(
        IPokedexPokemonRepository pokedexPokemonRepository,
        ILogger<GetPokedexPokemonCommand> logger
    )
    {
        _pokedexPokemonRepository = pokedexPokemonRepository;
        _logger = logger;
    }

    public async Task<IReadOnlyCollection<PokedexPokemon>> ExecuteAsync(GetPokedexPokemonInput input)
    {
        throw new NotImplementedException();
    }
}