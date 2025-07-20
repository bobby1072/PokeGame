﻿namespace PokeGame.Core.Schemas;

public sealed class PokedexPokemon: PersistableDomainModel<PokedexPokemon, int>
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



    public PokedexPokemonRawJson ToJsonType()
    {
        var typeArray = new List<string>
        {
            Type.Type1.ToString()
        };

        if (Type.Type2 is not null)
        {
            typeArray.Add(Type.Type2.ToString()!);
        }
        return new PokedexPokemonRawJson
        {
            Id = Id,
            Base = Stats.ToJson(),
            Name = new PokedexPokemonNameRawJson
            {
                English = EnglishName,
                Chinese = ChineseName,
                French = FrenchName,
                Japanese = JapaneseName
            },
            Type = typeArray,
        };
    }
}