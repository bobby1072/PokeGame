using PokeGame.Core.Schemas.Common;

namespace PokeGame.Core.Schemas.Pokedex;

public sealed class PokedexPokemonType : DomainModel<PokedexPokemonType>
{
    public required PokemonTypeEnum Type1 { get; set; }
    public PokemonTypeEnum? Type2 { get; set; }

    public override bool Equals(PokedexPokemonType? other)
    {
        return other?.Type1 == Type1 && other.Type2 == Type2;
    }
}