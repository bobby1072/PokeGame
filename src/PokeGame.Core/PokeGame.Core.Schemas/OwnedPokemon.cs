namespace PokeGame.Core.Schemas;

public sealed class OwnedPokemon : PersistableDomainModel<OwnedPokemon, Guid?>
{
    public required Guid GameSaveId { get; set; }
    public required string ResourceName { get; set; }
    public DateTime CaughtAt { get; set; } = DateTime.UtcNow;
    public bool InDeck { get; set; } = false;
    public required int PokemonLevel { get; set; }
    public int CurrentExperience { get; set; } = 0;
    public required int CurrentHp { get; set; }
    public required string MoveOneResourceName { get; set; }
    public string? MoveTwoResourceName { get; set; }
    public string? MoveThreeResourceName { get; set; }
    public string? MoveFourResourceName { get; set; }

    public override bool Equals(OwnedPokemon? other)
    {
        return other is not null &&
               GameSaveId == other.GameSaveId &&
               ResourceName == other.ResourceName &&
               CaughtAt == other.CaughtAt &&
               InDeck == other.InDeck &&
               PokemonLevel == other.PokemonLevel &&
               CurrentExperience == other.CurrentExperience &&
               CurrentHp == other.CurrentHp &&
               MoveOneResourceName == other.MoveOneResourceName &&
               MoveTwoResourceName == other.MoveTwoResourceName &&
               MoveThreeResourceName == other.MoveThreeResourceName &&
               MoveFourResourceName == other.MoveFourResourceName;
    }
}