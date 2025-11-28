using PokeGame.Core.Schemas.Game;

namespace PokeGame.Core.Domain.Services.Game.Abstract;

public interface IGameSessionProcessingManager
{
    Task<GameSession> StartGameSession(
        Guid gameSaveId,
        string connectionId,
        Schemas.Game.User user
    );

    Task EndGameSession(string connectionId);

    Task<IReadOnlyCollection<OwnedPokemon>> GetShallowOwnedPokemonInDeck(Guid GameSessionId,
        Schemas.Game.User user);
}
