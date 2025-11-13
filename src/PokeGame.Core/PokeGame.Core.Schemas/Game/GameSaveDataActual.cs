namespace PokeGame.Core.Schemas.Game;

public sealed class GameSaveDataActual: DomainModel<GameSaveDataActual>
{
    public required string LastPlayedScene  { get; set; }
    public required int LastPlayedLocationX { get; set; }
    public required int LastPlayedLocationY { get; set; }

    public override bool Equals(GameSaveDataActual? other)
    {
        return other is GameSaveDataActual gameSaveDataActual &&
               LastPlayedScene == gameSaveDataActual.LastPlayedScene &&
               LastPlayedLocationX == gameSaveDataActual.LastPlayedLocationX &&
               LastPlayedLocationY == gameSaveDataActual.LastPlayedLocationY;
    }
}