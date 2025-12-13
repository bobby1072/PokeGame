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
        using var activity = TelemetryHelperService.ActivitySource.StartActivity();
        activity?.SetTag("ownedPokemons.count", ownedPokemons.Count);

        try
        {
            (OwnedPokemon OwnedPokemon, Task<(Pokemon Pokemon, PokemonSpecies PokemonSpecies, Move MoveOne, Move? MoveTwo, Move? MoveThree,
                Move? MoveFour)> ResourceJob)[] getResourcesJobList = ownedPokemons
                .FastArraySelect(x => (x, 
                    GetResourcesFromApiAsync((x.PokemonResourceName, x.MoveOneResourceName, x.MoveTwoResourceName, x.MoveThreeResourceName, x.MoveFourResourceName),
                        cancellationToken)))
                .ToArray();

            await Task.WhenAll(getResourcesJobList.FastArraySelect(x => x.ResourceJob));

            var finalOwnedPokemon = new List<OwnedPokemon>();

            foreach (var resource in getResourcesJobList)
            {
                var results = await resource.ResourceJob;
                
                resource.OwnedPokemon.Pokemon = results.Pokemon;
                resource.OwnedPokemon.PokemonSpecies = results.PokemonSpecies;
                resource.OwnedPokemon.MoveOne = results.MoveOne;
                resource.OwnedPokemon.MoveTwo = results.MoveTwo;
                resource.OwnedPokemon.MoveThree = results.MoveThree;
                resource.OwnedPokemon.MoveFour = results.MoveFour;
                
                finalOwnedPokemon.Add(resource.OwnedPokemon);
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

    private async Task<(Pokemon Pokemon, PokemonSpecies PokemonSpecies, Move MoveOne, Move? MoveTwo, Move? MoveThree, Move? MoveFour)> GetResourcesFromApiAsync(
        (string PokemonResourceName,
            string MoveOneResourceName,
            string? MoveTwoResourceName,
            string? MoveThreeResourceName,
            string? MoveFourResourceName) ownedPokemon,
        CancellationToken cancellationToken = default
    )
    {
        using var activity = TelemetryHelperService.ActivitySource.StartActivity();
        activity?.SetTag("ownedPokemon.pokemonResourceName", ownedPokemon.PokemonResourceName);

        _logger.LogInformation(
            "Fetching OwnedPokemon resources from PokeApi with params: {@Params}",
            new
            {
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

        var pokemon = await pokemonJob;
        var pokemonSpecies = await speciesJob;
        var moveOne = await moveJobList[0];
        var moveTwo = finalMoveTwo is null ? null : await finalMoveTwo;
        var moveThree = finalMoveThree is null ? null : await finalMoveThree;
        var moveFour = finalMoveFour is null ? null : await finalMoveFour;

        return (pokemon, pokemonSpecies, moveOne, moveTwo, moveThree, moveFour);
    }
}
