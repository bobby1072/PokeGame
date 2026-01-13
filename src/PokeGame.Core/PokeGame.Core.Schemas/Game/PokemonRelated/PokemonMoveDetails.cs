using PokeGame.Core.Schemas.Common;

namespace PokeGame.Core.Schemas.Game.PokemonRelated;

public sealed class PokemonMoveDetails : DomainModel<PokemonMoveDetails>
{
    public required string MoveName { get; init; }
    public required int Accuracy { get; init; }
    public required int? EffectChance { get; init; }
    public required int? PowerPoints { get; init; }
    public required int Priority { get; init; }
    public required int? Power { get; init; }
    public required DamageClassTypeEnum DamageClass { get; init; }
    public required string AilmentName { get; init; }
    public required int AilmentChance { get; init; }
    public required int FlinchChance { get; init; }
    public required int CritRate { get; init; }
    public required int StatChance { get; init; }
    public required int? MinTurns { get; init; }
    public required int? MaxTurns { get; init; }
    public required int Healing { get; init; }
    public required PokemonTypeEnum Type { get; init; }

    public override bool Equals(PokemonMoveDetails? other)
    {
        return MoveName == other?.MoveName
            && Accuracy == other.Accuracy
            && EffectChance == other.EffectChance
            && PowerPoints == other.PowerPoints
            && Priority == other.Priority
            && Power == other.Power
            && DamageClass == other.DamageClass
            && AilmentName == other.AilmentName
            && AilmentChance == other.AilmentChance
            && FlinchChance == other.FlinchChance
            && CritRate == other.CritRate
            && StatChance == other.StatChance
            && MinTurns == other.MinTurns
            && MaxTurns == other.MaxTurns
            && Healing == other.Healing
            && Type == other.Type;
    }
}
