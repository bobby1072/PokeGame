namespace PokeGame.Core.Schemas.Game.PokemonRelated;

public sealed class PokemonTypeDetails: DomainModel<PokemonTypeDetails>
{
    public required string Name { get; set; }

    public override bool Equals(PokemonTypeDetails? other)
    {
        return Name == other?.Name;
    }
}