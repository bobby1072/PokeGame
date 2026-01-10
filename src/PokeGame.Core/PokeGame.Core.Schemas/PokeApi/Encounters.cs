
namespace PokeGame.Core.Schemas.PokeApi
{
    /// <summary>
    /// Methods by which the player might can encounter Pokémon
    /// in the wild, e.g., walking in tall grass.
    /// </summary>
    public sealed record EncounterMethod : NamedApiResource
    {
        /// <summary>
        /// The identifier for this resource.
        /// </summary>
        public override int Id { get; init; }

        internal new static string ApiEndpoint { get; } = "encounter-method";

        /// <summary>
        /// The name for this resource.
        /// </summary>
        public override required string Name { get; init; }

        /// <summary>
        /// A good value for sorting.
        /// </summary>
        public int Order { get; init; }

        /// <summary>
        /// The name of this resource listed in different
        /// languages.
        /// </summary>
        public required List<Names> Names { get; init; }
    }

    /// <summary>
    /// Conditions which affect what pokemon might appear in the
    /// wild, e.g., day or night.
    /// </summary>
    public sealed record EncounterCondition : NamedApiResource
    {
        /// <summary>
        /// The identifier for this resource.
        /// </summary>
        public override int Id { get; init; }

        internal new static string ApiEndpoint { get; } = "encounter-condition";

        /// <summary>
        /// The name for this resource.
        /// </summary>
        public override required string Name { get; init; }

        /// <summary>
        /// The name of this resource listed in different
        /// languages.
        /// </summary>
        public required List<Names> Names { get; init; }

        /// <summary>
        /// A list of possible values for this encounter condition.
        /// </summary>
        public required List<NamedApiResource<EncounterConditionValue>> Values { get; init; }
    }

    /// <summary>
    /// Encounter condition values are the various states that an encounter
    /// condition can have, i.e., time of day can be either day or night.
    /// </summary>
    public sealed record EncounterConditionValue : NamedApiResource
    {
        /// <summary>
        /// The identifier for this resource.
        /// </summary>
        public override int Id { get; init; }

        internal new static string ApiEndpoint { get; } = "encounter-condition-value";

        /// <summary>
        /// The name for this resource.
        /// </summary>
        public override required string Name { get; init; }

        /// <summary>
        /// The condition this encounter condition value pertains
        /// to.
        /// </summary>
        public required NamedApiResource<EncounterCondition> Condition { get; init; }

        /// <summary>
        /// The name of this resource listed in different
        /// languages.
        /// </summary>
        public required List<Names> Names { get; init; }
    }
}
