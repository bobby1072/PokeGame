namespace PokeGame.Core.Schemas.Game.PokemonRelated;

public sealed class PokemonStatDetails: DomainModel<PokemonStatDetails>
{
    public required string Name { get; set; }
    public required int BaseStat { get; set; }

    public override bool Equals(PokemonStatDetails? other)
    {
        return Name == other?.Name &&
               BaseStat == other.BaseStat;;
    }
}