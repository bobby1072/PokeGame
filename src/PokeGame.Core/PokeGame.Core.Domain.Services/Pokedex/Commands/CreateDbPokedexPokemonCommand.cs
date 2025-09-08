using BT.Common.FastArray.Proto;
using BT.Common.Persistence.Shared.Utils;
using Microsoft.Extensions.Logging;
using PokeGame.Core.Common.Exceptions;
using PokeGame.Core.Domain.Services.Abstract;
using PokeGame.Core.Domain.Services.Models;
using PokeGame.Core.Persistence.Repositories.Abstract;
using PokeGame.Core.Schemas;

namespace PokeGame.Core.Domain.Services.Pokedex.Commands;

internal sealed class CreateDbPokedexPokemonCommand: IDomainCommand<IReadOnlyCollection<PokedexPokemon>, DomainCommandResult<IReadOnlyCollection<PokedexPokemon>>>
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

    public async Task<DomainCommandResult<IReadOnlyCollection<PokedexPokemon>>> ExecuteAsync(IReadOnlyCollection<PokedexPokemon> email, CancellationToken cancellationToken = default)
    {
        _logger.LogInformation("Input contains {PokedexPokemonSaveCount} pokedex pokemon records...", email.Count);
        
        var existingPokedex = await EntityFrameworkUtils.TryDbOperation(() => _pokedexPokemonRepository.GetAll(), _logger) ?? throw new PokeGameApiServerException("Failed to get existing pokedex count");

        var pokemonToCreate = email.FastArrayWhere(x => !existingPokedex.Data.Any(y => y.Equals(x))).ToArray();

        if (pokemonToCreate.Length == 0)
        {
            _logger.LogWarning("None of the new entries are unique so no pokedex pokemon records to created.");

            return new DomainCommandResult<IReadOnlyCollection<PokedexPokemon>> {
                CommandResult = []
            };
        }
        
        var saveResult = await
            EntityFrameworkUtils.TryDbOperation(() => _pokedexPokemonRepository.Create(pokemonToCreate), _logger)
                ?? throw new PokeGameApiServerException("Failed to create pokedex pokemon");

        if (saveResult.IsSuccessful != true || saveResult.Data.Count == 0)
        {
            throw new PokeGameApiServerException("Failed to create pokedex pokemon");
        }

        return new DomainCommandResult<IReadOnlyCollection<PokedexPokemon>> {
            CommandResult = saveResult.Data
        };
    }
}