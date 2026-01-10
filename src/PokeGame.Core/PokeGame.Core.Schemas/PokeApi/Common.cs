using System.Text.Json.Serialization;

namespace PokeGame.Core.Schemas.PokeApi
{
    /// <summary>
    /// Languages for translations of API resource information.
    /// </summary>
    public sealed record Language : NamedApiResource
    {
        /// <summary>
        /// The identifier for this resource.
        /// </summary>
        public override int Id { get; init; }

        internal new static string ApiEndpoint { get; } = "language";

        /// <summary>
        /// The name for this resource.
        /// </summary>
        public override string Name { get; init; }

        /// <summary>
        /// Whether or not the games are published in this language.
        /// </summary>
        public bool Official { get; init; }

        /// <summary>
        /// The two-letter code of the country where this language
        /// is spoken. Note that it is not unique.
        /// </summary>
        public string Iso639 { get; init; }

        /// <summary>
        /// The two-letter code of the language. Note that it is not
        /// unique.
        /// </summary>
        public string Iso3166 { get; init; }

        /// <summary>
        /// The name of this resource listed in different languages.
        /// </summary>
        public List<Names> Names { get; init; }

        /// <summary>
        /// Is endpoint case sensitive
        /// </summary>
        internal new static bool IsApiEndpointCaseSensitive { get; } = true;
    }

    /// <summary>
    /// A reference to an API object that does not have a `Name` property
    /// </summary>
    /// <typeparam name="T">The type of resource</typeparam>
    public sealed record ApiResource<T> : UrlNavigation<T> where T : ResourceBase { }

    /// <summary>
    /// The description for an API resource
    /// </summary>
    public sealed record Descriptions
    {
        /// <summary>
        /// The localized description for an API resource in a
        /// specific language.
        /// </summary>
        public required string Description { get; init; }

        /// <summary>
        /// The language this name is in.
        /// </summary>
        public required NamedApiResource<Language> Language { get; init; }
    }

    /// <summary>
    /// The effect of an API resource
    /// </summary>
    public sealed record Effects
    {
        /// <summary>
        /// The localized effect text for an API resource in a
        /// specific language.
        /// </summary>
        public required string Effect { get; init; }

        /// <summary>
        /// The language this effect is in.
        /// </summary>
        public required NamedApiResource<Language> Language { get; init; }
    }

    /// <summary>
    /// Encounter information for a Pokémon
    /// </summary>
    public sealed record Encounter
    {
        /// <summary>
        /// The lowest level the Pokémon could be encountered at.
        /// </summary>
        [JsonPropertyName("min_level")]
        public int MinLevel { get; init; }

        /// <summary>
        /// The highest level the Pokémon could be encountered at.
        /// </summary>
        [JsonPropertyName("max_level")]
        public int MaxLevel { get; init; }

        /// <summary>
        /// A list of condition values that must be in effect for this
        /// encounter to occur.
        /// </summary>
        [JsonPropertyName("condition_values")]
        public required List<NamedApiResource<EncounterConditionValue>> ConditionValues { get; init; }

        /// <summary>
        /// Percent chance that this encounter will occur.
        /// </summary>
        public int Chance { get; init; }

        /// <summary>
        /// The method by which this encounter happens.
        /// </summary>
        public required NamedApiResource<EncounterMethod> Method { get; init; }
    }

    /// <summary>
    /// A flavor text entry for an API resource
    /// </summary>
    public sealed record FlavorTexts
    {
        /// <summary>
        /// The localized flavor text for an API resource in a specific language.
        /// </summary>
        [JsonPropertyName("flavor_text")]
        public required string FlavorText { get; init; }

        /// <summary>
        /// The language this name is in.
        /// </summary>
        public required NamedApiResource<Language> Language { get; init; }
    }

    /// <summary>
    /// The index information for a generation game
    /// </summary>
    public sealed record GenerationGameIndex
    {
        /// <summary>
        /// The internal id of an API resource within game data.
        /// </summary>
        [JsonPropertyName("game_index")]
        public int GameIndex { get; init; }

        /// <summary>
        /// The generation relevent to this game index.
        /// </summary>
        public required NamedApiResource<Generation> Generation { get; init; }
    }

    /// <summary>
    /// The version detail information for a machine
    /// </summary>
    public sealed record MachineVersionDetail
    {
        /// <summary>
        /// The machine that teaches a move from an item.
        /// </summary>
        public required ApiResource<Machine> Machine { get; init; }

        /// <summary>
        /// The version group of this specific machine.
        /// </summary>
        [JsonPropertyName("version_group")]
        public required NamedApiResource<VersionGroup> VersionGroup { get; init; }
    }

    /// <summary>
    /// A name with language information
    /// </summary>
    public sealed record Names
    {
        /// <summary>
        /// The localized name for an API resource in a specific language.
        /// </summary>
        public required string Name { get; init; }

        /// <summary>
        /// The language this name is in.
        /// </summary>
        public required NamedApiResource<Language> Language { get; init; }
    }

    /// <summary>
    /// A reference to an API resource that has a `Name` property
    /// </summary>
    /// <typeparam name="T">The type of reference</typeparam>
    public sealed record NamedApiResource<T> : UrlNavigation<T> where T : ResourceBase
    {
        /// <summary>
        /// The name of the referenced resource.
        /// </summary>
        public required string Name { get; init; }
    }

    /// <summary>
    /// Class representing data from a previous generation.
    /// </summary>
    /// <typeparam name="TData">The type of the data.</typeparam>
    public record PastGenerationData<TData>
    {
        /// <summary>
        /// The final generation in which the Pokemon had the given data.
        /// </summary>
        public required NamedApiResource<Generation> Generation { get; init; }

        /// <summary>
        /// The previous data.
        /// </summary>
        public required TData Data { get; init; }
    }

    /// <summary>
    /// The long text for effect text entries
    /// </summary>
    public sealed record VerboseEffect
    {
        /// <summary>
        /// The localized effect text for an API resource in a
        /// specific language.
        /// </summary>
        public required string Effect { get; init; }

        /// <summary>
        /// The localized effect text in brief.
        /// </summary>
        [JsonPropertyName("short_effect")]
        public required string ShortEffect { get; init; }

        /// <summary>
        /// The language this effect is in.
        /// </summary>
        public required NamedApiResource<Language> Language { get; init; }
    }

    /// <summary>
    /// The detailed information for version encounter entries
    /// </summary>
    public sealed record VersionEncounterDetail
    {
        /// <summary>
        /// The game version this encounter happens in.
        /// </summary>
        public required NamedApiResource<Version> Version { get; init; }

        /// <summary>
        /// The total percentage of all encounter potential.
        /// </summary>
        [JsonPropertyName("max_chance")]
        public int MaxChance { get; init; }

        /// <summary>
        /// A list of encounters and their specifics.
        /// </summary>
        [JsonPropertyName("encounter_details")]
        public required List<Encounter> EncounterDetails { get; init; }
    }

    /// <summary>
    /// The index information for games
    /// </summary>
    public sealed record VersionGameIndex
    {
        /// <summary>
        /// The internal id of an API resource within game data.
        /// </summary>
        [JsonPropertyName("game_index")]
        public int GameIndex { get; init; }

        /// <summary>
        /// The version relevent to this game index.
        /// </summary>
        public required NamedApiResource<Version> Version { get; init; }
    }

    /// <summary>
    /// The version group flavor text information
    /// </summary>
    public sealed record VersionGroupFlavorText
    {
        /// <summary>
        /// The localized name for an API resource in a specific language.
        /// </summary>
        public required string Text { get; init; }

        /// <summary>
        /// The language this name is in.
        /// </summary>
        public required NamedApiResource<Language> Language { get; init; }

        /// <summary>
        /// The version group which uses this flavor text.
        /// </summary>
        [JsonPropertyName("version_group")]
        public required NamedApiResource<VersionGroup> VersionGroup { get; init; }
    }
}
