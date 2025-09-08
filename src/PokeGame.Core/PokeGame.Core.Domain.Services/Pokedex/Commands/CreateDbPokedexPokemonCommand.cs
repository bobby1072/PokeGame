using BT.Common.FastArray.Proto;
using BT.Common.Persistence.Shared.Utils;
using Microsoft.Extensions.Logging;
using PokeGame.Core.Common.Exceptions;
using PokeGame.Core.Domain.Services.Abstract;
using PokeGame.Core.Persistence.Repositories.Abstract;
using PokeGame.Core.Schemas;

namespace PokeGame.Core.Domain.Services.Pokedex.Commands;

internal sealed class CreateDbPokedexPokemonCommand: IDomainCommand<IReadOnlyCollection<PokedexPokemon>, IReadOnlyCollection<PokedexPokemon>>
{
    public string CommandName => nameof(CreateDbPokedexPokemonCommand);
    private readonly IPokedexPokemonRepository _pokedexPokemonRepository;
    private readonly ILogger<CreateDbPokedexPokemonCommand> _logger;

    public CreateDbPokedexPokemonCommand(
        IPokedexPokemonRepository pokedexPokemonRepository,
        ILogger<CreateDbPokedexPokemonCommand> logger
    )
    {
        _pokedexPokemonRepository = pokedexPokemonRepository;
        _logger = logger;
    }

    public async Task<IReadOnlyCollection<PokedexPokemon>> ExecuteAsync(IReadOnlyCollection<PokedexPokemon> id, CancellationToken cancellationToken = default)
    {
        _logger.LogInformation("Input contains {PokedexPokemonSaveCount} pokedex pokemon records...", id.Count);
        
        var existingPokedex = await EntityFrameworkUtils.TryDbOperation(() => _pokedexPokemonRepository.GetAll(), _logger) ?? throw new PokeGameApiServerException("Failed to get existing pokedex count");

        var pokemonToCreate = id.FastArrayWhere(x => !existingPokedex.Data.Any(y => y.Equals(x))).ToArray();

        if (pokemonToCreate.Length == 0)
        {
            _logger.LogWarning("None of the new entries are unique so no pokedex pokemon records to created.");

            return [];
        }
        
        var saveResult = await
            EntityFrameworkUtils.TryDbOperation(() => _pokedexPokemonRepository.Create(pokemonToCreate), _logger)
                ?? throw new PokeGameApiServerException("Failed to create pokedex pokemon");

        if (saveResult.IsSuccessful != true || saveResult.Data.Count == 0)
        {
            throw new PokeGameApiServerException("Failed to create pokedex pokemon");
        }

        return saveResult.Data;
    }
}