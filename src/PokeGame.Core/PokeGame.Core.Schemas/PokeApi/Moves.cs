using System.Collections.Generic;
using System.Text.Json.Serialization;

namespace PokeGame.Core.Schemas.PokeApi
{
    /// <summary>
    /// Moves are the skills of Pokémon in battle. In battle, a Pokémon
    /// uses one move each turn. Some moves (including those learned by
    /// Hidden Machine) can be used outside of battle as well, usually
    /// for the purpose of removing obstacles or exploring new areas.
    /// </summary>
    public sealed record Move : NamedApiResource
    {
        /// <summary>
        /// The identifier for this resource.
        /// </summary>
        public override int Id { get; init; }

        internal new static string ApiEndpoint { get; } = "move";

        /// <summary>
        /// The name for this resource.
        /// </summary>
        public override string Name { get; init; }

        /// <summary>
        /// The percent value of how likely this move is to be successful.
        /// </summary>
        public int? Accuracy { get; init; }

        /// <summary>
        /// The percent value of how likely it is this moves effect will happen.
        /// </summary>
        [JsonPropertyName("effect_chance")]
        public int? EffectChance { get; init; }

        /// <summary>
        /// Power points. The number of times this move can be used.
        /// </summary>
        [JsonPropertyName("pp")]
        public int? PowerPoints { get; init; }

        /// <summary>
        /// A value between -8 and 8. Sets the order in which moves are executed
        /// during battle. See
        /// [Bulbapedia](http://bulbapedia.bulbagarden.net/wiki/Priority)
        /// for greater detail.
        /// </summary>
        public int Priority { get; init; }

        /// <summary>
        /// The base power of this move with a value of 0 if it does not have
        /// a base power.
        /// </summary>
        public int? Power { get; init; }

        /// <summary>
        /// A detail of normal and super contest combos that require this move.
        /// </summary>
        [JsonPropertyName("contest_combos")]
        public ContestComboSets? ContestCombos { get; init; }

        /// <summary>
        /// The type of appeal this move gives a Pokémon when used in a contest.
        /// </summary>
        [JsonPropertyName("contest_type")]
        public NamedApiResource<ContestType>? ContestType { get; init; }

        /// <summary>
        /// The effect the move has when used in a contest.
        /// </summary>
        [JsonPropertyName("contest_effect")]
        public ApiResource<ContestEffect>? ContestEffect { get; init; }

        /// <summary>
        /// The type of damage the move inflicts on the target, e.g. physical.
        /// </summary>
        [JsonPropertyName("sealed damage_record")]
        public required NamedApiResource<MoveDamageClass> DamageClass { get; init; }

        /// <summary>
        /// The effect of this move listed in different languages.
        /// </summary>
        [JsonPropertyName("effect_entries")]
        public List<VerboseEffect> EffectEntries { get; init; }

        /// <summary>
        /// The list of previous effects this move has had across version
        /// groups of the games.
        /// </summary>
        [JsonPropertyName("effect_changes")]
        public required List<AbilityEffectChange> EffectChanges { get; init; }

        /// <summary>
        /// The flavor text of this move listed in different languages.
        /// </summary>
        [JsonPropertyName("flavor_text_entries")]
        public required List<MoveFlavorText> FlavorTextEntries { get; init; }

        /// <summary>
        /// The generation in which this move was introduced.
        /// </summary>
        public required NamedApiResource<Generation> Generation { get; init; }

        /// <summary>The pokemon that learn this move.</summary>
        [JsonPropertyName("learned_by_pokemon")]
        public required List<NamedApiResource<Pokemon>> LearnedByPokemon { get; init; }

        /// <summary>
        /// A list of the machines that teach this move.
        /// </summary>
        public required List<MachineVersionDetail> Machines { get; init; }

        /// <summary>
        /// Metadata about this move
        /// </summary>
        public required MoveMetaData Meta { get; init; }

        /// <summary>
        /// The name of this resource listed in different languages.
        /// </summary>
        public required List<Names> Names { get; init; }

        /// <summary>
        /// A list of move resource value changes across version groups
        /// of the game.
        /// </summary>
        [JsonPropertyName("past_values")]
        public required List<PastMoveStatValues> PastValues { get; init; }

        /// <summary>
        /// A list of stats this moves effects and how much it
        /// effects them.
        /// </summary>
        [JsonPropertyName("stat_changes")]
        public required List<MoveStatChange> StatChanges { get; init; }

        /// <summary>
        /// The effect the move has when used in a super contest.
        /// </summary>
        [JsonPropertyName("super_contest_effect")]
        public ApiResource<SuperContestEffect>? SuperContestEffect { get; init; }

        /// <summary>
        /// The type of target that will receive the effects of the attack.
        /// </summary>
        public required NamedApiResource<MoveTarget> Target { get; init; }

        /// <summary>
        /// The elemental type of this move.
        /// </summary>
        public required NamedApiResource<Type> Type { get; init; }
    }

    /// <summary>
    /// A set of moves that are combos
    /// </summary>
    public sealed record ContestComboSets
    {
        /// <summary>
        /// A detail of moves this move can be used before or after,
        /// granting additional appeal points in contests.
        /// </summary>
        public ContestComboDetail? Normal { get; init; }

        /// <summary>
        /// A detail of moves this move can be used before or after,
        /// granting additional appeal points in super contests.
        /// </summary>
        public ContestComboDetail? Super { get; init; }
    }

    /// <summary>
    /// A detailed list of combos
    /// </summary>
    public sealed record ContestComboDetail
    {
        /// <summary>
        /// A list of moves to use before this move.
        /// </summary>
        [JsonPropertyName("use_before")]
        public required List<NamedApiResource<Move>> UseBefore { get; init; }

        /// <summary>
        /// A list of moves to use after this move.
        /// </summary>
        [JsonPropertyName("use_after")]
        public required List<NamedApiResource<Move>> UseAfter { get; init; }
    }

    /// <summary>
    /// The flavor text for a move
    /// </summary>
    public sealed record MoveFlavorText
    {
        /// <summary>
        /// The localized flavor text for an api resource in a
        /// specific language.
        /// </summary>
        [JsonPropertyName("flavor_text")]
        public required string FlavorText { get; init; }

        /// <summary>
        /// The language this name is in.
        /// </summary>
        public required NamedApiResource<Language> Language { get; init; }

        /// <summary>
        /// The version group that uses this flavor text.
        /// </summary>
        [JsonPropertyName("version_group")]
        public required NamedApiResource<VersionGroup> VersionGroup { get; init; }
    }

    /// <summary>
    /// The additional data for a move
    /// </summary>
    public sealed record MoveMetaData
    {
        /// <summary>
        /// The status ailment this move inflicts on its target.
        /// </summary>
        public required NamedApiResource<MoveAilment> Ailment { get; init; }

        /// <summary>
        /// The category of move this move falls under, e.g. damage or
        /// ailment.
        /// </summary>
        public required NamedApiResource<MoveCategory> Category { get; init; }

        /// <summary>
        /// The minimum number of times this move hits. Null if it always
        /// only hits once.
        /// </summary>
        [JsonPropertyName("min_hits")]
        public int? MinHits { get; init; }

        /// <summary>
        /// The maximum number of times this move hits. Null if it always
        /// only hits once.
        /// </summary>
        [JsonPropertyName("max_hits")]
        public int? MaxHits { get; init; }

        /// <summary>
        /// The minimum number of turns this move continues to take effect.
        /// Null if it always only lasts one turn.
        /// </summary>
        [JsonPropertyName("min_turns")]
        public int? MinTurns { get; init; }

        /// <summary>
        /// The maximum number of turns this move continues to take effect.
        /// Null if it always only lasts one turn.
        /// </summary>
        [JsonPropertyName("max_turns")]
        public int? MaxTurns { get; init; }

        /// <summary>
        /// HP drain (if positive) or Recoil damage (if negative), in percent
        /// of damage done.
        /// </summary>
        public int Drain { get; init; }

        /// <summary>
        /// The amount of hp gained by the attacking Pokemon, in percent of
        /// it's maximum HP.
        /// </summary>
        public int Healing { get; init; }

        /// <summary>
        /// Critical hit rate bonus.
        /// </summary>
        [JsonPropertyName("crit_rate")]
        public int CritRate { get; init; }

        /// <summary>
        /// The likelihood this attack will cause an ailment.
        /// </summary>
        [JsonPropertyName("ailment_chance")]
        public int AilmentChance { get; init; }

        /// <summary>
        /// The likelihood this attack will cause the target Pokémon to flinch.
        /// </summary>
        [JsonPropertyName("flinch_chance")]
        public int FlinchChance { get; init; }

        /// <summary>
        /// The likelihood this attack will cause a stat change in the target
        /// Pokémon.
        /// </summary>
        [JsonPropertyName("stat_chance")]
        public int StatChance { get; init; }
    }

    /// <summary>
    /// The status and the change for a move
    /// </summary>
    public sealed record MoveStatChange
    {
        /// <summary>
        /// The amount of change
        /// </summary>
        public int Change { get; init; }

        /// <summary>
        /// The stat being affected.
        /// </summary>
        public required NamedApiResource<Stat> Stat { get; init; }
    }

    /// <summary>
    /// Move status values
    /// </summary>
    public sealed record PastMoveStatValues
    {
        /// <summary>
        /// The percent value of how likely this move is to be successful.
        /// </summary>
        public int? Accuracy { get; init; }

        /// <summary>
        /// The percent value of how likely it is this moves effect will
        /// take effect.
        /// </summary>
        [JsonPropertyName("effect_chance")]
        public int? EffectChance { get; init; }

        /// <summary>
        /// The base power of this move with a value of 0 if it does not have
        /// a base power.
        /// </summary>
        /// <remarks>The docs lie - this is null</remarks>
        public int? Power { get; init; }

        /// <summary>
        /// Power points. The number of times this move can be used.
        /// </summary>
        public int? Pp { get; init; }

        /// <summary>
        /// The effect of this move listed in different languages.
        /// </summary>
        [JsonPropertyName("effect_entries")]
        public required List<VerboseEffect> EffectEntries { get; init; }

        /// <summary>
        /// The elemental type of this move.
        /// </summary>
        public NamedApiResource<Type>? Type { get; init; }

        /// <summary>
        /// The version group in which these move stat values were in effect.
        /// </summary>
        [JsonPropertyName("version_group")]
        public required NamedApiResource<VersionGroup> VersionGroup { get; init; }
    }

    /// <summary>
    /// Move Ailments are status conditions caused by moves used during battle.
    /// </summary>
    public sealed record MoveAilment : NamedApiResource
    {
        /// <summary>
        /// The identifier for this resource.
        /// </summary>
        public override int Id { get; init; }

        internal new static string ApiEndpoint { get; } = "move-ailment";

        /// <summary>
        /// The name for this resource.
        /// </summary>
        public required override string Name { get; init; }

        /// <summary>
        /// A list of moves that cause this ailment.
        /// </summary>
        public required List<NamedApiResource<Move>> Moves { get; init; }

        /// <summary>
        /// The name of this resource listed in different languages.
        /// </summary>
        public required List<Names> Names { get; init; }
    }

    /// <summary>
    /// Styles of moves when used in the Battle Palace.
    /// </summary>
    public sealed record MoveBattleStyle : NamedApiResource
    {
        /// <summary>
        /// The identifier for this resource.
        /// </summary>
        public override int Id { get; init; }

        internal new static string ApiEndpoint { get; } = "move-battle-style";

        /// <summary>
        /// The name for this resource.
        /// </summary>
        public required override string Name { get; init; }

        /// <summary>
        /// The name of this resource listed in different languages.
        /// </summary>
        public required List<Names> Names { get; init; }
    }

    /// <summary>
    /// Very general categories that loosely group move effects.
    /// </summary>
    public sealed record MoveCategory : NamedApiResource
    {
        /// <summary>
        /// The identifier for this resource.
        /// </summary>
        public override int Id { get; init; }

        internal new static string ApiEndpoint { get; } = "move-category";

        /// <summary>
        /// The name for this resource.
        /// </summary>
        public required override string Name { get; init; }

        /// <summary>
        /// A list of moves that fall into this category.
        /// </summary>
        public required List<NamedApiResource<Move>> Moves { get; init; }

        /// <summary>
        /// The description of this resource listed in different languages.
        /// </summary>
        public required List<Descriptions> Descriptions { get; init; }
    }

    /// <summary>
    /// Damage sealed recordes moves can have, e.g. physical, special, or non-damaging.
    /// </summary>
    public sealed record MoveDamageClass : NamedApiResource
    {
        /// <summary>
        /// The identifier for this resource.
        /// </summary>
        public override int Id { get; init; }

        internal new static string ApiEndpoint { get; } = "move-damage-sealed record";

        /// <summary>
        /// The name for this resource.
        /// </summary>
        public required override string Name { get; init; }

        /// <summary>
        /// A list of moves that fall into this damage public sealed record.
        /// </summary>
        public required List<NamedApiResource<Move>> Moves { get; init; }

        /// <summary>
        /// The description of this resource listed in different languages.
        /// </summary>
        public required List<Descriptions> Descriptions { get; init; }

        /// <summary>
        /// The name of this resource listed in different languages.
        /// </summary>
        public required List<Names> Names { get; init; }
    }

    /// <summary>
    /// Methods by which Pokémon can learn moves.
    /// </summary>
    public sealed record MoveLearnMethod : NamedApiResource
    {
        /// <summary>
        /// The identifier for this resource.
        /// </summary>
        public override int Id { get; init; }

        internal new static string ApiEndpoint { get; } = "move-learn-method";

        /// <summary>
        /// The name for this resource.
        /// </summary>
        public required override string Name { get; init; }

        /// <summary>
        /// The description of this resource listed in different languages.
        /// </summary>
        public required List<Descriptions> Descriptions { get; init; }

        /// <summary>
        /// The name of this resource listed in different languages.
        /// </summary>
        public required List<Names> Names { get; init; }

        /// <summary>
        /// A list of version groups where moves can be learned through this method.
        /// </summary>
        [JsonPropertyName("version_groups")]
        public required List<NamedApiResource<VersionGroup>> VersionGroups { get; init; }
    }

    /// <summary>
    /// Targets moves can be directed at during battle. Targets can be Pokémon,
    /// environments or even other moves.
    /// </summary>
    public sealed record MoveTarget : NamedApiResource
    {
        /// <summary>
        /// The identifier for this resource.
        /// </summary>
        public override int Id { get; init; }

        internal new static string ApiEndpoint { get; } = "move-target";

        /// <summary>
        /// The name for this resource.
        /// </summary>
        public required override string Name { get; init; }

        /// <summary>
        /// The description of this resource listed in different languages.
        /// </summary>
        public required List<Descriptions> Descriptions { get; init; }

        /// <summary>
        /// A list of moves that that are directed at this target.
        /// </summary>
        public required List<NamedApiResource<Move>> Moves { get; init; }

        /// <summary>
        /// The name of this resource listed in different languages.
        /// </summary>
        public required List<Names> Names { get; init; }
    }
}
