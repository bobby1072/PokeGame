using System.ComponentModel.DataAnnotations.Schema;
using PokeGame.Core.Schemas.Game;

namespace PokeGame.Core.Persistence.Entities;

[Table("game_save_data", Schema = "public")]
public sealed class GameSaveDataEntity: BasePokeGameEntity<long?, GameSaveData>
{
    public required Guid GameSaveId { get; set; }
    public required GameSaveDataActual GameData { get; set; }
    public override GameSaveData ToModel()
    {
        return new GameSaveData
        {
            Id = Id,
            GameSaveId = GameSaveId,
            GameData = GameData,
        };
    }
}