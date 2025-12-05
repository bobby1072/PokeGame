namespace PokeGame.Core.Schemas.Game;

public sealed class GameSaveDataActual: DomainModel<GameSaveDataActual>
{
    public required string LastPlayedScene  { get; set; }
    public required int LastPlayedLocationX { get; set; }
    public required int LastPlayedLocationY { get; set; }
    public List<GameSaveDataActualDeckPokemon> DeckPokemon { get; set; } = [];
    public List<GameDataActualUnlockedGameResource> UnlockedGameResource { get; set; } = [];
    
    public override bool Equals(GameSaveDataActual? other)
    {
        return LastPlayedScene == other?.LastPlayedScene &&
               LastPlayedLocationX == other.LastPlayedLocationX &&
               LastPlayedLocationY == other.LastPlayedLocationY &&
               DeckPokemon.SequenceEqual(other.DeckPokemon) &&
               UnlockedGameResource.SequenceEqual(other.UnlockedGameResource);;
    }
}