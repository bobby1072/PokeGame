using System.ComponentModel.DataAnnotations.Schema;
using PokeGame.Core.Schemas;

namespace PokeGame.Core.Persistence.Entities;

[Table("game_save", Schema = "public")]
public sealed class GameSaveEntity: BasePokeGameEntity<Guid?, GameSave>
{
    public required Guid UserId { get; set; }
    public required string CharacterName { get; set; }
    public DateTime DateCreated { get; set; } = DateTime.UtcNow;
    public DateTime LastPlayed { get; set; } = DateTime.UtcNow;

    public override GameSave ToModel()
    {
        return new GameSave
        {
            Id = Id,
            CharacterName = CharacterName,
            UserId = UserId,
            DateCreated = DateCreated,
            LastPlayed = LastPlayed
        };
    }
}