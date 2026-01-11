using PokeGame.Core.Schemas.Common;

namespace PokeGame.Core.Schemas.Game.PokemonRelated;

public sealed class PokemonInnerDetails: DomainModel<PokemonInnerDetails>
{
    public required int BaseExperienceFromDefeating { get; init; }
    public required int Height { get; init; }
    public required int Weight { get; init; }
    public required PokemonSpriteDetails Sprites { get; init; }
    public required IReadOnlyCollection<PokemonStatDetails> Stats { get; init; }
    public required IReadOnlyCollection<PokemonTypeEnum> Types { get; init; }
    public required bool IsLegendary { get; init; }
    
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
