using PokeGame.Core.Schemas.Game;

namespace PokeGame.Core.Persistence.Entities.Extensions;

public static class GameSaveDataExtensions
{
    public static GameSaveDataEntity ToGameSaveDataEntity(this GameSaveData gameSaveData)
    {
        return new GameSaveDataEntity
        {
            Id = gameSaveData.Id,
            GameData = gameSaveData.GameData,
            GameSaveId = gameSaveData.GameSaveId,
        };
    }
}