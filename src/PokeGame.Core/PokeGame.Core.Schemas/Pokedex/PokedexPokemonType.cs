namespace PokeGame.Core.Schemas.Pokedex;

public sealed class PokedexPokemonType : DomainModel<PokedexPokemonType>
{
    public required PokemonType Type1 { get; set; }
    public PokemonType? Type2 { get; set; }

    public override bool Equals(PokedexPokemonType? other)
    {
        return other?.Type1 == Type1 && other.Type2 == Type2;
    }
}