using System.Text.Json.Serialization;

namespace PokeGame.Core.Schemas.PokeApi
{
    /// <summary>
    /// A generation is a grouping of the Pokémon games that separates
    /// them based on the Pokémon they include. In each generation, a new
    /// set of Pokémon, Moves, Abilities and Types that did not exist in
    /// the previous generation are released.
    /// </summary>
    public sealed record Generation : NamedApiResource
    {
        /// <summary>
        /// The identifier for this resource.
        /// </summary>
        public override int Id { get; init; }

        internal new static string ApiEndpoint { get; } = "generation";

        /// <summary>
        /// The name for this resource.
        /// </summary>
        public override string Name { get; init; }

        /// <summary>
        /// A list of abilities that were introduced in this generation.
        /// </summary>
        public required List<NamedApiResource<Ability>> Abilities { get; init; }

        /// <summary>
        /// The name of this resource listed in different languages.
        /// </summary>
        public List<Names> Names { get; init; }

        /// <summary>
        /// The main region travelled in this generation.
        /// </summary>
        [JsonPropertyName("main_region")]
        public required NamedApiResource<Region> MainRegion { get; init; }

        /// <summary>
        /// A list of moves that were introduced in this generation.
        /// </summary>
        public required List<NamedApiResource<Move>> Moves { get; init; }

        /// <summary>
        /// A list of Pokemon species that were introduced in this
        /// generation.
        /// </summary>
        [JsonPropertyName("pokemon_species")]
        public required List<NamedApiResource<PokemonSpecies>> PokemonSpecies { get; init; }

        /// <summary>
        /// A list of types that were introduced in this generation.
        /// </summary>
        public required List<NamedApiResource<Type>> Types { get; init; }

        /// <summary>
        /// A list of version groups that were introduced in this
        /// generation.
        /// </summary>
        [JsonPropertyName("version_groups")]
        public required List<NamedApiResource<VersionGroup>> VersionGroups { get; init; }
    }

    /// <summary>
    /// A Pokédex is a handheld electronic encyclopedia device; one which
    /// is capable of recording and retaining information of the various
    /// Pokémon in a given region with the exception of the national dex
    /// and some smaller dexes related to portions of a region.
    /// </summary>
    public sealed record Pokedex : NamedApiResource
    {
        /// <summary>
        /// The identifier for this resource.
        /// </summary>
        public override int Id { get; init; }

        internal new static string ApiEndpoint { get; } = "pokedex";

        /// <summary>
        /// The name for this resource.
        /// </summary>
        public required override string Name { get; init; }

        /// <summary>
        /// Whether or not this Pokédex originated in the main series of the video games.
        /// </summary>
        [JsonPropertyName("is_main_series")]
        public bool IsMainSeries { get; init; }

        /// <summary>
        /// The description of this resource listed in different languages.
        /// </summary>
        public required List<Descriptions> Descriptions { get; init; }

        /// <summary>
        /// The name of this resource listed in different languages.
        /// </summary>
        public required List<Names> Names { get; init; }

        /// <summary>
        /// A list of Pokémon catalogued in this Pokédex and their indexes.
        /// </summary>
        [JsonPropertyName("pokemon_entries")]
        public required List<PokemonEntry> PokemonEntries { get; init; }

        /// <summary>
        /// The region this Pokédex catalogues Pokémon for.
        /// </summary>
        public NamedApiResource<Region>? Region { get; init; }

        /// <summary>
        /// A list of version groups this Pokédex is relevant to.
        /// </summary>
        [JsonPropertyName("version_groups")]
        public required List<NamedApiResource<VersionGroup>> VersionGroups { get; init; }
    }

    /// <summary>
    /// The entry information
    /// </summary>
    public sealed record PokemonEntry
    {
        /// <summary>
        /// The index of this Pokémon species entry within the Pokédex.
        /// </summary>
        [JsonPropertyName("entry_number")]
        public int EntryNumber { get; init; }

        /// <summary>
        /// The Pokémon species being encountered.
        /// </summary>
        [JsonPropertyName("pokemon_species")]
        public required NamedApiResource<PokemonSpecies> PokemonSpecies { get; init; }
    }

    /// <summary>
    /// Versions of the games, e.g., Red, Blue or Yellow.
    /// </summary>
    public sealed record Version : NamedApiResource
    {
        /// <summary>
        /// The identifier for this resource.
        /// </summary>
        public override int Id { get; init; }

        internal new static string ApiEndpoint { get; } = "version";

        /// <summary>
        /// The name for this resource.
        /// </summary>
        public required override string Name { get; init; }

        /// <summary>
        /// The name of this resource listed in different languages.
        /// </summary>
        public required List<Names> Names { get; init; }

        /// <summary>
        /// The version group this version belongs to.
        /// </summary>
        [JsonPropertyName("version_group")]
        public required NamedApiResource<VersionGroup> VersionGroup { get; init; }
    }

    /// <summary>
    /// Version groups categorize highly similar versions of the games.
    /// </summary>
    public sealed record VersionGroup : NamedApiResource
    {
        /// <summary>
        /// The identifier for this resource.
        /// </summary>
        public override int Id { get; init; }

        internal new static string ApiEndpoint { get; } = "version-group";

        /// <summary>
        /// The name for this resource.
        /// </summary>
        public required override string Name { get; init; }

        /// <summary>
        /// Order for sorting. Almost by date of release,
        /// except similar versions are grouped together.
        /// </summary>
        public int Order { get; init; }

        /// <summary>
        /// The generation this version was introduced in.
        /// </summary>
        public required NamedApiResource<Generation> Generation { get; init; }

        /// <summary>
        /// A list of methods in which Pokémon can learn moves in
        /// this version group.
        /// </summary>
        [JsonPropertyName("move_learn_methods")]
        public required List<NamedApiResource<MoveLearnMethod>> MoveLearnMethods { get; init; }

        /// <summary>
        /// A list of Pokédexes introduces in this version group.
        /// </summary>
        public required List<NamedApiResource<Pokedex>> Pokedexes { get; init; }

        /// <summary>
        /// A list of regions that can be visited in this version group.
        /// </summary>
        public required List<NamedApiResource<Region>> Regions { get; init; }

        /// <summary>
        /// The versions this version group owns.
        /// </summary>
        public required List<NamedApiResource<Version>> Versions { get; init; }
    }
}
