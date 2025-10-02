namespace PokeGame.Core.Schemas;

public sealed class PokedexPokemonStats : DomainModel<PokedexPokemonStats>
{
    public required int Hp { get; set; }
    public required int Attack { get; set; }
    public required int Defence { get; set; }
    public required int SpecialAttack { get; set; }
    public required int SpecialDefence { get; set; }
    public required int Speed { get; set; }

    public override bool Equals(PokedexPokemonStats? other)
    {
        return other?.Attack == Attack
            && other?.Defence == Defence
            && other.Hp == Hp
            && other.SpecialAttack == SpecialAttack
            && other.SpecialDefence == SpecialDefence
            && other.Speed == Speed;
    }

    public PokedexPokemonStatsRawJson ToJson()
    {
        return new PokedexPokemonStatsRawJson
        {
            Attack = Attack,
            Defence = Defence,
            Hp = Hp,
            Speed = Speed,
            SpecialAttack = SpecialAttack,
            SpecialDefence = SpecialDefence,
        };
    }
}
