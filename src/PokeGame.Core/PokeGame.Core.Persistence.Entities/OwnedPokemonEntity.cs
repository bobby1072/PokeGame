using System.ComponentModel.DataAnnotations.Schema;
using PokeGame.Core.Schemas.Game;

namespace PokeGame.Core.Persistence.Entities;

[Table("owned_pokemon", Schema = "public")]
public sealed class OwnedPokemonEntity : BasePokeGameEntity<Guid?, OwnedPokemon>
{
    public required Guid GameSaveId { get; set; }
    public required string ResourceName { get; set; }
    public DateTime CaughtAt { get; set; } = DateTime.UtcNow;
    public required int PokemonLevel { get; set; }
    public required int CurrentExperience { get; set; }
    public required int CurrentHp { get; set; }
    public required string MoveOneResourceName { get; set; }
    public string? MoveTwoResourceName { get; set; }
    public string? MoveThreeResourceName { get; set; }
    public string? MoveFourResourceName { get; set; }

    public override OwnedPokemon ToModel()
    {
        return new OwnedPokemon
        {
            Id = Id,
            GameSaveId = GameSaveId,
            ResourceName = ResourceName,
            CaughtAt = CaughtAt,
            PokemonLevel = PokemonLevel,
            CurrentExperience = CurrentExperience,
            CurrentHp = CurrentHp,
            MoveOneResourceName = MoveOneResourceName,
            MoveTwoResourceName = MoveTwoResourceName,
            MoveThreeResourceName = MoveThreeResourceName,
            MoveFourResourceName = MoveFourResourceName,
        };
    }
}
