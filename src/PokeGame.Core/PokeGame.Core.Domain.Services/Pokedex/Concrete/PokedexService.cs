using System.Net;
using BT.Common.FastArray.Proto;
using BT.Common.Persistence.Shared.Utils;
using Microsoft.Extensions.Logging;
using PokeGame.Core.Common.Exceptions;
using PokeGame.Core.Domain.Services.Pokedex.Abstract;
using PokeGame.Core.Persistence.Repositories.Abstract;
using PokeGame.Core.Schemas.Extensions;
using PokeGame.Core.Schemas.Input;
using PokeGame.Core.Schemas.Pokedex;

namespace PokeGame.Core.Domain.Services.Pokedex.Concrete;

internal sealed class PokedexService : IPokedexService
{
    private readonly IPokedexPokemonRepository _pokedexPokemonRepository;
    private readonly ILogger<PokedexService> _logger;

    public PokedexService(
        IPokedexPokemonRepository pokedexPokemonRepository,
        ILogger<PokedexService> logger
    )
    {
        _pokedexPokemonRepository = pokedexPokemonRepository;
        _logger = logger;
    }

    public async Task<IReadOnlyCollection<PokedexPokemon>> CreatePokedexPokemonAsync(
        IReadOnlyCollection<PokedexPokemon> pokemonToCreate,
        CancellationToken cancellationToken = default
    )
    {
        _logger.LogInformation(
            "Input contains {PokedexPokemonSaveCount} pokedex pokemon records...",
            pokemonToCreate.Count
        );

        var existingPokedex =
            await EntityFrameworkUtils.TryDbOperation(
                () => _pokedexPokemonRepository.GetAll(),
                _logger
            ) ?? throw new PokeGameApiServerException("Failed to get existing pokedex count");

        var pokemonToCreateFiltered = pokemonToCreate
            .FastArrayWhere(x => !existingPokedex.Data.Any(y => y.Equals(x)))
            .ToArray();

        if (pokemonToCreateFiltered.Length == 0)
        {
            _logger.LogWarning(
                "None of the new entries are unique so no pokedex pokemon records to created."
            );
            return Array.Empty<PokedexPokemon>();
        }

        var saveResult =
            await EntityFrameworkUtils.TryDbOperation(
                () => _pokedexPokemonRepository.Create(pokemonToCreateFiltered),
                _logger
            ) ?? throw new PokeGameApiServerException("Failed to create pokedex pokemon");

        if (saveResult.IsSuccessful != true || saveResult.Data.Count == 0)
        {
            throw new PokeGameApiServerException("Failed to create pokedex pokemon");
        }

        return saveResult.Data;
    }

    public async Task<IReadOnlyCollection<PokedexPokemon>> GetPokedexPokemonAsync(
        GetPokedexPokemonInput input,
        CancellationToken cancellationToken = default
    )
    {
        _logger.LogInformation(
            "About to get pokedex pokemon with input of: {@PokedexQueryInput}",
            input
        );

        if (!input.HasInputProperties())
        {
            var result =
                await EntityFrameworkUtils.TryDbOperation(
                    () => _pokedexPokemonRepository.GetAll(),
                    _logger
                )
                ?? throw new PokeGameApiServerException("Failed to fetch pokedex pokemon records");

            if (!result.IsSuccessful)
            {
                throw new PokeGameApiServerException("Failed to fetch pokedex pokemon records");
            }

            return result.Data;
        }

        IReadOnlyCollection<PokedexPokemon> pokedexPokemon;

        if (input.FetchMultiple)
        {
            var dbRes =
                await EntityFrameworkUtils.TryDbOperation(() =>
                    _pokedexPokemonRepository.GetMany(input.ToLangNameDictionary())
                )
                ?? throw new PokeGameApiServerException("Failed to fetch pokedex pokemon records");

            if (!dbRes.IsSuccessful || dbRes.Data.Count == 0)
            {
                throw new PokeGameApiUserException(
                    HttpStatusCode.NotFound,
                    "Failed to fetch pokedex pokemon records"
                );
            }

            pokedexPokemon = dbRes.Data;
        }
        else
        {
            var dbRes =
                await EntityFrameworkUtils.TryDbOperation(() =>
                    _pokedexPokemonRepository.GetOne(input.ToLangNameDictionary())
                )
                ?? throw new PokeGameApiServerException("Failed to fetch pokedex pokemon records");

            if (!dbRes.IsSuccessful || dbRes.Data is null)
            {
                throw new PokeGameApiUserException(
                    HttpStatusCode.NotFound,
                    "Failed to fetch pokedex pokemon records"
                );
            }

            pokedexPokemon = [dbRes.Data];
        }

        return pokedexPokemon;
    }
}
