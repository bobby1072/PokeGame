using PokeGame.Core.Schemas.PokeApi;

namespace PokeGame.Core.Schemas.Game;

public sealed class WildPokemon: DomainModel<WildPokemon>
{
    public required string PokemonResourceName { get; set; }
    public required int PokemonLevel { get; set; }
    public required int CurrentHp { get; set; }
    public required string MoveOneResourceName { get; set; }
    public Move? MoveOne { get; set; }
    public string? MoveTwoResourceName { get; set; }
    public Move? MoveTwo { get; set; }
    public string? MoveThreeResourceName { get; set; }
    public Move? MoveThree { get; set; }
    public string? MoveFourResourceName { get; set; }
    public Move? MoveFour { get; set; }
    public PokemonSpecies? PokemonSpecies { get; set; }
    public Pokemon? Pokemon { get; set; }

    public override bool Equals(WildPokemon? other)
    {
        return other is not null
               && PokemonResourceName == other.PokemonResourceName
               && PokemonLevel == other.PokemonLevel
               && CurrentHp == other.CurrentHp
               && MoveOneResourceName == other.MoveOneResourceName
               && MoveTwoResourceName == other.MoveTwoResourceName
               && MoveThreeResourceName == other.MoveThreeResourceName
               && MoveFourResourceName == other.MoveFourResourceName;
    }
}