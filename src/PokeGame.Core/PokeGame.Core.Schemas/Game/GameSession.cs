namespace PokeGame.Core.Schemas.Game;

public sealed class GameSession : PersistableDomainModel<GameSession, Guid?>
{
    public required Guid GameSaveId { get; set; }
    public required Guid UserId { get; set; }

    // Navigation properties
    public GameSave? GameSave { get; set; }
    public User? User { get; set; }

    public override bool Equals(GameSession? other)
    {
        return Id?.Equals(other?.Id) is true
            && GameSaveId.Equals(other?.GameSaveId)
            && UserId.Equals(other?.UserId);
    }
}
