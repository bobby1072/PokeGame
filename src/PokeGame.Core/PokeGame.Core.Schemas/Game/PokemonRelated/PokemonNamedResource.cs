namespace PokeGame.Core.Schemas.Game.PokemonRelated;

public sealed class PokemonNamedResource: DomainModel<PokemonNamedResource>
{
    public required string Name { get; init; }

    public override bool Equals(PokemonNamedResource? other)
    {
        return Name == other?.Name;
    }
}