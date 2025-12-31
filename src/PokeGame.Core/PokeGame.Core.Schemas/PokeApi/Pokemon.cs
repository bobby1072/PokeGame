using System.Text.Json.Serialization;

namespace PokeGame.Core.Schemas.PokeApi
{
    /// <summary>
    /// Abilities provide passive effects for Pokémon in battle or in
    /// the overworld. Pokémon have multiple possible abilities but
    /// can have only one ability at a time.
    /// </summary>
    public sealed record Ability : NamedApiResource
    {
        /// <summary>
        /// The identifier for this resource.
        /// </summary>
        public override int Id { get; init; }

        internal static new string ApiEndpoint { get; } = "ability";

        /// <summary>
        /// The name for this resource.
        /// </summary>
        public required override string Name { get; init; }

        /// <summary>
        /// Whether or not this ability originated in the main series of the video games.
        /// </summary>
        [JsonPropertyName("is_main_series")]
        public bool IsMainSeries { get; init; }

        /// <summary>
        /// The generation this ability originated in.
        /// </summary>
        public required NamedApiResource<Generation> Generation { get; init; }

        /// <summary>
        /// The name of this resource listed in different languages.
        /// </summary>
        public required List<Names> Names { get; init; }

        /// <summary>
        /// The effect of this ability listed in different languages.
        /// </summary>
        [JsonPropertyName("effect_entries")]
        public required List<VerboseEffect> EffectEntries { get; init; }

        /// <summary>
        /// The list of previous effects this ability has had across version groups.
        /// </summary>
        [JsonPropertyName("effect_changes")]
        public required List<AbilityEffectChange> EffectChanges { get; init; }

        /// <summary>
        /// The flavor text of this ability listed in different languages.
        /// </summary>
        [JsonPropertyName("flavor_text_entries")]
        public required List<AbilityFlavorText> FlavorTextEntries { get; init; }

        /// <summary>
        /// A list of Pokémon that could potentially have this ability.
        /// </summary>
        public required List<AbilityPokemon> Pokemon { get; init; }
    }

    /// <summary>
    /// An ability and it's associated versions
    /// </summary>
    public sealed record AbilityEffectChange
    {
        /// <summary>
        /// The previous effect of this ability listed in different languages.
        /// </summary>
        [JsonPropertyName("effect_entries")]
        public required List<Effects> EffectEntries { get; init; }

        /// <summary>
        /// The version group in which the previous effect of this ability originated.
        /// </summary>
        [JsonPropertyName("version_group")]
        public required NamedApiResource<VersionGroup> VersionGroup { get; init; }
    }

    /// <summary>
    /// The flavor text for an ability
    /// </summary>
    public sealed record AbilityFlavorText
    {
        /// <summary>
        /// The localized name for an API resource in a specific language.
        /// </summary>
        [JsonPropertyName("flavor_text")]
        public required string FlavorText { get; init; }

        /// <summary>
        /// The language this text resource is in.
        /// </summary>
        public required NamedApiResource<Language> Language { get; init; }

        /// <summary>
        /// The version group that uses this flavor text.
        /// </summary>
        [JsonPropertyName("version_group")]
        public required NamedApiResource<VersionGroup> VersionGroup { get; init; }
    }

    /// <summary>
    /// A mapping of an ability to a possible Pokémon
    /// </summary>
    public sealed record AbilityPokemon
    {
        /// <summary>
        /// Whether or not this a hidden ability for the referenced Pokémon.
        /// </summary>
        [JsonPropertyName("is_hidden")]
        public bool IsHidden { get; init; }

        /// <summary>
        /// Pokémon have 3 ability 'slots' which hold references to possible
        /// abilities they could have. This is the slot of this ability for the
        /// referenced pokemon.
        /// </summary>
        public int Slot { get; init; }

        /// <summary>
        /// The Pokémon this ability could belong to.
        /// </summary>
        public required NamedApiResource<Pokemon> Pokemon { get; init; }
    }

    /// <summary>
    /// Characteristics indicate which stat contains a Pokémon's highest IV.
    /// A Pokémon's Characteristic is determined by the remainder of its
    /// highest IV divided by 5 (gene_modulo).
    /// </summary>
    public sealed record Characteristic : ApiResource
    {
        /// <summary>
        /// The identifier for this resource.
        /// </summary>
        public override int Id { get; init; }

        internal static new string ApiEndpoint { get; } = "characteristic";

        /// <summary>
        /// The remainder of the highest stat/IV divided by 5.
        /// </summary>
        [JsonPropertyName("gene_modulo")]
        public int GeneModulo { get; init; }

        /// <summary>
        /// The possible values of the highest stat that would result in
        /// a Pokémon recieving this characteristic when divided by 5.
        /// </summary>
        [JsonPropertyName("possible_values")]
        public required List<int> PossibleValues { get; init; }

        /// <summary>
        /// The highest stat of this characteristic.
        /// </summary>
        [JsonPropertyName("highest_stat")]
        public required NamedApiResource<Stat> HighestStat { get; init; }

        /// <summary>
        /// The descriptions of this characteristic listed in different languages.
        /// </summary>
        public required List<Descriptions> Descriptions { get; init; }
    }

    /// <summary>
    /// Egg Groups are categories which determine which Pokémon are able
    /// to interbreed. Pokémon may belong to either one or two Egg Groups.
    /// </summary>
    public sealed record EggGroup : NamedApiResource
    {
        /// <summary>
        /// The identifier for this resource.
        /// </summary>
        public override int Id { get; init; }

        internal static new string ApiEndpoint { get; } = "egg-group";

        /// <summary>
        /// The name for this resource.
        /// </summary>
        public required override string Name { get; init; }

        /// <summary>
        ///	The name of this resource listed in different languages.
        /// </summary>
        public required List<Names> Names { get; init; }

        /// <summary>
        /// A list of all Pokémon species that are members of this egg group.
        /// </summary>
        [JsonPropertyName("pokemon_species")]
        public required List<NamedApiResource<PokemonSpecies>> PokemonSpecies { get; init; }
    }

    /// <summary>
    /// Genders were introduced in Generation II for the purposes of
    /// breeding Pokémon but can also result in visual differences or
    /// even different evolutionary lines
    /// </summary>
    public sealed record Gender : NamedApiResource
    {
        /// <summary>
        /// The identifier for this resource.
        /// </summary>
        public override int Id { get; init; }

        internal static new string ApiEndpoint { get; } = "gender";

        /// <summary>
        /// The name for this resource.
        /// </summary>
        public required override string Name { get; init; }

        /// <summary>
        /// A list of Pokémon species that can be this gender and how likely it
        /// is that they will be.
        /// </summary>
        [JsonPropertyName("pokemon_species_details")]
        public required List<PokemonSpeciesGender> PokemonSpeciesDetails { get; init; }

        /// <summary>
        /// A list of Pokémon species that required this gender in order for a
        /// Pokémon to evolve into them.
        /// </summary>
        [JsonPropertyName("required_for_evolution")]
        public required List<NamedApiResource<PokemonSpecies>> RequiredForEvolution { get; init; }
    }

    /// <summary>
    /// A rate of chance of a Pokémon being a specific gender
    /// </summary>
    public sealed record PokemonSpeciesGender
    {
        /// <summary>
        /// The chance of this Pokémon being female, in eighths; or -1 for
        /// genderless.
        /// </summary>
        public int Rate { get; init; }

        /// <summary>
        /// A Pokémon species that can be the referenced gender.
        /// </summary>
        [JsonPropertyName("pokemon_species")]
        public required NamedApiResource<PokemonSpecies> PokemonSpecies { get; init; }
    }

    /// <summary>
    /// Growth rates are the speed with which Pokémon gain levels through experience.
    /// </summary>
    public sealed record GrowthRate : NamedApiResource
    {
        /// <summary>
        /// The identifier for this resource.
        /// </summary>
        public override int Id { get; init; }

        internal static new string ApiEndpoint { get; } = "growth-rate";

        /// <summary>
        /// The name for this resource.
        /// </summary>
        public required override string Name { get; init; }

        /// <summary>
        /// The formula used to calculate the rate at which the Pokémon species
        /// gains level.
        /// </summary>
        public required string Formula { get; init; }

        /// <summary>
        /// The descriptions of this characteristic listed in different languages.
        /// </summary>
        public required List<Descriptions> Descriptions { get; init; }

        /// <summary>
        /// A list of levels and the amount of experienced needed to atain them
        /// based on this growth rate.
        /// </summary>
        public required List<GrowthRateExperienceLevel> Levels { get; init; }

        /// <summary>
        /// A list of Pokémon species that gain levels at this growth rate.
        /// </summary>
        [JsonPropertyName("pokemon_species")]
        public required List<NamedApiResource<PokemonSpecies>> PokemonSpecies { get; init; }
    }

    /// <summary>
    /// Information of a level and how much experience is needed to get to it
    /// </summary>
    public sealed record GrowthRateExperienceLevel
    {
        /// <summary>
        /// The level gained.
        /// </summary>
        public int Level { get; init; }

        /// <summary>
        /// The amount of experience required to reach the referenced level.
        /// </summary>
        public int Experience { get; init; }
    }

    /// <summary>
    /// Natures influence how a Pokémon's stats grow.
    /// </summary>
    public sealed record Nature : NamedApiResource
    {
        /// <summary>
        /// The identifier for this resource.
        /// </summary>
        public override int Id { get; init; }

        internal static new string ApiEndpoint { get; } = "nature";

        /// <summary>
        /// The name for this resource.
        /// </summary>
        public required override string Name { get; init; }

        /// <summary>
        /// The stat decreased by 10% in Pokémon with this nature.
        /// </summary>
        [JsonPropertyName("decreased_stat")]
        public required NamedApiResource<Stat> DecreasedStat { get; init; }

        /// <summary>
        /// The stat increased by 10% in Pokémon with this nature.
        /// </summary>
        [JsonPropertyName("increased_stat")]
        public required NamedApiResource<Stat> IncreasedStat { get; init; }

        /// <summary>
        /// The flavor hated by Pokémon with this nature.
        /// </summary>
        [JsonPropertyName("hates_flavor")]
        public required NamedApiResource<BerryFlavor> HatesFlavor { get; init; }

        /// <summary>
        /// The flavor liked by Pokémon with this nature.
        /// </summary>
        [JsonPropertyName("likes_flavor")]
        public required NamedApiResource<BerryFlavor> LikesFlavor { get; init; }

        /// <summary>
        /// A list of Pokéathlon stats this nature effects and how much it
        /// effects them.
        /// </summary>
        [JsonPropertyName("pokeathlon_stat_changes")]
        public required List<NatureStatChange> PokeathlonStatChanges { get; init; }

        /// <summary>
        /// A list of battle styles and how likely a Pokémon with this nature is
        /// to use them in the Battle Palace or Battle Tent.
        /// </summary>
        [JsonPropertyName("move_battle_style_preferences")]
        public required List<MoveBattleStylePreference> MoveBattleStylePreferences { get; init; }

        /// <summary>
        /// The name of this resource listed in different languages.
        /// </summary>
        public required List<Names> Names { get; init; }
    }

    /// <summary>
    /// The change information for a nature
    /// </summary>
    public sealed record NatureStatChange
    {
        /// <summary>
        /// The amount of change.
        /// </summary>
        [JsonPropertyName("max_changes")]
        public int MaxChange { get; init; }

        /// <summary>
        /// The stat being affected.
        /// </summary>
        [JsonPropertyName("pokeathlon_stat")]
        public required NamedApiResource<PokeathlonStat> PokeathlonStat { get; init; }
    }

    /// <summary>
    /// Move information for a battle style
    /// </summary>
    public sealed record MoveBattleStylePreference
    {
        /// <summary>
        /// Chance of using the move, in percent, if HP is under one half.
        /// </summary>
        [JsonPropertyName("low_hp_preference")]
        public int LowHpPreference { get; init; }

        /// <summary>
        /// Chance of using the move, in percent, if HP is over one half.
        /// </summary>
        [JsonPropertyName("high_hp_preference")]
        public int HighHpPreference { get; init; }

        /// <summary>
        /// The move battle style.
        /// </summary>
        [JsonPropertyName("move_battle_style")]
        public required NamedApiResource<MoveBattleStyle> MoveBattleStyle { get; init; }
    }

    /// <summary>
    /// Pokeathlon Stats are different attributes of a Pokémon's performance
    /// in Pokéathlons. In Pokéathlons, competitions happen on different
    /// courses; one for each of the different Pokéathlon stats.
    /// </summary>
    public sealed record PokeathlonStat : NamedApiResource
    {
        /// <summary>
        /// The identifier for this resource.
        /// </summary>
        public override int Id { get; init; }

        internal static new string ApiEndpoint { get; } = "pokeathlon-stat";

        /// <summary>
        /// The name for this resource.
        /// </summary>
        public required override string Name { get; init; }

        /// <summary>
        /// The name of this resource listed in different languages.
        /// </summary>
        public required List<Names> Names { get; init; }

        /// <summary>
        /// A detail of natures which affect this Pokéathlon stat positively
        /// or negatively.
        /// </summary>
        [JsonPropertyName("affecting_natures")]
        public required NaturePokeathlonStatAffectSets AffectingNatures { get; init; }
    }

    /// <summary>
    /// The natures and how they are changed with the referenced Pokéathlon stat
    /// </summary>
    public sealed record NaturePokeathlonStatAffectSets
    {
        /// <summary>
        /// A list of natures and how they change the referenced Pokéathlon stat.
        /// </summary>
        public required List<NaturePokeathlonStatAffect> Increase { get; init; }

        /// <summary>
        /// A list of natures and how they change the referenced Pokéathlon stat.
        /// </summary>
        public required List<NaturePokeathlonStatAffect> Decrease { get; init; }
    }

    /// <summary>
    /// The change information for a Pokéathlon stat
    /// </summary>
    public sealed record NaturePokeathlonStatAffect
    {
        /// <summary>
        /// The maximum amount of change to the referenced Pokéathlon stat.
        /// </summary>
        [JsonPropertyName("max_change")]
        public int MaxChange { get; init; }

        /// <summary>
        /// The nature causing the change.
        /// </summary>
        public required NamedApiResource<Nature> Nature { get; init; }
    }

    /// <summary>
    /// Pokémon are the creatures that inhabit the world of the Pokémon games.
    /// They can be caught using Pokéballs and trained by battling with other Pokémon.
    /// Each Pokémon belongs to a specific species but may take on a variant which
    /// makes it differ from other Pokémon of the same species, such as base stats,
    /// available abilities and typings.
    /// </summary>
    public sealed record Pokemon : NamedApiResource
    {
        /// <summary>
        /// The identifier for this resource.
        /// </summary>
        public override int Id { get; init; }

        internal static new string ApiEndpoint { get; } = "pokemon";

        /// <summary>
        /// The name for this resource.
        /// </summary>
        public required override string Name { get; init; }

        /// <summary>
        /// The base experience gained for defeating this Pokémon.
        /// </summary>
        [JsonPropertyName("base_experience")]
        public int BaseExperienceFromDefeating { get; init; }

        /// <summary>
        /// The height of this Pokémon in decimetres.
        /// </summary>
        public int Height { get; init; }

        /// <summary>
        /// Set for exactly one Pokémon used as the default for each species.
        /// </summary>
        [JsonPropertyName("is_default")]
        public bool IsDefault { get; init; }

        /// <summary>
        /// Order for sorting. Almost national order, except families
        /// are grouped together.
        /// </summary>
        public int Order { get; init; }

        /// <summary>
        /// The weight of this Pokémon in hectograms.
        /// </summary>
        public int Weight { get; init; }

        /// <summary>
        /// A list of abilities this Pokémon could potentially have.
        /// </summary>
        public required List<PokemonAbility> Abilities { get; init; }

        /// <summary>
        /// A list of forms this Pokémon can take on.
        /// </summary>
        public required List<NamedApiResource<PokemonForm>> Forms { get; init; }

        /// <summary>
        /// A list of game indices relevent to Pokémon item by generation.
        /// </summary>
        [JsonPropertyName("game_indices")]
        public required List<VersionGameIndex> GameIndicies { get; init; }

        /// <summary>
        /// A list of items this Pokémon may be holding when encountered.
        /// </summary>
        [JsonPropertyName("held_items")]
        public required List<PokemonHeldItem> HeldItems { get; init; }

        /// <summary>
        /// A link to a list of location areas, as well as encounter
        /// details pertaining to specific versions.
        /// </summary>
        [JsonPropertyName("location_area_encounters")]
        public required string LocationAreaEncounters { get; init; }

        /// <summary>
        /// A list of moves along with learn methods and level
        /// details pertaining to specific version groups.
        /// </summary>
        public required List<PokemonMove> Moves { get; init; }

        /// <summary>
        /// Type data in previous generations for this Pokemon.
        /// </summary>
        [JsonPropertyName("past_types")]
        public required List<PokemonPastTypes> PastTypes { get; init; }

        /// <summary>
        /// A init of sprites used to depict this Pokémon in the game.
        /// </summary>
        public required PokemonSprites Sprites { get; init; }

        /// <summary>
        /// The species this Pokémon belongs to.
        /// </summary>
        public required NamedApiResource<PokemonSpecies> Species { get; init; }

        /// <summary>
        /// A list of base stat values for this Pokémon.
        /// </summary>
        public required List<PokemonStat> Stats { get; init; }

        /// <summary>
        /// A list of details showing types this Pokémon has.
        /// </summary>
        public required List<PokemonType> Types { get; init; }
    }

    /// <summary>
    /// A Pokémon ability
    /// </summary>
    public sealed record PokemonAbility
    {
        /// <summary>
        /// Whether or not this is a hidden ability.
        /// </summary>
        [JsonPropertyName("is_hidden")]
        public bool IsHidden { get; init; }

        /// <summary>
        /// The slot this ability occupies in this Pokémon species.
        /// </summary>
        public int Slot { get; init; }

        /// <summary>
        /// The ability the Pokémon may have.
        /// </summary>
        public required NamedApiResource<Ability> Ability { get; init; }
    }

    /// <summary>
    /// A Pokémon type
    /// </summary>
    public sealed record PokemonType
    {
        /// <summary>
        /// The order the Pokémon's types are listed in.
        /// </summary>
        public int Slot { get; init; }

        /// <summary>
        /// The type the referenced Pokémon has.
        /// </summary>
        public required NamedApiResource<Type> Type { get; init; }
    }

    /// <summary>
    /// Class for storing a Pokemon's type data in a previous generation.
    /// </summary>
    public sealed record PokemonPastTypes : PastGenerationData<List<PokemonType>>
    {
        /// <summary>
        /// The previous list of types.
        /// </summary>
        public List<PokemonType> Types
        {
            get => Data;
            init { Data = value; }
        }
    }

    /// <summary>
    /// A Pokémon held item
    /// </summary>
    public sealed record PokemonHeldItem
    {
        /// <summary>
        /// The item the referenced Pokémon holds.
        /// </summary>
        public required NamedApiResource<Item> Item { get; init; }

        /// <summary>
        /// The details of the different versions in which the item is held.
        /// </summary>
        [JsonPropertyName("version_details")]
        public required List<PokemonHeldItemVersion> VersionDetails { get; init; }
    }

    /// <summary>
    /// A Pokémon held item and version information
    /// </summary>
    public sealed record PokemonHeldItemVersion
    {
        /// <summary>
        /// The version in which the item is held.
        /// </summary>
        public required NamedApiResource<Version> Version { get; init; }

        /// <summary>
        /// How often the item is held.
        /// </summary>
        public int Rarity { get; init; }
    }

    /// <summary>
    /// A reference to a move and the version information
    /// </summary>
    public sealed record PokemonMove
    {
        /// <summary>
        /// The move the Pokémon can learn.
        /// </summary>
        public required NamedApiResource<Move> Move { get; init; }

        /// <summary>
        /// The details of the version in which the Pokémon can learn the move.
        /// </summary>
        [JsonPropertyName("version_group_details")]
        public required List<PokemonMoveVersion> VersionGroupDetails { get; init; }
    }

    /// <summary>
    /// The moves a Pokémon learns in which versions
    /// </summary>
    public sealed record PokemonMoveVersion
    {
        /// <summary>
        /// The method by which the move is learned.
        /// </summary>
        [JsonPropertyName("move_learn_method")]
        public required NamedApiResource<MoveLearnMethod> MoveLearnMethod { get; init; }

        /// <summary>
        /// The version group in which the move is learned.
        /// </summary>
        [JsonPropertyName("version_group")]
        public required NamedApiResource<VersionGroup> VersionGroup { get; init; }

        /// <summary>
        /// The minimum level to learn the move.
        /// </summary>
        [JsonPropertyName("level_learned_at")]
        public int LevelLearnedAt { get; init; }
    }

    /// <summary>
    /// A Pokémon stat
    /// </summary>
    public sealed record PokemonStat
    {
        /// <summary>
        /// The stat the Pokémon has.
        /// </summary>
        public required NamedApiResource<Stat> Stat { get; init; }

        /// <summary>
        /// The effort points (EV) the Pokémon has in the stat.
        /// </summary>
        public int Effort { get; init; }

        /// <summary>
        /// The base value of the stat.
        /// </summary>
        [JsonPropertyName("base_stat")]
        public int BaseStat { get; init; }
    }

    /// <summary>
    /// Pokémon sprite information
    /// </summary>
    public sealed record PokemonSprites
    {
        /// <summary>
        /// The default depiction of this Pokémon from the front in battle.
        /// </summary>
        [JsonPropertyName("front_default")]
        public required string FrontDefault { get; init; }

        /// <summary>
        /// The shiny depiction of this Pokémon from the front in battle.
        /// </summary>
        [JsonPropertyName("front_shiny")]
        public required string FrontShiny { get; init; }

        /// <summary>
        /// The female depiction of this Pokémon from the front in battle.
        /// </summary>
        [JsonPropertyName("front_female")]
        public string? FrontFemale { get; init; }

        /// <summary>
        /// The shiny female depiction of this Pokémon from the front in battle.
        /// </summary>
        [JsonPropertyName("front_shiny_female")]
        public string? FrontShinyFemale { get; init; }

        /// <summary>
        /// The default depiction of this Pokémon from the back in battle.
        /// </summary>
        [JsonPropertyName("back_default")]
        public required string BackDefault { get; init; }

        /// <summary>
        /// The shiny depiction of this Pokémon from the back in battle.
        /// </summary>
        [JsonPropertyName("back_shiny")]
        public required string BackShiny { get; init; }

        /// <summary>
        /// The female depiction of this Pokémon from the back in battle.
        /// </summary>
        [JsonPropertyName("back_female")]
        public string? BackFemale { get; init; }

        /// <summary>
        /// The shiny female depiction of this Pokémon from the back in battle.
        /// </summary>
        [JsonPropertyName("back_shiny_female")]
        public string? BackShinyFemale { get; init; }

        /// <summary>
        /// Other sprites
        /// </summary>
        public OtherSprites Other { get; init; }

        /// <summary>
        /// Pókemon sprites versioned by game generation
        /// </summary>
        public VersionSprites Versions { get; init; }

        /// <summary>
        /// Other Pokémon sprites
        /// </summary>
        public sealed record OtherSprites
        {
            /// <summary>
            /// DreamWorld sprites
            /// </summary>
            [JsonPropertyName("dream_world")]
            public DreamWorldSprites DreamWorld { get; init; }

            /// <summary>
            /// Home sprites
            /// </summary>
            public HomeSprites Home { get; init; }

            /// <summary>
            /// Official Artwork sprites
            /// </summary>
            [JsonPropertyName("official-artwork")]
            public OfficialArtworkSprites OfficialArtwork { get; init; }

            /// <summary>
            /// Showdown sprites
            /// </summary>
            [JsonPropertyName("showdown")]
            public ShowdownSprites Showdown { get; init; }

            /// <summary>
            /// DreamWorld Pókemon sprites
            /// </summary>
            public sealed record DreamWorldSprites
            {
                /// <summary>
                /// The default depiction of this Pokémon from the front in battle.
                /// </summary>
                [JsonPropertyName("front_default")]
                public required string FrontDefault { get; init; }

                /// <summary>
                /// The female depiction of this Pokémon from the front in battle.
                /// </summary>
                [JsonPropertyName("front_female")]
                public required string FrontFemale { get; init; }
            }

            /// <summary>
            /// Home Pókemon sprites
            /// </summary>
            public sealed record HomeSprites
            {
                /// <summary>
                /// The default depiction of this Pokémon from the front in battle.
                /// </summary>
                [JsonPropertyName("front_default")]
                public required string FrontDefault { get; init; }

                /// <summary>
                /// The female depiction of this Pokémon from the front in battle.
                /// </summary>
                [JsonPropertyName("front_female")]
                public required string FrontFemale { get; init; }

                /// <summary>
                /// The shiny depiction of this Pokémon from the front in battle.
                /// </summary>
                [JsonPropertyName("front_shiny")]
                public required string FrontShiny { get; init; }

                /// <summary>
                /// The shiny female depiction of this Pokémon from the front in battle.
                /// </summary>
                [JsonPropertyName("front_shiny_female")]
                public required string FrontShinyFemale { get; init; }
            }

            /// <summary>
            /// Official Artwork sprites
            /// </summary>
            public sealed record OfficialArtworkSprites
            {
                /// <summary>
                /// The default depiction of this Pokémon from the front in battle.
                /// </summary>
                [JsonPropertyName("front_default")]
                public required string FrontDefault { get; init; }

                /// <summary>
                /// The shiny depiction of this Pokémon from the front in battle.
                /// </summary>
                [JsonPropertyName("front_shiny")]
                public required string FrontShiny { get; init; }
            }

            /// <summary>
            /// Showdown sprites
            /// </summary>
            public sealed record ShowdownSprites
            {
                /// <summary>
                /// The default depiction of this Pokémon from the back in battle.
                /// </summary>
                [JsonPropertyName("back_default")]
                public required string BackDefault { get; init; }

                /// <summary>
                /// The female depiction of this Pokémon from the back in battle.
                /// </summary>
                [JsonPropertyName("back_female")]
                public required string BackFemale { get; init; }

                /// <summary>
                /// The shiny depiction of this Pokémon from the back in battle.
                /// </summary>
                [JsonPropertyName("back_shiny")]
                public required string BackShiny { get; init; }

                /// <summary>
                /// The shiny female depiction of this Pokémon from the back in battle.
                /// </summary>
                [JsonPropertyName("back_shiny_female")]
                public required string BackShinyFemale { get; init; }

                /// <summary>
                /// The default depiction of this Pokémon from the front in battle.
                /// </summary>
                [JsonPropertyName("front_default")]
                public required string FrontDefault { get; init; }

                /// <summary>
                /// The female depiction of this Pokémon from the front in battle.
                /// </summary>
                [JsonPropertyName("front_female")]
                public required string FrontFemale { get; init; }

                /// <summary>
                /// The shiny depiction of this Pokémon from the front in battle.
                /// </summary>
                [JsonPropertyName("front_shiny")]
                public required string FrontShiny { get; init; }

                /// <summary>
                /// The shiny female depiction of this Pokémon from the front in battle.
                /// </summary>
                [JsonPropertyName("front_shiny_female")]
                public required string FrontShinyFemale { get; init; }
            }
        }

        /// <summary>
        /// Pókemon sprites versioned by game generation
        /// </summary>
        public sealed record VersionSprites
        {
            /// <summary>
            /// Pókemon sprites for Generation I
            /// </summary>
            [JsonPropertyName("generation-i")]
            public GenerationISprites GenerationI { get; init; }

            /// <summary>
            /// Pókemon sprites for Generation II
            /// </summary>
            [JsonPropertyName("generation-ii")]
            public GenerationIISprites GenerationII { get; init; }

            /// <summary>
            /// Pókemon sprites for Generation III
            /// </summary>
            [JsonPropertyName("generation-iii")]
            public GenerationIIISprites GenerationIII { get; init; }

            /// <summary>
            /// Pókemon sprites for Generation IV
            /// </summary>
            [JsonPropertyName("generation-iv")]
            public GenerationIVSprites GenerationIV { get; init; }

            /// <summary>
            /// Pókemon sprites for Generation V
            /// </summary>
            [JsonPropertyName("generation-v")]
            public GenerationVSprites GenerationV { get; init; }

            /// <summary>
            /// Pókemon sprites for Generation VI
            /// </summary>
            [JsonPropertyName("generation-vi")]
            public GenerationVISprites GenerationVI { get; init; }

            /// <summary>
            /// Pókemon sprites for Generation VII
            /// </summary>
            [JsonPropertyName("generation-vii")]
            public GenerationVIISprites GenerationVII { get; init; }

            /// <summary>
            /// Pókemon sprites for Generation VIII
            /// </summary>
            [JsonPropertyName("generation-viii")]
            public GenerationVIIISprites GenerationVIII { get; init; }

            /// <summary>
            /// Pókemon sprites for Generation I
            /// </summary>
            public sealed record GenerationISprites
            {
                /// <summary>
                /// Pókemon Red-Blue sprites
                /// </summary>
                [JsonPropertyName("red-blue")]
                public RedBlueSprites RedBlue { get; init; }

                /// <summary>
                /// Pókemon Yellow sprites
                /// </summary>
                public YellowSprites Yellow { get; init; }

                /// <summary>
                /// Pókemon Red-Blue sprites
                /// </summary>
                public sealed record RedBlueSprites
                {
                    /// <summary>
                    /// The default depiction of this Pokémon from the back in battle.
                    /// </summary>
                    [JsonPropertyName("back_default")]
                    public string BackDefault { get; init; }

                    /// <summary>
                    /// The default depiction of this Pokémon from the back in battle on gray background.
                    /// </summary>
                    [JsonPropertyName("back_gray")]
                    public string BackGray { get; init; }

                    /// <summary>
                    /// The default depiction of this Pokémon from the back in battle on transparent background.
                    /// </summary>
                    [JsonPropertyName("back_transparent")]
                    public string BackTransparent { get; init; }

                    /// <summary>
                    /// The default depiction of this Pokémon from the front in battle.
                    /// </summary>
                    [JsonPropertyName("front_default")]
                    public string FrontDefault { get; init; }

                    /// <summary>
                    /// The default depiction of this Pokémon from the front in battle on gray background.
                    /// </summary>
                    [JsonPropertyName("front_gray")]
                    public string FrontGray { get; init; }

                    /// <summary>
                    /// The default depiction of this Pokémon from the front in battle on transparent background.
                    /// </summary>
                    [JsonPropertyName("front_transparent")]
                    public string FrontTransparent { get; init; }
                }

                /// <summary>
                /// Pókemon Yellow sprites
                /// </summary>
                public sealed record YellowSprites
                {
                    /// <summary>
                    /// The default depiction of this Pokémon from the back in battle.
                    /// </summary>
                    [JsonPropertyName("back_default")]
                    public string BackDefault { get; init; }

                    /// <summary>
                    /// The default depiction of this Pokémon from the back in battle on gray background.
                    /// </summary>
                    [JsonPropertyName("back_gray")]
                    public string BackGray { get; init; }

                    /// <summary>
                    /// The default depiction of this Pokémon from the back in battle on transparent background.
                    /// </summary>
                    [JsonPropertyName("back_transparent")]
                    public string BackTransparent { get; init; }

                    /// <summary>
                    /// The default depiction of this Pokémon from the front in battle.
                    /// </summary>
                    [JsonPropertyName("front_default")]
                    public string FrontDefault { get; init; }

                    /// <summary>
                    /// The default depiction of this Pokémon from the front in battle on gray background.
                    /// </summary>
                    [JsonPropertyName("front_gray")]
                    public string FrontGray { get; init; }

                    /// <summary>
                    /// The default depiction of this Pokémon from the front in battle on transparent background.
                    /// </summary>
                    [JsonPropertyName("front_transparent")]
                    public string FrontTransparent { get; init; }
                }
            }

            /// <summary>
            /// Pókemon sprites for Generation II
            /// </summary>
            public sealed record GenerationIISprites
            {
                /// <summary>
                /// Pókemon Crystal sprites
                /// </summary>
                public CrystalSprites Crystal { get; init; }

                /// <summary>
                /// Pókemon Gold sprites
                /// </summary>
                public GoldSprites Gold { get; init; }

                /// <summary>
                /// Pókemon Silver sprites
                /// </summary>
                public SilverSprites Silver { get; init; }

                /// <summary>
                /// Pókemon Crystal sprites
                /// </summary>
                public sealed record CrystalSprites
                {
                    /// <summary>
                    /// The default depiction of this Pokémon from the back in battle.
                    /// </summary>
                    [JsonPropertyName("back_default")]
                    public string BackDefault { get; init; }

                    /// <summary>
                    /// The default depiction of this Pokémon from the back shiny in battle.
                    /// </summary>
                    [JsonPropertyName("back_shiny")]
                    public string BackShiny { get; init; }

                    /// <summary>
                    /// The default depiction of this Pokémon from the back shiny in battle on transparent background.
                    /// </summary>
                    [JsonPropertyName("back_shiny_transparent")]
                    public string BackShinyTransparent { get; init; }

                    /// <summary>
                    /// The default depiction of this Pokémon from the back in battle on transparent background.
                    /// </summary>
                    [JsonPropertyName("back_transparent")]
                    public string BackTransparent { get; init; }

                    /// <summary>
                    /// The default depiction of this Pokémon from the front in battle.
                    /// </summary>
                    [JsonPropertyName("front_default")]
                    public string FrontDefault { get; init; }

                    /// <summary>
                    /// The default depiction of this Pokémon from the front shiny in battle.
                    /// </summary>
                    [JsonPropertyName("front_shiny")]
                    public string FrontShiny { get; init; }

                    /// <summary>
                    /// The default depiction of this Pokémon from the front shiny in battle on transparent background.
                    /// </summary>
                    [JsonPropertyName("front_shiny_transparent")]
                    public string FrontShinyTransparent { get; init; }

                    /// <summary>
                    /// The default depiction of this Pokémon from the front in battle on transparent background.
                    /// </summary>
                    [JsonPropertyName("front_transparent")]
                    public string FrontTransparent { get; init; }
                }

                /// <summary>
                /// Pókemon Gold sprites
                /// </summary>
                public sealed record GoldSprites
                {
                    /// <summary>
                    /// The default depiction of this Pokémon from the back in battle.
                    /// </summary>
                    [JsonPropertyName("back_default")]
                    public string BackDefault { get; init; }

                    /// <summary>
                    /// The default depiction of this Pokémon from the back shiny in battle.
                    /// </summary>
                    [JsonPropertyName("back_shiny")]
                    public string BackShiny { get; init; }

                    /// <summary>
                    /// The default depiction of this Pokémon from the front in battle.
                    /// </summary>
                    [JsonPropertyName("front_default")]
                    public string FrontDefault { get; init; }

                    /// <summary>
                    /// The default depiction of this Pokémon from the front shiny in battle.
                    /// </summary>
                    [JsonPropertyName("front_shiny")]
                    public string FrontShiny { get; init; }

                    /// <summary>
                    /// The default depiction of this Pokémon from the front in battle on transparent background.
                    /// </summary>
                    [JsonPropertyName("front_transparent")]
                    public string FrontTransparent { get; init; }
                }

                /// <summary>
                /// Pókemon Silver sprites
                /// </summary>
                public sealed record SilverSprites
                {
                    /// <summary>
                    /// The default depiction of this Pokémon from the back in battle.
                    /// </summary>
                    [JsonPropertyName("back_default")]
                    public string BackDefault { get; init; }

                    /// <summary>
                    /// The default depiction of this Pokémon from the back shiny in battle.
                    /// </summary>
                    [JsonPropertyName("back_shiny")]
                    public string BackShiny { get; init; }

                    /// <summary>
                    /// The default depiction of this Pokémon from the front in battle.
                    /// </summary>
                    [JsonPropertyName("front_default")]
                    public string FrontDefault { get; init; }

                    /// <summary>
                    /// The default depiction of this Pokémon from the front shiny in battle.
                    /// </summary>
                    [JsonPropertyName("front_shiny")]
                    public string FrontShiny { get; init; }

                    /// <summary>
                    /// The default depiction of this Pokémon from the front in battle on transparent background.
                    /// </summary>
                    [JsonPropertyName("front_transparent")]
                    public string FrontTransparent { get; init; }
                }
            }

            /// <summary>
            /// Pókemon sprites for Generation III
            /// </summary>
            public sealed record GenerationIIISprites
            {
                /// <summary>
                /// Pókemon Emerald sprites
                /// </summary>
                [JsonPropertyName("emerald")]
                public EmeraldSprites Emerald { get; init; }

                /// <summary>
                /// Pókemon Firered/Leafgreen sprites
                /// </summary>
                [JsonPropertyName("firered-leafgreen")]
                public FireredLeafgreenSprites FireredLeafgreen { get; init; }

                /// <summary>
                /// Pókemon Ruby/Sapphire sprites
                /// </summary>
                [JsonPropertyName("ruby-sapphire")]
                public RubySapphireSprites RubySapphire { get; init; }

                /// <summary>
                /// Pókemon Emerald sprites
                /// </summary>
                public sealed record EmeraldSprites
                {
                    /// <summary>
                    /// The default depiction of this Pokémon from the front in battle.
                    /// </summary>
                    [JsonPropertyName("front_default")]
                    public string FrontDefault { get; init; }

                    /// <summary>
                    /// The default depiction of this Pokémon from the front shiny in battle.
                    /// </summary>
                    [JsonPropertyName("front_shiny")]
                    public string FrontShiny { get; init; }
                }

                /// <summary>
                /// Pókemon Firered/Leafgreen sprites
                /// </summary>
                public sealed record FireredLeafgreenSprites
                {
                    /// <summary>
                    /// The default depiction of this Pokémon from the back in battle.
                    /// </summary>
                    [JsonPropertyName("back_default")]
                    public string BackDefault { get; init; }

                    /// <summary>
                    /// The default depiction of this Pokémon from the back shiny in battle.
                    /// </summary>
                    [JsonPropertyName("back_shiny")]
                    public string BackShiny { get; init; }

                    /// <summary>
                    /// The default depiction of this Pokémon from the front in battle.
                    /// </summary>
                    [JsonPropertyName("front_default")]
                    public string FrontDefault { get; init; }

                    /// <summary>
                    /// The default depiction of this Pokémon from the front shiny in battle.
                    /// </summary>
                    [JsonPropertyName("front_shiny")]
                    public string FrontShiny { get; init; }
                }

                /// <summary>
                /// Pókemon Ruby/Sapphire sprites
                /// </summary>
                public sealed record RubySapphireSprites
                {
                    /// <summary>
                    /// The default depiction of this Pokémon from the back in battle.
                    /// </summary>
                    [JsonPropertyName("back_default")]
                    public string BackDefault { get; init; }

                    /// <summary>
                    /// The default depiction of this Pokémon from the back shiny in battle.
                    /// </summary>
                    [JsonPropertyName("back_shiny")]
                    public string BackShiny { get; init; }

                    /// <summary>
                    /// The default depiction of this Pokémon from the front in battle.
                    /// </summary>
                    [JsonPropertyName("front_default")]
                    public string FrontDefault { get; init; }

                    /// <summary>
                    /// The default depiction of this Pokémon from the front shiny in battle.
                    /// </summary>
                    [JsonPropertyName("front_shiny")]
                    public string FrontShiny { get; init; }
                }
            }

            /// <summary>
            /// Pókemon sprites for Generation IV
            /// </summary>
            public sealed record GenerationIVSprites
            {
                /// <summary>
                /// Pókemon Diamond/Pearl sprites
                /// </summary>
                [JsonPropertyName("diamond-pearl")]
                public DiamondPearlSprites DiamondPearl { get; init; }

                /// <summary>
                /// Pókemon Heartgold/Soulsilver sprites
                /// </summary>
                [JsonPropertyName("heartgold-soulsilver")]
                public HeartGoldSoulSilverSprites HeartGoldSoulSilver { get; init; }

                /// <summary>
                /// Pókemon Platinum sprites
                /// </summary>
                public PlatinumSprites Platinum { get; init; }

                /// <summary>
                /// Pókemon Diamond/Pearl sprites
                /// </summary>
                public sealed record DiamondPearlSprites
                {
                    /// <summary>
                    /// The default depiction of this Pokémon from the back in battle.
                    /// </summary>
                    [JsonPropertyName("back_default")]
                    public string BackDefault { get; init; }

                    /// <summary>
                    /// The default depiction of this Pokémon from the back female in battle.
                    /// </summary>
                    [JsonPropertyName("back_female")]
                    public string BackFemale { get; init; }

                    /// <summary>
                    /// The default depiction of this Pokémon from the back shiny in battle.
                    /// </summary>
                    [JsonPropertyName("back_shiny")]
                    public string BackShiny { get; init; }

                    /// <summary>
                    /// The default depiction of this Pokémon from the back female shiny in battle.
                    /// </summary>
                    [JsonPropertyName("back_shiny_female")]
                    public string BackShinyFemale { get; init; }

                    /// <summary>
                    /// The default depiction of this Pokémon from the front in battle.
                    /// </summary>
                    [JsonPropertyName("front_default")]
                    public string FrontDefault { get; init; }

                    /// <summary>
                    /// The default depiction of this Pokémon from the front female in battle.
                    /// </summary>
                    [JsonPropertyName("front_female")]
                    public string FrontFemale { get; init; }

                    /// <summary>
                    /// The default depiction of this Pokémon from the front shiny in battle.
                    /// </summary>
                    [JsonPropertyName("front_shiny")]
                    public string FrontShiny { get; init; }

                    // <summary>
                    /// The default depiction of this Pokémon from the front female shiny in battle.
                    /// </summary>
                    [JsonPropertyName("front_shiny_female")]
                    public string FrontShinyFemale { get; init; }
                }

                /// <summary>
                /// Pókemon Heartgold/Soulsilver sprites
                /// </summary>
                public sealed record HeartGoldSoulSilverSprites
                {
                    /// <summary>
                    /// The default depiction of this Pokémon from the back in battle.
                    /// </summary>
                    [JsonPropertyName("back_default")]
                    public string BackDefault { get; init; }

                    /// <summary>
                    /// The default depiction of this Pokémon from the back female in battle.
                    /// </summary>
                    [JsonPropertyName("back_female")]
                    public string BackFemale { get; init; }

                    /// <summary>
                    /// The default depiction of this Pokémon from the back shiny in battle.
                    /// </summary>
                    [JsonPropertyName("back_shiny")]
                    public string BackShiny { get; init; }

                    /// <summary>
                    /// The default depiction of this Pokémon from the back female shiny in battle.
                    /// </summary>
                    [JsonPropertyName("back_shiny_female")]
                    public string BackShinyFemale { get; init; }

                    /// <summary>
                    /// The default depiction of this Pokémon from the front in battle.
                    /// </summary>
                    [JsonPropertyName("front_default")]
                    public string FrontDefault { get; init; }

                    /// <summary>
                    /// The default depiction of this Pokémon from the front female in battle.
                    /// </summary>
                    [JsonPropertyName("front_female")]
                    public string FrontFemale { get; init; }

                    /// <summary>
                    /// The default depiction of this Pokémon from the front shiny in battle.
                    /// </summary>
                    [JsonPropertyName("front_shiny")]
                    public string FrontShiny { get; init; }

                    // <summary>
                    /// The default depiction of this Pokémon from the front female shiny in battle.
                    /// </summary>
                    [JsonPropertyName("front_shiny_female")]
                    public string FrontShinyFemale { get; init; }
                }

                /// <summary>
                /// Pókemon Platinum sprites
                /// </summary>
                public sealed record PlatinumSprites
                {
                    /// <summary>
                    /// The default depiction of this Pokémon from the back in battle.
                    /// </summary>
                    [JsonPropertyName("back_default")]
                    public string BackDefault { get; init; }

                    /// <summary>
                    /// The default depiction of this Pokémon from the back female in battle.
                    /// </summary>
                    [JsonPropertyName("back_female")]
                    public string BackFemale { get; init; }

                    /// <summary>
                    /// The default depiction of this Pokémon from the back shiny in battle.
                    /// </summary>
                    [JsonPropertyName("back_shiny")]
                    public string BackShiny { get; init; }

                    /// <summary>
                    /// The default depiction of this Pokémon from the back female shiny in battle.
                    /// </summary>
                    [JsonPropertyName("back_shiny_female")]
                    public string BackShinyFemale { get; init; }

                    /// <summary>
                    /// The default depiction of this Pokémon from the front in battle.
                    /// </summary>
                    [JsonPropertyName("front_default")]
                    public string FrontDefault { get; init; }

                    /// <summary>
                    /// The default depiction of this Pokémon from the front female in battle.
                    /// </summary>
                    [JsonPropertyName("front_female")]
                    public string FrontFemale { get; init; }

                    /// <summary>
                    /// The default depiction of this Pokémon from the front shiny in battle.
                    /// </summary>
                    [JsonPropertyName("front_shiny")]
                    public string FrontShiny { get; init; }

                    // <summary>
                    /// The default depiction of this Pokémon from the front female shiny in battle.
                    /// </summary>
                    [JsonPropertyName("front_shiny_female")]
                    public string FrontShinyFemale { get; init; }
                }
            }

            /// <summary>
            /// Pókemon sprites for Generation V
            /// </summary>
            public sealed record GenerationVSprites
            {
                /// <summary>
                /// Pókemon Black/White sprites
                /// </summary>
                [JsonPropertyName("black-white")]
                public BlackWhiteSprites BlackWhite { get; init; }

                /// <summary>
                /// Pókemon Black/White sprites
                /// </summary>
                public sealed record BlackWhiteSprites
                {
                    /// <summary>
                    /// The animated depiction of this Pokémon from the back in battle.
                    /// </summary>
                    public AnimatedSprites Animated { get; init; }

                    /// <summary>
                    /// The default depiction of this Pokémon from the back in battle.
                    /// </summary>
                    [JsonPropertyName("back_default")]
                    public string BackDefault { get; init; }

                    /// <summary>
                    /// The default depiction of this Pokémon from the back female in battle.
                    /// </summary>
                    [JsonPropertyName("back_female")]
                    public string BackFemale { get; init; }

                    /// <summary>
                    /// The default depiction of this Pokémon from the back shiny in battle.
                    /// </summary>
                    [JsonPropertyName("back_shiny")]
                    public string BackShiny { get; init; }

                    /// <summary>
                    /// The default depiction of this Pokémon from the back female shiny in battle.
                    /// </summary>
                    [JsonPropertyName("back_shiny_female")]
                    public string BackShinyFemale { get; init; }

                    /// <summary>
                    /// The default depiction of this Pokémon from the front in battle.
                    /// </summary>
                    [JsonPropertyName("front_default")]
                    public string FrontDefault { get; init; }

                    /// <summary>
                    /// The default depiction of this Pokémon from the front female in battle.
                    /// </summary>
                    [JsonPropertyName("front_female")]
                    public string FrontFemale { get; init; }

                    /// <summary>
                    /// The default depiction of this Pokémon from the front shiny in battle.
                    /// </summary>
                    [JsonPropertyName("front_shiny")]
                    public string FrontShiny { get; init; }

                    // <summary>
                    /// The default depiction of this Pokémon from the front female shiny in battle.
                    /// </summary>
                    [JsonPropertyName("front_shiny_female")]
                    public string FrontShinyFemale { get; init; }

                    /// <summary>
                    /// The animated depiction of this Pokémon from the back in battle.
                    /// </summary>
                    public sealed record AnimatedSprites
                    {
                        /// <summary>
                        /// The default depiction of this Pokémon from the back in battle.
                        /// </summary>
                        [JsonPropertyName("back_default")]
                        public string BackDefault { get; init; }

                        /// <summary>
                        /// The default depiction of this Pokémon from the back female in battle.
                        /// </summary>
                        [JsonPropertyName("back_female")]
                        public string BackFemale { get; init; }

                        /// <summary>
                        /// The default depiction of this Pokémon from the back shiny in battle.
                        /// </summary>
                        [JsonPropertyName("back_shiny")]
                        public string BackShiny { get; init; }

                        /// <summary>
                        /// The default depiction of this Pokémon from the back female shiny in battle.
                        /// </summary>
                        [JsonPropertyName("back_shiny_female")]
                        public string BackShinyFemale { get; init; }

                        /// <summary>
                        /// The default depiction of this Pokémon from the front in battle.
                        /// </summary>
                        [JsonPropertyName("front_default")]
                        public string FrontDefault { get; init; }

                        /// <summary>
                        /// The default depiction of this Pokémon from the front female in battle.
                        /// </summary>
                        [JsonPropertyName("front_female")]
                        public string FrontFemale { get; init; }

                        /// <summary>
                        /// The default depiction of this Pokémon from the front shiny in battle.
                        /// </summary>
                        [JsonPropertyName("front_shiny")]
                        public string FrontShiny { get; init; }

                        // <summary>
                        /// The default depiction of this Pokémon from the front female shiny in battle.
                        /// </summary>
                        [JsonPropertyName("front_shiny_female")]
                        public string FrontShinyFemale { get; init; }
                    }
                }
            }

            /// <summary>
            /// Pókemon sprites for Generation VI
            /// </summary>
            public sealed record GenerationVISprites
            {
                /// <summary>
                /// Pókemon OmegaRuby/AlphaSapphire sprites
                /// </summary>
                [JsonPropertyName("omegaruby-alphasapphire")]
                public OmegaRubyAlphaSapphireSprites OmegaRubyAlphaSapphire { get; init; }

                /// <summary>
                /// Pókemon X/Y sprites
                /// </summary>
                [JsonPropertyName("x-y")]
                public XYSprites XY { get; init; }

                /// <summary>
                /// Pókemon OmegaRuby/AlphaSapphire sprites
                /// </summary>
                public sealed record OmegaRubyAlphaSapphireSprites
                {
                    /// <summary>
                    /// The default depiction of this Pokémon from the front in battle.
                    /// </summary>
                    [JsonPropertyName("front_default")]
                    public string FrontDefault { get; init; }

                    /// <summary>
                    /// The default depiction of this Pokémon from the front female in battle.
                    /// </summary>
                    [JsonPropertyName("front_female")]
                    public string FrontFemale { get; init; }

                    /// <summary>
                    /// The default depiction of this Pokémon from the front shiny in battle.
                    /// </summary>
                    [JsonPropertyName("front_shiny")]
                    public string FrontShiny { get; init; }

                    // <summary>
                    /// The default depiction of this Pokémon from the front female shiny in battle.
                    /// </summary>
                    [JsonPropertyName("front_shiny_female")]
                    public string FrontShinyFemale { get; init; }
                }

                /// <summary>
                /// Pókemon X/Y sprites
                /// </summary>
                public sealed record XYSprites
                {
                    /// <summary>
                    /// The default depiction of this Pokémon from the front in battle.
                    /// </summary>
                    [JsonPropertyName("front_default")]
                    public string FrontDefault { get; init; }

                    /// <summary>
                    /// The default depiction of this Pokémon from the front female in battle.
                    /// </summary>
                    [JsonPropertyName("front_female")]
                    public string FrontFemale { get; init; }

                    /// <summary>
                    /// The default depiction of this Pokémon from the front shiny in battle.
                    /// </summary>
                    [JsonPropertyName("front_shiny")]
                    public string FrontShiny { get; init; }

                    // <summary>
                    /// The default depiction of this Pokémon from the front female shiny in battle.
                    /// </summary>
                    [JsonPropertyName("front_shiny_female")]
                    public string FrontShinyFemale { get; init; }
                }
            }

            /// <summary>
            /// Pókemon sprites for Generation VII
            /// </summary>
            public sealed record GenerationVIISprites
            {
                /// <summary>
                /// Pókemon Icons sprites
                /// </summary>
                public IconsSprites Icons { get; init; }

                /// <summary>
                /// Pókemon UltraSun/UltraMoon sprites
                /// </summary>
                [JsonPropertyName("ultra-sun-ultra-moon")]
                public UltraSunUltraMoonSprites UltraSunUltraMoon { get; init; }

                /// <summary>
                /// Pókemon Icons sprites
                /// </summary>
                public sealed record IconsSprites
                {
                    /// <summary>
                    /// The default depiction of this Pokémon from the front in battle.
                    /// </summary>
                    [JsonPropertyName("front_default")]
                    public string FrontDefault { get; init; }

                    /// <summary>
                    /// The default depiction of this Pokémon from the front female in battle.
                    /// </summary>
                    [JsonPropertyName("front_female")]
                    public string FrontFemale { get; init; }
                }

                /// <summary>
                /// Pókemon UltraSun/UltraMoon sprites
                /// </summary>
                public sealed record UltraSunUltraMoonSprites
                {
                    /// <summary>
                    /// The default depiction of this Pokémon from the front in battle.
                    /// </summary>
                    [JsonPropertyName("front_default")]
                    public string FrontDefault { get; init; }

                    /// <summary>
                    /// The default depiction of this Pokémon from the front female in battle.
                    /// </summary>
                    [JsonPropertyName("front_female")]
                    public string FrontFemale { get; init; }

                    /// <summary>
                    /// The default depiction of this Pokémon from the front shiny in battle.
                    /// </summary>
                    [JsonPropertyName("front_shiny")]
                    public string FrontShiny { get; init; }

                    // <summary>
                    /// The default depiction of this Pokémon from the front female shiny in battle.
                    /// </summary>
                    [JsonPropertyName("front_shiny_female")]
                    public string FrontShinyFemale { get; init; }
                }
            }

            /// <summary>
            /// Pókemon sprites for Generation VIII
            /// </summary>
            public sealed record GenerationVIIISprites
            {
                /// <summary>
                /// Pókemon Icons sprites
                /// </summary>
                public IconsSprites Icons { get; init; }

                /// <summary>
                /// Pókemon Icons sprites
                /// </summary>
                public sealed record IconsSprites
                {
                    /// <summary>
                    /// The default depiction of this Pokémon from the front in battle.
                    /// </summary>
                    [JsonPropertyName("front_default")]
                    public string FrontDefault { get; init; }

                    /// <summary>
                    /// The default depiction of this Pokémon from the front female in battle.
                    /// </summary>
                    [JsonPropertyName("front_female")]
                    public string FrontFemale { get; init; }
                }
            }
        }
    }

    /// <summary>
    /// A list of possible encounter locations for a Pokémon with the version information
    /// </summary>
    public sealed record LocationAreaEncounter
    {
        /// <summary>
        /// The location area the referenced Pokémon can be encountered in.
        /// </summary>
        [JsonPropertyName("location_area")]
        public NamedApiResource<LocationArea> LocationArea { get; init; }

        /// <summary>
        /// A list of versions and encounters with the referenced Pokémon
        /// that might happen.
        /// </summary>
        [JsonPropertyName("version_details")]
        public List<VersionEncounterDetail> VersionDetails { get; init; }
    }

    /// <summary>
    /// Colors used for sorting Pokémon in a Pokédex. The color listed in the
    /// Pokédex is usually the color most apparent or covering each Pokémon's
    /// body. No orange category exists; Pokémon that are primarily orange are
    /// listed as red or brown.
    /// </summary>
    public sealed record PokemonColor : NamedApiResource
    {
        /// <summary>
        /// The identifier for this resource.
        /// </summary>
        public override int Id { get; init; }

        internal static new string ApiEndpoint { get; } = "pokemon-color";

        /// <summary>
        /// The name for this resource.
        /// </summary>
        public override string Name { get; init; }

        /// <summary>
        /// The name of this resource listed in different languages.
        /// </summary>
        public List<Names> Names { get; init; }

        /// <summary>
        /// A list of the Pokémon species that have this color.
        /// </summary>
        [JsonPropertyName("pokemon_species")]
        public List<NamedApiResource<PokemonSpecies>> PokemonSpecies { get; init; }
    }

    /// <summary>
    /// Some Pokémon may appear in one of multiple, visually different
    /// forms. These differences are purely cosmetic. For variations
    /// within a Pokémon species, which do differ in more than just visuals,
    /// the 'Pokémon' entity is used to represent such a variety.
    /// </summary>
    public sealed record PokemonForm : NamedApiResource
    {
        /// <summary>
        /// The identifier for this resource.
        /// </summary>
        public override int Id { get; init; }

        internal static new string ApiEndpoint { get; } = "pokemon-form";

        /// <summary>
        /// The name for this resource.
        /// </summary>
        public override string Name { get; init; }

        /// <summary>
        /// The order in which forms should be sorted within all forms.
        /// Multiple forms may have equal order, in which case they should
        /// fall back on sorting by name.
        /// </summary>
        public int Order { get; init; }

        /// <summary>
        /// The order in which forms should be sorted within a species' forms.
        /// </summary>
        [JsonPropertyName("form_order")]
        public int FormOrder { get; init; }

        /// <summary>
        /// True for exactly one form used as the default for each Pokémon.
        /// </summary>
        [JsonPropertyName("is_default")]
        public bool IsDefault { get; init; }

        /// <summary>
        /// Whether or not this form can only happen during battle.
        /// </summary>
        [JsonPropertyName("is_battle_only")]
        public bool IsBattleOnly { get; init; }

        /// <summary>
        /// Whether or not this form requires mega evolution.
        /// </summary>
        [JsonPropertyName("is_mega")]
        public bool IsMega { get; init; }

        /// <summary>
        /// The name of this form.
        /// </summary>
        [JsonPropertyName("form_name")]
        public string FormName { get; init; }

        /// <summary>
        /// The Pokémon that can take on this form.
        /// </summary>
        public NamedApiResource<Pokemon> Pokemon { get; init; }

        /// <summary>
        /// A init of sprites used to depict this Pokémon form in the game.
        /// </summary>
        public PokemonFormSprites Sprites { get; init; }

        /// <summary>
        /// List of types belonging to this Pokémon form.
        /// </summary>
        public List<PokemonType> Types { get; init; }

        /// <summary>
        /// The version group this Pokémon form was introduced in.
        /// </summary>
        [JsonPropertyName("version_group")]
        public NamedApiResource<VersionGroup> VersionGroup { get; init; }

        /// <summary>
        /// The form specific full name of this Pokémon form, or empty if
        /// the form does not have a specific name.
        /// </summary>
        public List<Names> Names { get; init; }

        /// <summary>
        /// The form specific form name of this Pokémon form, or empty if the
        /// form does not have a specific name.
        /// </summary>
        [JsonPropertyName("form_names")]
        public List<Names> FormNames { get; init; }
    }

    /// <summary>
    /// Pokémon sprite information with relation to a form
    /// </summary>
    public sealed record PokemonFormSprites
    {
        /// <summary>
        /// The default depiction of this Pokémon form from the front in battle.
        /// </summary>
        [JsonPropertyName("front_default")]
        public string FrontDefault { get; init; }

        /// <summary>
        /// The shiny depiction of this Pokémon form from the front in battle.
        /// </summary>
        [JsonPropertyName("front_shiny")]
        public string FrontShiny { get; init; }

        /// <summary>
        /// The default depiction of this Pokémon form from the back in battle.
        /// </summary>
        [JsonPropertyName("back_default")]
        public string BackDefault { get; init; }

        /// <summary>
        /// The shiny depiction of this Pokémon form from the back in battle.
        /// </summary>
        [JsonPropertyName("back_shiny")]
        public string BackShiny { get; init; }
    }

    /// <summary>
    /// Habitats are generally different terrain Pokémon can be found in but
    /// can also be areas designated for rare or legendary Pokémon.
    /// </summary>
    public sealed record PokemonHabitat : NamedApiResource
    {
        /// <summary>
        /// The identifier for this resource.
        /// </summary>
        public override int Id { get; init; }

        internal static new string ApiEndpoint { get; } = "pokemon-habitat";

        /// <summary>
        /// The name for this resource.
        /// </summary>
        public override string Name { get; init; }

        /// <summary>
        /// The name of this resource listed in different languages.
        /// </summary>
        public List<Names> Names { get; init; }

        /// <summary>
        /// A list of the Pokémon species that can be found in this habitat.
        /// </summary>
        [JsonPropertyName("pokemon_species")]
        public List<NamedApiResource<PokemonSpecies>> PokemonSpecies { get; init; }
    }

    /// <summary>
    /// Shapes used for sorting Pokémon in a Pokédex.
    /// </summary>
    public sealed record PokemonShape : NamedApiResource
    {
        /// <summary>
        /// The identifier for this resource.
        /// </summary>
        public override int Id { get; init; }

        internal static new string ApiEndpoint { get; } = "pokemon-shape";

        /// <summary>
        /// The name for this resource.
        /// </summary>
        public override string Name { get; init; }

        /// <summary>
        /// The "scientific" name of this Pokémon shape listed in
        /// different languages.
        /// </summary>
        [JsonPropertyName("awesome_names")]
        public List<AwesomeNames> AwesomeNames { get; init; }

        /// <summary>
        /// The name of this resource listed in different languages.
        /// </summary>
        public List<Names> Names { get; init; }

        /// <summary>
        /// A list of the Pokémon species that have this shape.
        /// </summary>
        [JsonPropertyName("pokemon_species")]
        public List<NamedApiResource<PokemonSpecies>> PokemonSpecies { get; init; }
    }

    /// <summary>
    /// The "scientific" name for an API resource and the language information
    /// </summary>
    public sealed record AwesomeNames
    {
        /// <summary>
        /// The localized "scientific" name for an API resource in a
        /// specific language.
        /// </summary>
        [JsonPropertyName("awesome_name")]
        public string AwesomeName { get; init; }

        /// <summary>
        /// The language this "scientific" name is in.
        /// </summary>
        public NamedApiResource<Language> Language { get; init; }
    }

    /// <summary>
    /// A Pokémon Species forms the basis for at least one Pokémon. Attributes
    /// of a Pokémon species are shared across all varieties of Pokémon within
    /// the species. A good example is Wormadam; Wormadam is the species which
    /// can be found in three different varieties, Wormadam-Trash,
    /// Wormadam-Sandy and Wormadam-Plant.
    /// </summary>
    public sealed record PokemonSpecies : NamedApiResource
    {
        /// <summary>
        /// The identifier for this resource.
        /// </summary>
        public override int Id { get; init; }

        internal static new string ApiEndpoint { get; } = "pokemon-species";

        /// <summary>
        /// The name for this resource.
        /// </summary>
        public override string Name { get; init; }

        /// <summary>
        /// The order in which species should be sorted. Based on National Dex
        /// order, except families are grouped together and sorted by stage.
        /// </summary>
        public int Order { get; init; }

        /// <summary>
        /// The chance of this Pokémon being female, in eighths; or -1 for
        /// genderless.
        /// </summary>
        [JsonPropertyName("gender_rate")]
        public int GenderRate { get; init; }

        /// <summary>
        /// The base capture rate; up to 255. The higher the number, the easier
        /// the catch.
        /// </summary>
        [JsonPropertyName("capture_rate")]
        public int? CaptureRate { get; init; }

        /// <summary>
        /// The happiness when caught by a normal Pokéball; up to 255. The higher
        /// the number, the happier the Pokémon.
        /// </summary>
        [JsonPropertyName("base_happiness")]
        public int? BaseHappiness { get; init; }

        /// <summary>
        /// Whether or not this is a baby Pokémon.
        /// </summary>
        [JsonPropertyName("is_baby")]
        public bool IsBaby { get; init; }

        /// <summary>
        /// Whether or not this is a legendary Pokémon.
        /// </summary>
        [JsonPropertyName("is_legendary")]
        public bool IsLegendary { get; init; }

        /// <summary>
        /// Whether or not this is a mythical Pokémon.
        /// </summary>
        [JsonPropertyName("is_mythical")]
        public bool IsMythical { get; init; }

        /// <summary>
        /// Initial hatch counter: one must walk 255 × (hatch_counter + 1) steps
        /// before this Pokémon's egg hatches, unless utilizing bonuses like
        /// Flame Body's.
        /// </summary>
        [JsonPropertyName("hatch_counter")]
        public int? HatchCounter { get; init; }

        /// <summary>
        /// Whether or not this Pokémon has visual gender differences.
        /// </summary>
        [JsonPropertyName("has_gender_differences")]
        public bool HasGenderDifferences { get; init; }

        /// <summary>
        /// Whether or not this Pokémon has multiple forms and can switch between
        /// them.
        /// </summary>
        [JsonPropertyName("forms_switchable")]
        public bool FormsSwitchable { get; init; }

        /// <summary>
        /// The rate at which this Pokémon species gains levels.
        /// </summary>
        [JsonPropertyName("growth_rate")]
        public NamedApiResource<GrowthRate> GrowthRate { get; init; }

        /// <summary>
        /// A list of Pokedexes and the indexes reserved within them for this
        /// Pokémon species.
        /// </summary>
        [JsonPropertyName("pokedex_numbers")]
        public List<PokemonSpeciesDexEntry> PokedexNumbers { get; init; }

        /// <summary>
        /// A list of egg groups this Pokémon species is a member of.
        /// </summary>
        [JsonPropertyName("egg_groups")]
        public List<NamedApiResource<EggGroup>> EggGroups { get; init; }

        /// <summary>
        /// The color of this Pokémon for Pokédex search.
        /// </summary>
        public NamedApiResource<PokemonColor> Color { get; init; }

        /// <summary>
        /// The shape of this Pokémon for Pokédex search.
        /// </summary>
        public NamedApiResource<PokemonShape> Shape { get; init; }

        /// <summary>
        /// The Pokémon species that evolves into this Pokemon_species.
        /// </summary>
        [JsonPropertyName("evolves_from_species")]
        public NamedApiResource<PokemonSpecies> EvolvesFromSpecies { get; init; }

        /// <summary>
        /// The evolution chain this Pokémon species is a member of.
        /// </summary>
        [JsonPropertyName("evolution_chain")]
        public ApiResource<EvolutionChain> EvolutionChain { get; init; }

        /// <summary>
        /// The habitat this Pokémon species can be encountered in.
        /// </summary>
        public NamedApiResource<PokemonHabitat> Habitat { get; init; }

        /// <summary>
        /// The generation this Pokémon species was introduced in.
        /// </summary>
        public NamedApiResource<Generation> Generation { get; init; }

        /// <summary>
        /// The name of this resource listed in different languages.
        /// </summary>
        public List<Names> Names { get; init; }

        /// <summary>
        /// A list of encounters that can be had with this Pokémon species in
        /// pal park.
        /// </summary>
        [JsonPropertyName("pal_park_encounters")]
        public List<PalParkEncounterArea> PalParkEncounters { get; init; }

        /// <summary>
        /// A list of flavor text entries for this Pokémon species.
        /// </summary>
        [JsonPropertyName("flavor_text_entries")]
        public List<PokemonSpeciesFlavorTexts> FlavorTextEntries { get; init; }

        /// <summary>
        /// Descriptions of different forms Pokémon take on within the Pokémon
        /// species.
        /// </summary>
        [JsonPropertyName("form_descriptions")]
        public List<Descriptions> FormDescriptions { get; init; }

        /// <summary>
        /// The genus of this Pokémon species listed in multiple languages.
        /// </summary>
        public List<Genuses> Genera { get; init; }

        /// <summary>
        /// A list of the Pokémon that exist within this Pokémon species.
        /// </summary>
        public List<PokemonSpeciesVariety> Varieties { get; init; }
    }

    /// <summary>
    /// The flavor text for a Pokémon species
    /// </summary>
    public sealed record PokemonSpeciesFlavorTexts
    {
        /// <summary>
        /// The localized flavor text for an api resource in a specific language
        /// </summary>
        [JsonPropertyName("flavor_text")]
        public string FlavorText { get; init; }

        /// <summary>
        /// The game version this flavor text is extracted from.
        /// </summary>
        public NamedApiResource<Version> Version { get; init; }

        /// <summary>
        /// The language this flavor text is in.
        /// </summary>
        public NamedApiResource<Language> Language { get; init; }
    }

    /// <summary>
    /// The genus information for a Pokémon and the associated language
    /// </summary>
    public sealed record Genuses
    {
        /// <summary>
        /// The localized genus for the referenced Pokémon species
        /// </summary>
        public string Genus { get; init; }

        /// <summary>
        /// The language this genus is in.
        /// </summary>
        public NamedApiResource<Language> Language { get; init; }
    }

    /// <summary>
    /// The Pokémon Pokédex entry information
    /// </summary>
    public sealed record PokemonSpeciesDexEntry
    {
        /// <summary>
        /// The index number within the Pokédex.
        /// </summary>
        [JsonPropertyName("entry_number")]
        public int EntryNumber { get; init; }

        /// <summary>
        /// The Pokédex the referenced Pokémon species can be found in.
        /// </summary>
        public NamedApiResource<Pokedex> Pokedex { get; init; }
    }

    /// <summary>
    /// Information for a PalPark area
    /// </summary>
    public sealed record PalParkEncounterArea
    {
        /// <summary>
        /// The base score given to the player when the referenced Pokémon is
        /// caught during a pal park run.
        /// </summary>
        [JsonPropertyName("base_score")]
        public int BaseScore { get; init; }

        /// <summary>
        /// The base rate for encountering the referenced Pokémon in this pal
        /// park area.
        /// </summary>
        public int Rate { get; init; }

        /// <summary>
        /// The pal park area where this encounter happens.
        /// </summary>
        public NamedApiResource<PalParkArea> Area { get; init; }
    }

    /// <summary>
    /// A variety of a Pokémon species
    /// </summary>
    public sealed record PokemonSpeciesVariety
    {
        /// <summary>
        /// Whether this variety is the default variety.
        /// </summary>
        [JsonPropertyName("is_default")]
        public bool IsDefault { get; init; }

        /// <summary>
        /// The Pokémon variety.
        /// </summary>
        public NamedApiResource<Pokemon> Pokemon { get; init; }
    }

    /// <summary>
    /// Stats determine certain aspects of battles. Each Pokémon has a value
    /// for each stat which grows as they gain levels and can be altered
    /// momentarily by effects in battles.
    /// </summary>
    public sealed record Stat : NamedApiResource
    {
        /// <summary>
        /// The identifier for this resource.
        /// </summary>
        public override int Id { get; init; }

        internal static new string ApiEndpoint { get; } = "stat";

        /// <summary>
        /// The name for this resource.
        /// </summary>
        public override string Name { get; init; }

        /// <summary>
        /// ID the games use for this stat.
        /// </summary>
        [JsonPropertyName("game_index")]
        public int GameIndex { get; init; }

        /// <summary>
        /// Whether this stat only exists within a battle.
        /// </summary>
        [JsonPropertyName("is_battle_only")]
        public bool IsBattleOnly { get; init; }

        /// <summary>
        /// A detail of moves which affect this stat positively or negatively.
        /// </summary>
        [JsonPropertyName("affecting_moves")]
        public MoveStatAffectSets AffectingMoves { get; init; }

        /// <summary>
        /// A detail of natures which affect this stat positively or negatively.
        /// </summary>
        [JsonPropertyName("affecting_natures")]
        public NatureStatAffectSets AffectingNatures { get; init; }

        /// <summary>
        /// A list of characteristics that are init on a Pokémon when its highest
        /// base stat is this stat.
        /// </summary>
        public List<ApiResource<Characteristic>> Characteristics { get; init; }

        /// <summary>
        /// The public sealed record of damage this stat is directly related to.
        /// </summary>
        [JsonPropertyName("move_damage_class")]
        public NamedApiResource<MoveDamageClass> MoveDamageClass { get; init; }

        /// <summary>
        /// The name of this resource listed in different languages.
        /// </summary>
        public List<Names> Names { get; init; }
    }

    /// <summary>
    /// A list of moves and how they change statuses
    /// </summary>
    public sealed record MoveStatAffectSets
    {
        /// <summary>
        /// A list of moves and how they change the referenced stat.
        /// </summary>
        public List<MoveStatAffect> Increase { get; init; }

        /// <summary>
        /// A list of moves and how they change the referenced stat.
        /// </summary>
        public List<MoveStatAffect> Decrease { get; init; }
    }

    /// <summary>
    /// A reference to a move and the change to a status
    /// </summary>
    public sealed record MoveStatAffect
    {
        /// <summary>
        /// The maximum amount of change to the referenced stat.
        /// </summary>
        public int Change { get; init; }

        /// <summary>
        /// The move causing the change.
        /// </summary>
        public NamedApiResource<Move> Move { get; init; }
    }

    /// <summary>
    /// A reference to a nature and the change to a status
    /// </summary>
    public sealed record NatureStatAffectSets
    {
        /// <summary>
        /// A list of natures and how they change the referenced stat.
        /// </summary>
        public List<NamedApiResource<Nature>> Increase { get; init; }

        /// <summary>
        /// A list of natures and how they change the referenced stat.
        /// </summary>
        public List<NamedApiResource<Nature>> Decrease { get; init; }
    }

    /// <summary>
    /// Types are properties for Pokémon and their moves. Each type has three
    /// properties: which types of Pokémon it is super effective against,
    /// which types of Pokémon it is not very effective against, and which types
    /// of Pokémon it is completely ineffective against.
    /// </summary>
    public sealed record Type : NamedApiResource
    {
        /// <summary>
        /// The identifier for this resource.
        /// </summary>
        public override int Id { get; init; }

        internal static new string ApiEndpoint { get; } = "type";

        /// <summary>
        /// The name for this resource.
        /// </summary>
        public override string Name { get; init; }

        /// <summary>
        /// A detail of how effective this type is toward others and vice versa.
        /// </summary>
        [JsonPropertyName("damage_relations")]
        public TypeRelations DamageRelations { get; init; }

        /// <summary>
        /// A list of game indices relevent to this item by generation.
        /// </summary>
        [JsonPropertyName("game_indices")]
        public List<GenerationGameIndex> GameIndices { get; init; }

        /// <summary>
        /// The generation this type was introduced in.
        /// </summary>
        public NamedApiResource<Generation> Generation { get; init; }

        /// <summary>
        /// The public sealed record of damage inflicted by this type.
        /// </summary>
        [JsonPropertyName("move_damage_class")]
        public NamedApiResource<MoveDamageClass> MoveDamageClass { get; init; }

        /// <summary>
        /// The name of this resource listed in different languages.
        /// </summary>
        public List<Names> Names { get; init; }

        /// <summary>
        /// A list of details of Pokémon that have this type.
        /// </summary>
        public List<TypePokemon> Pokemon { get; init; }

        /// <summary>
        /// A list of moves that have this type.
        /// </summary>
        public List<NamedApiResource<Move>> Moves { get; init; }
    }

    /// <summary>
    /// A Pokémon type information
    /// </summary>
    public sealed record TypePokemon
    {
        /// <summary>
        /// The order the Pokémon's types are listed in.
        /// </summary>
        public int Slot { get; init; }

        /// <summary>
        /// The Pokémon that has the referenced type.
        /// </summary>
        public NamedApiResource<Pokemon> Pokemon { get; init; }
    }

    /// <summary>
    /// The information for how a type interacts with other types
    /// </summary>
    public sealed record TypeRelations
    {
        /// <summary>
        /// A list of types this type has no effect on.
        /// </summary>
        [JsonPropertyName("no_damage_to")]
        public List<NamedApiResource<Type>> NoDamageTo { get; init; }

        /// <summary>
        /// A list of types this type is not very effect against.
        /// </summary>
        [JsonPropertyName("half_damage_to")]
        public List<NamedApiResource<Type>> HalfDamageTo { get; init; }

        /// <summary>
        /// A list of types this type is very effect against.
        /// </summary>
        [JsonPropertyName("double_damage_to")]
        public List<NamedApiResource<Type>> DoubleDamageTo { get; init; }

        /// <summary>
        /// A list of types that have no effect on this type.
        /// </summary>
        [JsonPropertyName("no_damage_from")]
        public List<NamedApiResource<Type>> NoDamageFrom { get; init; }

        /// <summary>
        /// A list of types that are not very effective against this type.
        /// </summary>
        [JsonPropertyName("half_damage_from")]
        public List<NamedApiResource<Type>> HalfDamageFrom { get; init; }

        /// <summary>
        /// A list of types that are very effective against this type.
        /// </summary>
        [JsonPropertyName("double_damage_from")]
        public List<NamedApiResource<Type>> DoubleDamageFrom { get; init; }
    }
}
