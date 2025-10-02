using System.Net;
using BT.Common.Persistence.Shared.Utils;
using Microsoft.Extensions.Logging;
using PokeGame.Core.Common.Exceptions;
using PokeGame.Core.Domain.Services.Abstract;
using PokeGame.Core.Domain.Services.Models;
using PokeGame.Core.Persistence.Repositories.Abstract;
using PokeGame.Core.Schemas;
using PokeGame.Core.Schemas.Extensions;
using PokeGame.Core.Schemas.Input;

namespace PokeGame.Core.Domain.Services.Pokedex.Commands;

internal sealed class GetDbPokedexPokemonCommand : IDomainCommand<GetPokedexPokemonInput, DomainCommandResult<IReadOnlyCollection<PokedexPokemon>>>
{
    public string CommandName => nameof(GetDbPokedexPokemonCommand);
    private readonly IPokedexPokemonRepository _pokedexPokemonRepository;
    private readonly ILogger<GetDbPokedexPokemonCommand> _logger;

    public GetDbPokedexPokemonCommand(
        IPokedexPokemonRepository pokedexPokemonRepository,
        ILogger<GetDbPokedexPokemonCommand> logger
    )
    {
        _pokedexPokemonRepository = pokedexPokemonRepository;
        _logger = logger;
    }

    public async Task<DomainCommandResult<IReadOnlyCollection<PokedexPokemon>>> ExecuteAsync(GetPokedexPokemonInput email, CancellationToken cancellationToken = default)
    {
        _logger.LogInformation("About to get pokedex pokemon with input of: {@PokedexQueryInput}",
            email);

        if (!email.HasInputProperties())
        {
            var result = await EntityFrameworkUtils
                .TryDbOperation(() =>
                    _pokedexPokemonRepository.GetAll(), _logger)
                ?? throw new PokeGameApiServerException("Failed to fetch pokedex pokemon records");

            if (!result.IsSuccessful)
            {
                throw new PokeGameApiServerException("Failed to fetch pokedex pokemon records");
            }

            return new DomainCommandResult<IReadOnlyCollection<PokedexPokemon>>
            {
                CommandResult = result.Data
            };
        }

        IReadOnlyCollection<PokedexPokemon> pokedexPokemon;

        if (email.FetchMultiple)
        {
            var dbRes = await EntityFrameworkUtils.TryDbOperation(() =>
                _pokedexPokemonRepository.GetMany(email.ToLangNameDictionary()))
                    ?? throw new PokeGameApiServerException("Failed to fetch pokedex pokemon records");

            if (!dbRes.IsSuccessful || dbRes.Data.Count == 0)
            {
                throw new PokeGameApiUserException(HttpStatusCode.NotFound, "Failed to fetch pokedex pokemon records");
            }

            pokedexPokemon = dbRes.Data;
        }
        else
        {
            var dbRes = await EntityFrameworkUtils
                .TryDbOperation(() =>
                    _pokedexPokemonRepository.GetOne(email.ToLangNameDictionary()))
                        ?? throw new PokeGameApiServerException("Failed to fetch pokedex pokemon records");

            if (!dbRes.IsSuccessful || dbRes.Data is null)
            {
                throw new PokeGameApiUserException(HttpStatusCode.NotFound, "Failed to fetch pokedex pokemon records");
            }

            pokedexPokemon = [dbRes.Data];
        }

        return new DomainCommandResult<IReadOnlyCollection<PokedexPokemon>>
        {
            CommandResult = pokedexPokemon
        };
    }
}