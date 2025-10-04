using PokeGame.Core.Schemas.Game;

namespace PokeGame.Core.Domain.Services.Game.Abstract;

public interface IGameSessionProcessingManager
{
    Task<GameSession> StartGameSession(Guid gameSaveId, Schemas.Game.User user);
    Task DeleteAllGameSessionsForGameSave(Guid gameSaveId);
}