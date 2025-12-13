using PokeGame.Core.Schemas.PokeApi;

namespace PokeGame.Core.Schemas.Game;

public sealed class OwnedPokemon : PersistableDomainModel<OwnedPokemon, Guid?>
{
    public required Guid GameSaveId { get; set; }
    public required string PokemonResourceName { get; set; }
    public DateTime CaughtAt { get; set; } = DateTime.UtcNow;
    public required int PokemonLevel { get; set; }
    public int CurrentExperience { get; set; } = 0;
    public required int CurrentHp { get; set; }
    public required string MoveOneResourceName { get; set; }
    public Move? MoveOne { get; set; }
    public string? MoveTwoResourceName { get; set; }
    public Move? MoveTwo { get; set; }
    public string? MoveThreeResourceName { get; set; }
    public Move? MoveThree { get; set; }
    public string? MoveFourResourceName { get; set; }
    public Move? MoveFour { get; set; }
    public PokemonSpecies? PokemonSpecies { get; set; }
    public Pokemon? Pokemon { get; set; }
    public GameSave? GameSave { get; set; }

    public override bool Equals(OwnedPokemon? other)
    {
        return other is not null
            && Id == other.Id
            && GameSaveId == other.GameSaveId
            && PokemonResourceName == other.PokemonResourceName
            && CaughtAt == other.CaughtAt
            && PokemonLevel == other.PokemonLevel
            && CurrentExperience == other.CurrentExperience
            && CurrentHp == other.CurrentHp
            && MoveOneResourceName == other.MoveOneResourceName
            && MoveTwoResourceName == other.MoveTwoResourceName
            && MoveThreeResourceName == other.MoveThreeResourceName
            && MoveFourResourceName == other.MoveFourResourceName;
    }
}
