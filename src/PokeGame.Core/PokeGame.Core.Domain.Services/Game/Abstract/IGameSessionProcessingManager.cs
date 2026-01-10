using PokeGame.Core.Schemas.Game;
using PokeGame.Core.Schemas.Game.PokemonRelated;

namespace PokeGame.Core.Domain.Services.Game.Abstract;

public interface IGameSessionProcessingManager
{
    Task<GameSession> StartGameSession(
        Guid gameSaveId,
        string connectionId,
        Schemas.Game.User user
    );

    Task EndGameSession(string connectionId);

    Task<IReadOnlyCollection<OwnedPokemon>> GetShallowOwnedPokemonInDeck(Guid gameSessionId,
        Schemas.Game.User user, CancellationToken cancellationToken = default);

    Task<IReadOnlyCollection<OwnedPokemon>> GetDeepOwnedPokemonInDeck(string connectionId,
        Schemas.Game.User user, CancellationToken cancellationToken = default);
}
