using BT.Common.FastArray.Proto;
using BT.Common.Persistence.Shared.Utils;
using BT.Common.Services.Concrete;
using Microsoft.Extensions.Logging;
using PokeGame.Core.Common.Exceptions;
using PokeGame.Core.Domain.Services.Game.Abstract;
using PokeGame.Core.Persistence.Repositories.Abstract;
using PokeGame.Core.Schemas.Game;
using PokeGame.Core.Schemas.Game.PokemonRelated;
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

    public async Task<(Pokemon Pokemon, PokemonSpecies PokemonSpecies)> GetPokemonAndSpecies(int pokemonNumber,
        CancellationToken cancellationToken = default)
    {
        using var activity = TelemetryHelperService.ActivitySource.StartActivity();
        activity?.SetTag(nameof(pokemonNumber), pokemonNumber);

        try
        {
            var pokemonMainJob = _pokeApiClient.GetResourceAsync<Pokemon>(pokemonNumber, cancellationToken);
            var pokemonSpeciesJob = _pokeApiClient.GetResourceAsync<PokemonSpecies>(pokemonNumber, cancellationToken);

            await Task.WhenAll(pokemonSpeciesJob, pokemonMainJob);
            
            return (await pokemonMainJob,await pokemonSpeciesJob);
        }
        catch (PokeGameApiException)
        {
            throw;
        }
        catch (Exception ex)
        {
            throw new PokeGameApiServerException("Failed to fetch pokemon resources", ex);
        }
    }
    public async Task<(Pokemon Pokemon, PokemonSpecies PokemonSpecies)> GetPokemonAndSpecies(string pokemonName,
        CancellationToken cancellationToken = default)
    {
        using var activity = TelemetryHelperService.ActivitySource.StartActivity();
        activity?.SetTag(nameof(pokemonName), pokemonName);

        try
        {
            var pokemonMainJob = _pokeApiClient.GetResourceAsync<Pokemon>(pokemonName, cancellationToken);
            var pokemonSpeciesJob = _pokeApiClient.GetResourceAsync<PokemonSpecies>(pokemonName, cancellationToken);

            await Task.WhenAll(pokemonSpeciesJob, pokemonMainJob);
            
            return (await pokemonMainJob,await pokemonSpeciesJob);
        }
        catch (PokeGameApiException)
        {
            throw;
        }
        catch (Exception ex)
        {
            throw new PokeGameApiServerException("Failed to fetch pokemon resources", ex);
        }
    }
    public async Task<(Move? MoveOne, Move? MoveTwo, Move? MoveThree, Move? MoveFour)> GetMoveSet(
        string? moveOneResourceName,
        string? moveTwoResourceName,
        string? moveThreeResourceName,
        string? moveFourResourceName,
        CancellationToken cancellationToken = default
    )
    {
        using var activity = TelemetryHelperService.ActivitySource.StartActivity();
        activity?.SetTag(nameof(moveOneResourceName), moveOneResourceName);
        activity?.SetTag(nameof(moveTwoResourceName), moveTwoResourceName);
        activity?.SetTag(nameof(moveThreeResourceName), moveThreeResourceName);
        activity?.SetTag(nameof(moveFourResourceName), moveFourResourceName);

        try
        {
            return await GetMoves(
                (
                    moveOneResourceName,
                    moveTwoResourceName,
                    moveThreeResourceName,
                    moveFourResourceName
                ),
                cancellationToken
            );
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

    public async Task<IReadOnlyCollection<OwnedPokemon>> GetDeepOwnedPokemon(
        IReadOnlyCollection<OwnedPokemon> ownedPokemons,
        CancellationToken cancellationToken = default
    )
    {
        using var activity = TelemetryHelperService.ActivitySource.StartActivity();
        activity?.SetTag("ownedPokemons.count", ownedPokemons.Count);

        try
        {
            (
                OwnedPokemon OwnedPokemon,
                Task<(
                    Pokemon Pokemon,
                    PokemonSpecies PokemonSpecies,
                    Move? MoveOne,
                    Move? MoveTwo,
                    Move? MoveThree,
                    Move? MoveFour
                )> ResourceJob
            )[] getResourcesJobList = ownedPokemons
                .FastArraySelect(x =>
                    (
                        x,
                        GetResourcesFromApiAsync(
                            (
                                x.PokemonResourceName,
                                x.MoveOneResourceName,
                                x.MoveTwoResourceName,
                                x.MoveThreeResourceName,
                                x.MoveFourResourceName
                            ),
                            cancellationToken
                        )
                    )
                )
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

    private async Task<(
        Pokemon Pokemon,
        PokemonSpecies PokemonSpecies,
        Move? MoveOne,
        Move? MoveTwo,
        Move? MoveThree,
        Move? MoveFour
    )> GetResourcesFromApiAsync(
        (
            string PokemonName,
            string? MoveOneResourceName,
            string? MoveTwoResourceName,
            string? MoveThreeResourceName,
            string? MoveFourResourceName
        ) pokemonInfo,
        CancellationToken cancellationToken = default
    )
    {
        using var activity = TelemetryHelperService.ActivitySource.StartActivity();
        activity?.SetTag("pokemonResourceName", pokemonInfo.PokemonName);

        _logger.LogInformation(
            "Fetching Pokemon resources from PokeApi with params: {@Params}",
            new
            {
                pokemonInfo.PokemonName,
                pokemonInfo.MoveOneResourceName,
                pokemonInfo.MoveTwoResourceName,
                pokemonInfo.MoveThreeResourceName,
                pokemonInfo.MoveFourResourceName,
            }
        );

        var speciesAndPokemonJob = GetPokemonAndSpecies(pokemonInfo.PokemonName, cancellationToken);
        
        var getMovesJob = GetMoves(
            (
                pokemonInfo.MoveOneResourceName,
                pokemonInfo.MoveTwoResourceName,
                pokemonInfo.MoveThreeResourceName,
                pokemonInfo.MoveFourResourceName
            ),
            cancellationToken
        );
        

        await Task.WhenAll(getMovesJob, speciesAndPokemonJob);

        var pokemonSpecies = (await speciesAndPokemonJob).PokemonSpecies;
        var pokemon = (await speciesAndPokemonJob).Pokemon;
        var moveSet = await getMovesJob;

        return (
            pokemon,
            pokemonSpecies,
            moveSet.MoveOne,
            moveSet.MoveTwo,
            moveSet.MoveThree,
            moveSet.MoveFour
        );
    }

    private async Task<(Move? MoveOne, Move? MoveTwo, Move? MoveThree, Move? MoveFour)> GetMoves(
        (
            string? MoveOneResourceName,
            string? MoveTwoResourceName,
            string? MoveThreeResourceName,
            string? MoveFourResourceName
        ) moveInfo,
        CancellationToken cancellationToken = default
    )
    {
        var moveJobList = new List<Task<Move>>();

        if (!string.IsNullOrWhiteSpace(moveInfo.MoveOneResourceName))
        {
            moveJobList.Add(
                _pokeApiClient.GetResourceAsync<Move>(
                    moveInfo.MoveOneResourceName,
                    cancellationToken
                )
            );
        }

        if (!string.IsNullOrWhiteSpace(moveInfo.MoveTwoResourceName))
        {
            moveJobList.Add(
                _pokeApiClient.GetResourceAsync<Move>(
                    moveInfo.MoveTwoResourceName,
                    cancellationToken
                )
            );
        }
        if (!string.IsNullOrWhiteSpace(moveInfo.MoveThreeResourceName))
        {
            moveJobList.Add(
                _pokeApiClient.GetResourceAsync<Move>(
                    moveInfo.MoveThreeResourceName,
                    cancellationToken
                )
            );
        }
        if (!string.IsNullOrWhiteSpace(moveInfo.MoveFourResourceName))
        {
            moveJobList.Add(
                _pokeApiClient.GetResourceAsync<Move>(
                    moveInfo.MoveFourResourceName,
                    cancellationToken
                )
            );
        }

        await Task.WhenAll(moveJobList);

        var finalMoveOne = moveJobList.ElementAtOrDefault(0);
        var finalMoveTwo = moveJobList.ElementAtOrDefault(1);
        var finalMoveThree = moveJobList.ElementAtOrDefault(2);
        var finalMoveFour = moveJobList.ElementAtOrDefault(3);

        var moveOne = finalMoveOne is null ? null :await finalMoveOne;
        var moveTwo = finalMoveTwo is null ? null : await finalMoveTwo;
        var moveThree = finalMoveThree is null ? null : await finalMoveThree;
        var moveFour = finalMoveFour is null ? null : await finalMoveFour;

        return (moveOne, moveTwo, moveThree, moveFour);
    }
}
