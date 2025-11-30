using BT.Common.FastArray.Proto;
using BT.Common.Persistence.Shared.Utils;
using BT.Common.Services.Concrete;
using Microsoft.Extensions.Logging;
using PokeGame.Core.Common.Exceptions;
using PokeGame.Core.Domain.Services.Game.Abstract;
using PokeGame.Core.Persistence.Repositories.Abstract;
using PokeGame.Core.Schemas.Game;
using PokeGame.Core.Schemas.PokeApi;

namespace PokeGame.Core.Domain.Services.Game.Concrete;

internal sealed class GameAndPokeApiResourceManagerService : IGameAndPokeApiResourceManagerService
{
    private readonly IOwnedPokemonRepository _ownedPokemonRepository;
    private readonly IPokeApiClient _pokeApiClient;
    private readonly ILogger<GameAndPokeApiResourceManagerService> _logger;

    public GameAndPokeApiResourceManagerService(
        IOwnedPokemonRepository ownedPokemonRepository,
        IPokeApiClient pokeApiClient,
        ILogger<GameAndPokeApiResourceManagerService> logger
    )
    {
        _ownedPokemonRepository = ownedPokemonRepository;
        _pokeApiClient = pokeApiClient;
        _logger = logger;
    }

    public async Task<IReadOnlyCollection<OwnedPokemon>> GetDeepOwnedPokemon(
        IReadOnlyCollection<OwnedPokemon> ownedPokemons,
        CancellationToken cancellationToken = default
    )
    {
        using var activity = TelemetryHelperService.ActivitySource.StartActivity(
            nameof(GetDeepOwnedPokemon)
        );
        activity?.SetTag("ownedPokemons.count", ownedPokemons.Count);

        try
        {
            var getResourcesJobList = ownedPokemons
                .FastArraySelect(x => GetResourcesFromApiAsync(x, cancellationToken))
                .ToArray();

            await Task.WhenAll(getResourcesJobList);

            var finalOwnedPokemon = new List<OwnedPokemon>();

            foreach (var resource in getResourcesJobList)
            {
                finalOwnedPokemon.Add(await resource);
            }

            return finalOwnedPokemon;
        }
        catch (PokeGameApiException)
        {
            throw;
        }
        catch (Exception ex)
        {
            throw new PokeGameApiServerException("Failed to fetch owned pokemon resources", ex);
        }
    }

    public async Task<IReadOnlyCollection<OwnedPokemon>> GetFullOwnedPokemon(
        IReadOnlyCollection<Guid> ownedPokemonId,
        CancellationToken cancellationToken = default
    )
    {
        using var activity = TelemetryHelperService.ActivitySource.StartActivity();
        activity?.SetTag("ownedPokemonId.count", ownedPokemonId.Count);

        try
        {
            var foundPokemonFromDb =
                await EntityFrameworkUtils.TryDbOperation(
                    () =>
                        _ownedPokemonRepository.GetMany(
                            ownedPokemonId.FastArraySelect(x => (Guid?)x).ToArray()
                        ),
                    _logger
                )
                ?? throw new PokeGameApiServerException(
                    "Failed to fetch owned pokemon from database"
                );

            if (!foundPokemonFromDb.IsSuccessful || foundPokemonFromDb.Data.Count < 1)
            {
                throw new PokeGameApiServerException(
                    "Pokemon could not be retrieved from database"
                );
            }

            return await GetDeepOwnedPokemon(foundPokemonFromDb.Data, cancellationToken);
        }
        catch (PokeGameApiException)
        {
            throw;
        }
        catch (Exception ex)
        {
            throw new PokeGameApiServerException("Failed to fetch owned pokemon resources", ex);
        }
    }

    private async Task<OwnedPokemon> GetResourcesFromApiAsync(
        OwnedPokemon ownedPokemon,
        CancellationToken cancellationToken = default
    )
    {
        using var activity = TelemetryHelperService.ActivitySource.StartActivity(
            nameof(GetResourcesFromApiAsync)
        );
        activity?.SetTag("ownedPokemon.id", ownedPokemon.Id.ToString());
        activity?.SetTag("ownedPokemon.pokemonResourceName", ownedPokemon.PokemonResourceName);

        _logger.LogInformation(
            "Fetching OwnedPokemon resources from PokeApi with params: {@Params}",
            new
            {
                ownedPokemon.Id,
                ownedPokemon.PokemonResourceName,
                ownedPokemon.MoveOneResourceName,
                ownedPokemon.MoveTwoResourceName,
                ownedPokemon.MoveThreeResourceName,
                ownedPokemon.MoveFourResourceName,
            }
        );

        var pokemonJob = _pokeApiClient.GetResourceAsync<Pokemon>(
            ownedPokemon.PokemonResourceName,
            cancellationToken
        );
        var speciesJob = _pokeApiClient.GetResourceAsync<PokemonSpecies>(
            ownedPokemon.PokemonResourceName,
            cancellationToken
        );

        var moveJobList = new List<Task<Move>>
        {
            _pokeApiClient.GetResourceAsync<Move>(
                ownedPokemon.MoveOneResourceName,
                cancellationToken
            ),
        };

        if (!string.IsNullOrWhiteSpace(ownedPokemon.MoveTwoResourceName))
        {
            moveJobList.Add(
                _pokeApiClient.GetResourceAsync<Move>(
                    ownedPokemon.MoveTwoResourceName,
                    cancellationToken
                )
            );
        }
        if (!string.IsNullOrWhiteSpace(ownedPokemon.MoveThreeResourceName))
        {
            moveJobList.Add(
                _pokeApiClient.GetResourceAsync<Move>(
                    ownedPokemon.MoveThreeResourceName,
                    cancellationToken
                )
            );
        }
        if (!string.IsNullOrWhiteSpace(ownedPokemon.MoveFourResourceName))
        {
            moveJobList.Add(
                _pokeApiClient.GetResourceAsync<Move>(
                    ownedPokemon.MoveFourResourceName,
                    cancellationToken
                )
            );
        }

        var executionList = new List<Task> { pokemonJob, speciesJob }
            .Concat(moveJobList)
            .ToArray();

        await Task.WhenAll(executionList);

        var finalMoveTwo = moveJobList.ElementAtOrDefault(1);
        var finalMoveThree = moveJobList.ElementAtOrDefault(2);
        var finalMoveFour = moveJobList.ElementAtOrDefault(3);

        ownedPokemon.Pokemon = await pokemonJob;
        ownedPokemon.PokemonSpecies = await speciesJob;
        ownedPokemon.MoveOne = await moveJobList[0];
        ownedPokemon.MoveTwo = finalMoveTwo is null ? null : await finalMoveTwo;
        ownedPokemon.MoveThree = finalMoveThree is null ? null : await finalMoveThree;
        ownedPokemon.MoveFour = finalMoveFour is null ? null : await finalMoveFour;

        return ownedPokemon;
    }
}
