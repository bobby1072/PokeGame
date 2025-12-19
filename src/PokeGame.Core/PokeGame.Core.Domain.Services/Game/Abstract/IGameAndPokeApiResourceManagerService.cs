using PokeGame.Core.Schemas.Game;
using PokeGame.Core.Schemas.PokeApi;

namespace PokeGame.Core.Domain.Services.Game.Abstract;

internal interface IGameAndPokeApiResourceManagerService
{
    Task<(Move MoveOne, Move? MoveTwo, Move? MoveThree, Move? MoveFour)> GetMoveSet(
        string moveOneResourceName,
        string? moveTwoResourceName,
        string? moveThreeResourceName,
        string? moveFourResourceName,
        CancellationToken cancellationToken = default
    );
    Task<IReadOnlyCollection<OwnedPokemon>> GetDeepOwnedPokemon(
        IReadOnlyCollection<OwnedPokemon> ownedPokemons,
        CancellationToken cancellationToken = default
    );
    Task<IReadOnlyCollection<OwnedPokemon>> GetFullOwnedPokemon(
        IReadOnlyCollection<Guid> ownedPokemonId,
        CancellationToken cancellationToken = default
    );
}
