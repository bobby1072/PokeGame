using System.ComponentModel.DataAnnotations.Schema;
using BT.Common.Persistence.Shared.Entities;
using PokeGame.Core.Schemas;
using PokeGame.Core.Schemas.Extensions;

namespace PokeGame.Core.Persistence.Entities;

[Table("pokedex", Schema = "public")]
public sealed class PokedexPokemonEntity : BasePokeGameEntity<int, PokedexPokemon>
{
    public required string EnglishName { get; set; }
    public required string JapaneseName { get; set; }
    public required string ChineseName { get; set; }
    public required string FrenchName { get; set; }

    [Column(name: "type_one")]
    public required string Type1 { get; set; }

    [Column(name: "type_two")]
    public string? Type2 { get; set; }
    public required int Hp { get; set; }
    public required int Attack { get; set; }
    public required int Defence { get; set; }
    public required int SpecialAttack { get; set; }
    public required int SpecialDefence { get; set; }
    public required int Speed { get; set; }

    public override PokedexPokemon ToModel()
    {
        return new PokedexPokemon
        {
            Id = Id,
            EnglishName = EnglishName,
            ChineseName = ChineseName,
            FrenchName = FrenchName,
            Type = new PokedexPokemonType
            {
                Type1 = Enum.Parse<PokemonType>(Type1),
                Type2 = Type2 is null ? null : Enum.Parse<PokemonType>(Type2),
            },
            Stats = new PokedexPokemonStats
            {
                Attack = Attack,
                Defence = Defence,
                Speed = Speed,
                Hp = Hp,
                SpecialAttack = SpecialAttack,
                SpecialDefence = SpecialDefence,
            },
            JapaneseName = JapaneseName,
        };
    }
}
