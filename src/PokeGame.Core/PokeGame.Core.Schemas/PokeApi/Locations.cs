
using System.Text.Json.Serialization;

namespace PokeGame.Core.Schemas.PokeApi
{
    /// <summary>
    /// Locations that can be visited within the games. Locations make
    /// up sizable portions of regions, like cities or routes.
    /// </summary>
    public sealed record Location : NamedApiResource
    {
        /// <summary>
        /// The identifier for this resource.
        /// </summary>
        public override int Id { get; init; }

        internal new static string ApiEndpoint { get; } = "location";

        /// <summary>
        /// The name for this resource.
        /// </summary>
        public required override string Name { get; init; }

        /// <summary>
        /// The region this location can be found in.
        /// </summary>
        public required NamedApiResource<Region> Region { get; init; }

        /// <summary>
        /// The name of this resource listed in different languages.
        /// </summary>
        public required List<Names> Names { get; init; }

        /// <summary>
        /// A list of game indices relevent to this location by generation.
        /// </summary>
        [JsonPropertyName("game_indices")]
        public required List<GenerationGameIndex> GameIndices { get; init; }

        /// <summary>
        /// Areas that can be found within this location
        /// </summary>
        public required List<NamedApiResource<LocationArea>> Areas { get; init; }
    }

    /// <summary>
    /// Location areas are sections of areas, such as floors in a building
    /// or cave. Each area has its own init of possible Pokémon encounters.
    /// </summary>
    public sealed record LocationArea : NamedApiResource
    {
        /// <summary>
        /// The identifier for this resource.
        /// </summary>
        public override int Id { get; init; }

        internal new static string ApiEndpoint { get; } = "location-area";

        /// <summary>
        /// The name for this resource.
        /// </summary>
        public required override string Name { get; init; }

        /// <summary>
        /// The internal id of an API resource within game data.
        /// </summary>
        [JsonPropertyName("game_index")]
        public int GameIndex { get; init; }

        /// <summary>
        /// A list of methods in which Pokémon may be encountered in this
        /// area and how likely the method will occur depending on the version
        /// of the game.
        /// </summary>
        [JsonPropertyName("encounter_method_rates")]
        public required List<EncounterMethodRate> EncounterMethodRates { get; init; }

        /// <summary>
        /// The region this location can be found in.
        /// </summary>
        public required NamedApiResource<Location> Location { get; init; }

        /// <summary>
        /// The name of this resource listed in different languages.
        /// </summary>
        public required List<Names> Names { get; init; }

        /// <summary>
        /// A list of Pokémon that can be encountered in this area along with
        /// version specific details about the encounter.
        /// </summary>
        [JsonPropertyName("pokemon_encounters")]
        public required List<PokemonEncounter> PokemonEncounters { get; init; }
    }

    /// <summary>
    /// A mapping between an encounter method and the version that applies
    /// </summary>
    public sealed record EncounterMethodRate
    {
        /// <summary>
        /// The method in which Pokémon may be encountered in an area.
        /// </summary>
        [JsonPropertyName("encounter_method")]
        public required NamedApiResource<EncounterMethod> EncounterMethod { get; init; }

        /// <summary>
        /// The chance of the encounter to occur on a version of the game.
        /// </summary>
        [JsonPropertyName("version_details")]
        public required List<EncounterVersionDetails> VersionDetails { get; init; }
    }

    /// <summary>
    /// The details for an encounter with the version
    /// </summary>
    public sealed record EncounterVersionDetails
    {
        /// <summary>
        /// The chance of an encounter to occur.
        /// </summary>
        public int Rate { get; init; }

        /// <summary>
        /// The version of the game in which the encounter can occur with
        /// the given chance.
        /// </summary>
        public required NamedApiResource<Version> Version { get; init; }
    }

    /// <summary>
    /// A Pokémon encounter and the version that encounter can happen
    /// </summary>
    public sealed record PokemonEncounter
    {
        /// <summary>
        /// The Pokémon being encountered.
        /// </summary>
        public required NamedApiResource<Pokemon> Pokemon { get; init; }

        /// <summary>
        /// A list of versions and encounters with Pokémon that might happen
        /// in the referenced location area.
        /// </summary>
        [JsonPropertyName("version_details")]
        public required List<VersionEncounterDetail> VersionDetails { get; init; }
    }

    /// <summary>
    /// Areas used for grouping Pokémon encounters in Pal Park. They're like
    /// habitats that are specific to Pal Park.
    /// </summary>
    public sealed record PalParkArea : NamedApiResource
    {
        /// <summary>
        /// The identifier for this resource.
        /// </summary>
        public override int Id { get; init; }

        internal new static string ApiEndpoint { get; } = "pal-park-area";

        /// <summary>
        /// The name for this resource.
        /// </summary>
        public required override string Name { get; init; }

        /// <summary>
        /// The name of this resource listed in different languages.
        /// </summary>
        public required List<Names> Names { get; init; }

        /// <summary>
        /// A list of Pokémon encountered in thi pal park area along with
        /// details.
        /// </summary>
        [JsonPropertyName("pokemon_encounters")]
        public required List<PalParkEncounterSpecies> PokemonEncounters { get; init; }
    }

    /// <summary>
    /// Information for an encounter in PalPark
    /// </summary>
    public sealed record PalParkEncounterSpecies
    {
        /// <summary>
        /// The base score given to the player when this Pokémon is caught
        /// during a pal park run.
        /// </summary>
        [JsonPropertyName("base_score")]
        public int BaseScore { get; init; }

        /// <summary>
        /// The base rate for encountering this Pokémon in this pal park area.
        /// </summary>
        public int Rate { get; init; }

        /// <summary>
        /// The Pokémon species being encountered.
        /// </summary>
        [JsonPropertyName("pokemon_species")]
        public required NamedApiResource<PokemonSpecies> PokemonSpecies { get; init; }
    }

    /// <summary>
    /// A region is an organized area of the Pokémon world. Most often,
    /// the main difference between regions is the species of Pokémon
    /// that can be encountered within them.
    /// </summary>
    public sealed record Region : NamedApiResource
    {
        /// <summary>
        /// The identifier for this resource.
        /// </summary>
        public override int Id { get; init; }

        internal new static string ApiEndpoint { get; } = "region";

        /// <summary>
        /// A list of locations that can be found in this region.
        /// </summary>
        public required List<NamedApiResource<Location>> Locations { get; init; }

        /// <summary>
        /// The name for this resource.
        /// </summary>
        public required override string Name { get; init; }

        /// <summary>
        /// The name of this resource listed in different languages.
        /// </summary>
        public required List<Names> Names { get; init; }

        /// <summary>
        /// The generation this region was introduced in.
        /// </summary>
        [JsonPropertyName("main_generation")]
        public required NamedApiResource<Generation> MainGeneration { get; init; }

        /// <summary>
        /// A list of pokédexes that catalogue Pokémon in this region.
        /// </summary>
        public required List<NamedApiResource<Pokedex>> Pokedexes { get; init; }

        /// <summary>
        /// A list of version groups where this region can be visited.
        /// </summary>
        [JsonPropertyName("version_groups")]
        public required List<NamedApiResource<VersionGroup>> VersionGroups { get; init; }
    }
}
