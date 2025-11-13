using System.Text.Json;

namespace PokeGame.Core.Schemas.Game;

public sealed class GameSaveData: PersistableDomainModel<GameSaveData, long?>
{
    public required Guid GameSaveId { get; set; }
    public required GameSaveDataActual GameData { get; set; }

    public override bool Equals(GameSaveData? other)
    {
        return other is not null && 
               Id == other.Id &&
               GameSaveId == other.GameSaveId &&
               GameData.Equals(other.GameData);
    }
}