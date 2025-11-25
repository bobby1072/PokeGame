using BT.Common.FastArray.Proto;
using BT.Common.Persistence.Shared.Utils;
using Microsoft.Extensions.Logging;
using PokeGame.Core.Common.Exceptions;
using PokeGame.Core.Domain.Services.Game.Abstract;
using PokeGame.Core.Persistence.Repositories.Abstract;
using PokeGame.Core.Schemas.Game;

namespace PokeGame.Core.Domain.Services.Game.Concrete;

internal sealed class GameAndPokeApiResourceManagerService: IGameAndPokeApiResourceManagerService
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

    public async Task<IReadOnlyCollection<OwnedPokemon>> GetFullOwnedPokemon(IReadOnlyCollection<Guid> ownedPokemonId)
    {
        var foundPokemonFromDb = await EntityFrameworkUtils
            .TryDbOperation(() => _ownedPokemonRepository
                .GetMany(ownedPokemonId.FastArraySelect(x => (Guid?)x).ToArray()),
                _logger)
                    ?? throw new PokeGameApiServerException("Failed to fetch owned pokemon from database");

        if (!foundPokemonFromDb.IsSuccessful || foundPokemonFromDb.Data.Count < 1)
        {
            throw new PokeGameApiServerException("Pokemon could not be retrieved from database");
        }


        throw new NotImplementedException();
    }
}