using System.Text.Json.Serialization;

namespace PokeGame.Core.Schemas.PokeApi
{
    /// <summary>
    /// Evolution chains are essentially family trees. They start with
    /// the lowest stage within a family and detail evolution conditions
    /// for each as well as Pokémon they can evolve into up through the
    /// hierarchy.
    /// </summary>
    public sealed record EvolutionChain : ApiResource
    {
        /// <summary>
        /// The identifier for this resource.
        /// </summary>
        public override int Id { get; init; }

        internal new static string ApiEndpoint { get; } = "evolution-chain";

        /// <summary>
        /// The item that a Pokémon would be holding
        /// when mating that would trigger the egg hatching
        /// a baby Pokémon rather than a basic Pokémon.
        /// </summary>
        [JsonPropertyName("baby_trigger_item")]
        public NamedApiResource<Item>? BabyTriggerItem { get; init; }

        /// <summary>
        /// The base chain link object. Each link contains
        /// evolution details for a Pokémon in the chain.
        /// Each link references the next Pokémon in the
        /// natural evolution order.
        /// </summary>
        public required ChainLink Chain { get; init; }
    }

    /// <summary>
    /// The linking information between a Pokémon and it's evolution(s)
    /// </summary>
    public sealed record ChainLink
    {
        /// <summary>
        /// Whether or not this link is for a baby Pokémon. This would
        /// only ever be true on the base link.
        /// </summary>
        [JsonPropertyName("is_baby")]
        public bool IsBaby { get; init; }

        /// <summary>
        /// The Pokémon species at this point in the evolution chain.
        /// </summary>
        public required NamedApiResource<PokemonSpecies> Species { get; init; }

        /// <summary>
        /// All details regarding the specific details of the referenced
        /// Pokémon species evolution.
        /// </summary>
        [JsonPropertyName("evolution_details")]
        public required List<EvolutionDetail> EvolutionDetails { get; init; }

        /// <summary>
        /// A List of chain objects.
        /// </summary>
        [JsonPropertyName("evolves_to")]
        public required List<ChainLink> EvolvesTo { get; init; }
    }

    /// <summary>
    /// The details for an evolution
    /// </summary>
    public sealed record EvolutionDetail
    {
        /// <summary>
        /// The item required to cause evolution this into Pokémon species.
        /// </summary>
        public NamedApiResource<Item>? Item { get; init; }

        /// <summary>
        /// The type of event that triggers evolution into this Pokémon
        /// species.
        /// </summary>
        public required NamedApiResource<EvolutionTrigger> Trigger { get; init; }

        /// <summary>
        /// The id of the gender of the evolving Pokémon species must be in
        /// order to evolve into this Pokémon species.
        /// </summary>
        public int? Gender { get; init; }

        /// <summary>
        /// The item the evolving Pokémon species must be holding during the
        /// evolution trigger event to evolve into this Pokémon species.
        /// </summary>
        [JsonPropertyName("held_item")]
        public NamedApiResource<Item>? HeldItem { get; init; }

        /// <summary>
        /// The move that must be known by the evolving Pokémon species
        /// during the evolution trigger event in order to evolve into
        /// this Pokémon species.
        /// </summary>
        [JsonPropertyName("known_move")]
        public NamedApiResource<Move>? KnownMove { get; init; }

        /// <summary>
        /// The evolving Pokémon species must know a move with this type
        /// during the evolution trigger event in order to evolve into this
        /// Pokémon species.
        /// </summary>
        [JsonPropertyName("known_move_type")]
        public NamedApiResource<Type>? KnownMoveType { get; init; }

        /// <summary>
        /// The location the evolution must be triggered at.
        /// </summary>
        public NamedApiResource<Location>? Location { get; init; }

        /// <summary>
        /// The minimum required level of the evolving Pokémon species to
        /// evolve into this Pokémon species.
        /// </summary>
        [JsonPropertyName("min_level")]
        public int? MinLevel { get; init; }

        /// <summary>
        /// The minimum required level of happiness the evolving Pokémon
        /// species to evolve into this Pokémon species.
        /// </summary>
        [JsonPropertyName("min_happiness")]
        public int? MinHappiness { get; init; }

        /// <summary>
        /// The minimum required level of beauty the evolving Pokémon species
        /// to evolve into this Pokémon species.
        /// </summary>
        [JsonPropertyName("min_beauty")]
        public int? MinBeauty { get; init; }

        /// <summary>
        /// The minimum required level of affection the evolving Pokémon
        /// species to evolve into this Pokémon species.
        /// </summary>
        [JsonPropertyName("min_affection")]
        public int? MinAffection { get; init; }

        /// <summary>
        /// Whether or not it must be raining in the overworld to cause
        /// evolution this Pokémon species.
        /// </summary>
        [JsonPropertyName("needs_overworld_rain")]
        public bool NeedsOverworldRain { get; init; }

        /// <summary>
        /// The Pokémon species that must be in the players party in
        /// order for the evolving Pokémon species to evolve into this
        /// Pokémon species.
        /// </summary>
        [JsonPropertyName("party_species")]
        public NamedApiResource<PokemonSpecies>? PartySpecies { get; init; }

        /// <summary>
        /// The player must have a Pokémon of this type in their party
        /// during the evolution trigger event in order for the evolving
        /// Pokémon species to evolve into this Pokémon species.
        /// </summary>
        [JsonPropertyName("party_type")]
        public NamedApiResource<Type>? PartyType { get; init; }

        /// <summary>
        /// The required relation between the Pokémon's Attack and Defense
        /// stats. 1 means Attack > Defense. 0 means Attack = Defense.
        /// -1 means Attack &gt; Defense.
        /// </summary>
        [JsonPropertyName("relative_physical_stats")]
        public int? RelativePhysicalStats { get; init; }

        /// <summary>
        /// The required time of day. Day or night.
        /// </summary>
        [JsonPropertyName("time_of_day")]
        public required string TimeOfDay { get; init; }

        /// <summary>
        /// Pokémon species for which this one must be traded.
        /// </summary>
        [JsonPropertyName("trade_species")]
        public NamedApiResource<PokemonSpecies>? TradeSpecies { get; init; }

        /// <summary>
        /// Whether or not the 3DS needs to be turned upside-down as this
        /// Pokémon levels up.
        /// </summary>
        [JsonPropertyName("turn_upside_down")]
        public bool TurnUpsideDown { get; init; }
    }

    /// <summary>
    /// Evolution triggers are the events and conditions that
    /// cause a Pokémon to evolve.
    /// </summary>
    public sealed record EvolutionTrigger : NamedApiResource
    {
        /// <summary>
        /// The identifier for this resource.
        /// </summary>
        public override int Id { get; init; }

        internal new static string ApiEndpoint { get; } = "evolution-trigger";

        /// <summary>
        /// The name for this resource.
        /// </summary>
        public override required string Name { get; init; }

        /// <summary>
        /// The name of this resource listed in different languages.
        /// </summary>
        public required List<Names> Names { get; init; }

        /// <summary>
        /// A list of pokemon species that result from this evolution
        /// trigger.
        /// </summary>
        [JsonPropertyName("pokemon_species")]
        public required List<NamedApiResource<PokemonSpecies>> PokemonSpecies { get; init; }
    }
}
