using System.ComponentModel.DataAnnotations.Schema;
using PokeGame.Core.Schemas;
using PokeGame.Core.Schemas.Game;

namespace PokeGame.Core.Persistence.Entities;

[Table("game_save", Schema = "public")]
public sealed class GameSaveEntity: BasePokeGameEntity<Guid?, GameSave>
{
    public required Guid UserId { get; set; }
    public required string CharacterName { get; set; }
    public required string LastPlayedScene  { get; set; }
    public required int LastPlayedLocationX { get; set; }
    public required int LastPlayedLocationY { get; set; }
    public DateTime DateCreated { get; set; } = DateTime.UtcNow;
    public DateTime LastPlayed { get; set; } = DateTime.UtcNow;

    public override GameSave ToModel()
    {
        return new GameSave
        {
            Id = Id,
            CharacterName = CharacterName,
            UserId = UserId,
            LastPlayed = LastPlayed,
            LastPlayedScene = LastPlayedScene,
            LastPlayedLocationX = LastPlayedLocationX,
            LastPlayedLocationY = LastPlayedLocationY,
            DateCreated = DateCreated,
        };
    }
}