using BT.Common.Persistence.Shared.Utils;
using Microsoft.Extensions.Logging;
using PokeGame.Core.Common.Exceptions;
using PokeGame.Core.Domain.Services.Abstract;
using PokeGame.Core.Persistence.Repositories.Abstract;
using PokeGame.Core.Schemas;
using PokeGame.Core.Schemas.Extensions;
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
        if (!input.HasInputProperties())
        {
            var result = await EntityFrameworkUtils
                .TryDbOperation(() => 
                    _pokedexPokemonRepository.GetAll(), _logger)
                ?? throw new PokeGameApiServerException("Failed to fetch pokedex pokemon records");

            if (!result.IsSuccessful)
            {
                throw new PokeGameApiServerException("Failed to fetch pokedex pokemon records");
            }
            
            return result.Data;
        }

        throw new NotImplementedException();
    }
}