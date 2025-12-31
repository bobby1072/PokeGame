using System.Text.Json.Serialization;

namespace PokeGame.Core.Schemas.PokeApi
{
    /// <summary>
    /// Berries are small fruits that can provide HP and status condition restoration,
    /// stat enhancement, and even damage negation when eaten by Pokémon.
    /// </summary>
    public sealed record Berry : NamedApiResource
    {
        /// <summary>
        /// The identifier for this resource.
        /// </summary>
        public override int Id { get; init; }

        internal new static string ApiEndpoint { get; } = "berry";

        /// <summary>
        /// The name for this resource.
        /// </summary>
        public override required string Name { get; init; }

        /// <summary>
        /// Time it takes the tree to grow one stage, in hours.
        /// Berry trees go through four of these growth stages
        /// before they can be picked.
        /// </summary>
        [JsonPropertyName("growth_time")]
        public int GrowthTime { get; init; }

        /// <summary>
        /// The maximum number of these berries that can grow
        /// on one tree in Generation IV.
        /// </summary>
        [JsonPropertyName("max_harvest")]
        public int MaxHarvest { get; init; }

        /// <summary>
        /// The power of the move "Natural Gift" when used with
        /// this Berry.
        /// </summary>
        [JsonPropertyName("natural_gift_power")]
        public int NaturalGiftPower { get; init; }

        /// <summary>
        /// The size of this Berry, in millimeters.
        /// </summary>
        public int Size { get; init; }

        /// <summary>
        /// The smoothness of this Berry, used in making
        /// Pokeblocks of Poffins.
        /// </summary>
        public int Smoothness { get; init; }

        /// <summary>
        /// The speed at which this Berry dries out the soil as
        /// it grows. A higher rate means the soil dries out
        /// more quickly.
        /// </summary>
        [JsonPropertyName("soil_dryness")]
        public int SoilDryness { get; init; }

        /// <summary>
        /// The firmness of this berry, used in making Pokeblocks
        /// or Poffins.
        /// </summary>
        public required NamedApiResource<BerryFirmness> Firmness { get; init; }

        /// <summary>
        /// A list of references to each flavor a berry can have
        /// and the potency of each of those flavors in regards
        /// to this berry.
        /// </summary>
        public required List<BerryFlavorMap> Flavors { get; init; }

        /// <summary>
        /// Berries are actually items. This is a reference to
        /// the item specific data for this berry.
        /// </summary>
        public required NamedApiResource<Item> Item { get; init; }

        /// <summary>
        /// The type inherited by "Natural Gift" when used with
        /// this Berry.
        /// </summary>
        [JsonPropertyName("natural_gift_type")]
        public required NamedApiResource<Type> NaturalGiftType { get; init; }
    }

    /// <summary>
    /// The potency and flavor that a berry can have
    /// </summary>
    public sealed record BerryFlavorMap
    {
        /// <summary>
        /// How powerful the referenced flavor is for this
        /// berry.
        /// </summary>
        public int Potency { get; init; }

        /// <summary>
        /// The referenced berry flavor.
        /// </summary>
        public required NamedApiResource<BerryFlavor> Flavor { get; init; }
    }

    /// <summary>
    /// Berries can be soft or hard.
    /// </summary>
    public sealed record BerryFirmness : NamedApiResource
    {
        /// <summary>
        /// The identifier for this resource.
        /// </summary>
        public override int Id { get; init; }

        internal new static string ApiEndpoint { get; } = "berry-firmness";

        /// <summary>
        /// The name for this resource.
        /// </summary>
        public override required string Name { get; init; }

        /// <summary>
        /// A list of berries with this firmness.
        /// </summary>
        public required List<NamedApiResource<Berry>> Berries { get; init; }

        /// <summary>
        /// The name of this resource listed in different
        /// languages.
        /// </summary>
        public required List<Names> Names { get; init; }
    }

    /// <summary>
    /// Flavors determine whether a Pokémon will benefit or suffer from eating
    /// a berry based on their nature.
    /// </summary>
    public sealed record BerryFlavor : NamedApiResource
    {
        /// <summary>
        /// The identifier for this resource.
        /// </summary>
        public override int Id { get; init; }

        internal new static string ApiEndpoint { get; } = "berry-flavor";

        /// <summary>
        /// The name for this resource.
        /// </summary>
        public override required string Name { get; init; }

        /// <summary>
        /// A list of berries with this firmness.
        /// </summary>
        public required List<FlavorBerryMap> Berries { get; init; }

        /// <summary>
        /// The contest type that correlates with this berry
        /// flavor.
        /// </summary>
        [JsonPropertyName("contest_type")]
        public required NamedApiResource<ContestType> ContestType { get; init; }

        /// <summary>
        /// The name of this resource in different languages.
        /// </summary>
        public required List<Names> Names { get; init; }
    }

    /// <summary>
    /// The potency and flavor that a berry can have
    /// </summary>
    public sealed record FlavorBerryMap
    {
        /// <summary>
        /// How powerful this referenced flavor is for this
        /// berry.
        /// </summary>
        public int Potency { get; init; }

        /// <summary>
        /// The berry with the referenced flavor.
        /// </summary>
        public required NamedApiResource<Berry> Berry { get; init; }
    }
}
