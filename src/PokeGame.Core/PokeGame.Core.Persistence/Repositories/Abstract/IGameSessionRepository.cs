using BT.Common.Persistence.Shared.Models;
using BT.Common.Persistence.Shared.Repositories.Abstract;
using PokeGame.Core.Persistence.Entities;
using PokeGame.Core.Schemas.Game;

namespace PokeGame.Core.Persistence.Repositories.Abstract;

public interface IGameSessionRepository : IRepository<GameSessionEntity, Guid?, GameSession>
{
    Task<DbResult> EndGameSession(GameSession gameSession);
    Task DeleteAllSessionsByGameSaveIdAsync(Guid gameSaveId);
    Task DeleteAllSessionsByConnectionIdAsync(string connectionId);
}
