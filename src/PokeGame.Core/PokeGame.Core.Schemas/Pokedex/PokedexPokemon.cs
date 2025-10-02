namespace PokeGame.Core.Schemas.Pokedex;

public sealed class PokedexPokemon : PersistableDomainModel<PokedexPokemon, int>
{
    public required string EnglishName { get; set; }
    public required string JapaneseName { get; set; }
    public required string ChineseName { get; set; }
    public required string FrenchName { get; set; }
    public required PokedexPokemonType Type { get; set; }
    public required PokedexPokemonStats Stats { get; set; }

    public override bool Equals(PokedexPokemon? other)
    {
        return other?.Id == Id &&
               other.EnglishName == EnglishName &&
               other.JapaneseName == JapaneseName &&
               other.ChineseName == ChineseName &&
               other.FrenchName == FrenchName &&
               other.Type.Equals(Type) &&
               other.Stats.Equals(Stats);
    }
}