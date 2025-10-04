using System.ComponentModel.DataAnnotations.Schema;
using PokeGame.Core.Schemas.Game;

namespace PokeGame.Core.Persistence.Entities;

[Table("game_session", Schema = "public")]
public sealed class GameSessionEntity : BasePokeGameEntity<Guid?, GameSession>
{
    public required Guid GameSaveId { get; set; }
    public required Guid UserId { get; set; }
    public required DateTime StartedAt { get; set; }

    // Navigation properties
    public GameSaveEntity? GameSave { get; set; }
    public UserEntity? User { get; set; }

    public override GameSession ToModel()
    {
        return new GameSession
        {
            Id = Id,
            GameSaveId = GameSaveId,
            UserId = UserId,
            StartedAt = StartedAt,
            GameSave = GameSave?.ToModel(),
            User = User?.ToModel(),
        };
    }
}
