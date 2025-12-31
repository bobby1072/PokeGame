using PokeGame.Core.Schemas.Game.PokemonRelated;

namespace PokeGame.Core.Schemas.Game;

public sealed class OwnedPokemonInnerDetails: DomainModel<OwnedPokemonInnerDetails>
{
    public int BaseExperienceFromDefeating { get; set; }
    public int Height { get; set; }
    public int Weight { get; set; }
    public required PokemonSpriteDetails Sprites { get; set; }
    // public required List<PokemonStat> Stats { get; set; }
    // public required List<PokemonType> Types { get; set; }

    public override bool Equals(OwnedPokemonInnerDetails? other)
    {
        return BaseExperienceFromDefeating == other?.BaseExperienceFromDefeating &&
               Height == other.Height &&
               Weight == other.Weight &&
               Sprites.Equals(other.Sprites);
    }
}
