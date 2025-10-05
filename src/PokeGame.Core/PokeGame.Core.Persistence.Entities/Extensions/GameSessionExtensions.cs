using PokeGame.Core.Schemas.Game;

namespace PokeGame.Core.Persistence.Entities.Extensions;

public static class GameSessionExtensions
{
    public static GameSessionEntity ToEntity(this GameSession gameSession)
    {
        return new GameSessionEntity
        {
            Id = gameSession.Id,
            GameSaveId = gameSession.GameSaveId,
            UserId = gameSession.UserId,
            StartedAt = gameSession.StartedAt,
            GameSave = gameSession.GameSave?.ToEntity(),
            User = gameSession.User?.ToEntity(),
        };
    }
}
