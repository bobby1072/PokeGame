namespace PokeGame.Core.Schemas.Game;

public sealed class GameSave : PersistableDomainModel<GameSave, Guid?>
{
    public required Guid UserId { get; set; }
    public required string CharacterName { get; set; }
    public DateTime DateCreated { get; set; } = DateTime.UtcNow;
    public DateTime LastPlayed { get; set; } = DateTime.UtcNow;

    public override bool Equals(GameSave? other)
    {
        return UserId == other?.UserId &&
               CharacterName == other.CharacterName &&
               DateCreated == other.DateCreated &&
               LastPlayed == other.LastPlayed;
    }
}