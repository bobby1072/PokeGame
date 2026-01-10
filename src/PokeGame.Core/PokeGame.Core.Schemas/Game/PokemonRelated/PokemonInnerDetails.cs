namespace PokeGame.Core.Schemas.Game.PokemonRelated;

public sealed class PokemonInnerDetails: DomainModel<PokemonInnerDetails>
{
    public required int BaseExperienceFromDefeating { get; set; }
    public required int Height { get; set; }
    public required int Weight { get; set; }
    public required PokemonSpriteDetails Sprites { get; set; }
    public required IReadOnlyCollection<PokemonStatDetails> Stats { get; set; }
    public required IReadOnlyCollection<PokemonTypeDetails> Types { get; set; }
    public required bool IsLegendary { get; set; }
    
    public override bool Equals(PokemonInnerDetails? other)
    {
        return BaseExperienceFromDefeating == other?.BaseExperienceFromDefeating &&
               Height == other.Height &&
               Weight == other.Weight &&
               Sprites.Equals(other.Sprites) &&
               Stats.SequenceEqual(other.Stats) &&
               Types.SequenceEqual(other.Types) &&
               IsLegendary == other.IsLegendary;;
    }
}
