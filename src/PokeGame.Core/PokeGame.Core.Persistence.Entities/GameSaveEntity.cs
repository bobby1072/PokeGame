using System.ComponentModel.DataAnnotations.Schema;
using PokeGame.Core.Schemas.Game;

namespace PokeGame.Core.Persistence.Entities;

[Table("game_save", Schema = "public")]
public sealed class GameSaveEntity: BasePokeGameEntity<Guid?, GameSave>
{
    public required Guid UserId { get; set; }
    public required string CharacterName { get; set; }
    public DateTime DateCreated { get; set; } = DateTime.UtcNow;
    public DateTime LastPlayed { get; set; } = DateTime.UtcNow;
    public GameSaveDataEntity? GameSaveData { get; init; }

    public override GameSave ToModel()
    {
        return new GameSave
        {
            Id = Id,
            CharacterName = CharacterName,
            UserId = UserId,
            LastPlayed = LastPlayed,
            DateCreated = DateCreated,
            GameSaveData = GameSaveData?.ToModel(),
        };
    }
}