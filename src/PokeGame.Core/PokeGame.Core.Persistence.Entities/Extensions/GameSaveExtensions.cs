using PokeGame.Core.Schemas.Game;

namespace PokeGame.Core.Persistence.Entities.Extensions;

public static class GameSaveExtensions
{
    public static GameSaveEntity ToEntity(this GameSave gameSave)
    {
        return new GameSaveEntity
        {
            Id = gameSave.Id,
            CharacterName = gameSave.CharacterName,
            UserId = gameSave.UserId,
            LastPlayedScene = gameSave.LastPlayedScene,
            LastPlayedLocationX = gameSave.LastPlayedLocationX,
            LastPlayedLocationY = gameSave.LastPlayedLocationY,
            DateCreated = gameSave.DateCreated,
            LastPlayed = gameSave.LastPlayed,
        };
    }
}
