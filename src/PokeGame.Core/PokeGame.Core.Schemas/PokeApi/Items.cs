
using System.Text.Json.Serialization;

namespace PokeGame.Core.Schemas.PokeApi
{
    /// <summary>
    /// An item is an object in the games which the player can
    /// pick up, keep in their bag, and use in some manner. They
    /// have various uses, including healing, powering up, helping
    /// catch Pokémon, or to access a new area.
    /// </summary>
    public sealed record Item : NamedApiResource
    {
        /// <summary>
        /// The identifier for this resource.
        /// </summary>
        public override int Id { get; init; }

        internal new static string ApiEndpoint { get; } = "item";

        /// <summary>
        /// The name for this resource.
        /// </summary>
        public required override string Name { get; init; }

        /// <summary>
        /// The price of this item in stores.
        /// </summary>
        public int Cost { get; init; }

        /// <summary>
        /// The power of the move Fling when used with this item.
        /// </summary>
        [JsonPropertyName("fling_power")]
        public int? FlingPower { get; init; }

        /// <summary>
        /// The effect of the move Fling when used with this item.
        /// </summary>
        [JsonPropertyName("fling_effect")]
        public NamedApiResource<ItemFlingEffect>? FlingEffect { get; init; }

        /// <summary>
        /// A list of attributes this item has.
        /// </summary>
        public required List<NamedApiResource<ItemAttribute>> Attributes { get; init; }

        /// <summary>
        /// The category of items this item falls into.
        /// </summary>
        public required NamedApiResource<ItemCategory> Category { get; init; }

        /// <summary>
        /// The effect of this ability listed in different languages.
        /// </summary>
        [JsonPropertyName("effect_entries")]
        public required List<VerboseEffect> EffectEntries { get; init; }

        /// <summary>
        /// The flavor text of this ability listed in different languages.
        /// </summary>
        [JsonPropertyName("flavor_text_entries")]
        public required List<VersionGroupFlavorText> FlavorGroupTextEntries { get; init; }

        /// <summary>
        /// A list of game indices relevent to this item by generation.
        /// </summary>
        [JsonPropertyName("game_indices")]
        public required List<GenerationGameIndex> GameIndices { get; init; }

        /// <summary>
        /// The name of this item listed in different languages.
        /// </summary>
        public required List<Names> Names { get; init; }

        /// <summary>
        /// A set of sprites used to depict this item in the game.
        /// </summary>
        public required ItemSprites Sprites { get; init; }

        /// <summary>
        /// A list of Pokémon that might be found in the wild holding this item.
        /// </summary>
        [JsonPropertyName("held_by_pokemon")]
        public required List<ItemHolderPokemon> HeldByPokemon { get; init; }

        /// <summary>
        /// An evolution chain this item requires to produce a baby during mating.
        /// </summary>
        [JsonPropertyName("baby_trigger_for")]
        public ApiResource<EvolutionChain>? BabyTriggerFor { get; init; }

        /// <summary>
        /// A list of the machines related to this item.
        /// </summary>
        public required List<MachineVersionDetail> Machines { get; init; }
    }

    /// <summary>
    /// The default description of this item.
    /// </summary>
    public sealed record ItemSprites
    {
        /// <summary>
        /// The default description of this item.
        /// </summary>
        public string Default { get; init; }
    }

    /// <summary>
    /// Information for which Pokémon holds an item
    /// </summary>
    public sealed record ItemHolderPokemon
    {
        /// <summary>
        /// The Pokémon that holds this item.
        /// </summary>
        /// <remarks>The docs lie; this is not a string</remarks>
        public required NamedApiResource<Pokemon> Pokemon { get; init; }

        /// <summary>
        /// The details for the version that this item is held in by the Pokémon.
        /// </summary>
        [JsonPropertyName("version_details")]
        public required List<ItemHolderPokemonVersionDetail> VersionDetails { get; init; }
    }

    /// <summary>
    /// Information for which Pokémon hold an item
    /// </summary>
    public sealed record ItemHolderPokemonVersionDetail
    {
        /// <summary>
        /// How often this Pokémon holds this item in this version.
        /// </summary>
        public int Rarity { get; init; }

        /// <summary>
        /// The version that this item is held in by the Pokémon.
        /// </summary>
        public required NamedApiResource<Version> Version { get; init; }
    }

    /// <summary>
    /// Item attributes define particular aspects of items,
    /// e.g. "usable in battle" or "consumable".
    /// </summary>
    public sealed record ItemAttribute : NamedApiResource
    {
        /// <summary>
        /// The identifier for this resource.
        /// </summary>
        public override int Id { get; init; }

        internal new static string ApiEndpoint { get; } = "item-attribute";

        /// <summary>
        /// The name for this resource.
        /// </summary>
        public required override string Name { get; init; }

        /// <summary>
        /// A list of items that have this attribute.
        /// </summary>
        public required List<NamedApiResource<Item>> Items { get; init; }

        /// <summary>
        /// The name of this item attribute listed in different languages.
        /// </summary>
        public required List<Names> Names { get; init; }

        /// <summary>
        /// The description of this item attribute listed in different languages.
        /// </summary>
        public required List<Descriptions> Descriptions { get; init; }
    }

    /// <summary>
    /// Item categories determine where items will be placed in the players bag.
    /// </summary>
    public sealed record ItemCategory : NamedApiResource
    {
        /// <summary>
        /// The identifier for this resource.
        /// </summary>
        public override int Id { get; init; }

        internal new static string ApiEndpoint { get; } = "item-category";

        /// <summary>
        /// The name for this resource.
        /// </summary>
        public required override string Name { get; init; }

        /// <summary>
        /// A list of items that are a part of this category.
        /// </summary>
        public required List<NamedApiResource<Item>> Items { get; init; }

        /// <summary>
        /// The name of this item category listed in different languages.
        /// </summary>
        public required List<Names> Names { get; init; }

        /// <summary>
        /// The pocket items in this category would be put in.
        /// </summary>
        public required NamedApiResource<ItemPocket> Pocket { get; init; }
    }

    /// <summary>
    /// The various effects of the move "Fling" when used with different items.
    /// </summary>
    public sealed record ItemFlingEffect : NamedApiResource
    {
        /// <summary>
        /// The identifier for this resource.
        /// </summary>
        public override int Id { get; init; }

        internal new static string ApiEndpoint { get; } = "item-fling-effect";

        /// <summary>
        /// The name for this resource.
        /// </summary>
        public required override string Name { get; init; }

        /// <summary>
        /// The result of this fling effect listed in different languages.
        /// </summary>
        [JsonPropertyName("effect_entries")]
        public required List<Effects> EffectEntries { get; init; }

        /// <summary>
        /// A list of items that have this fling effect.
        /// </summary>
        public required List<NamedApiResource<Item>> Items { get; init; }
    }

    /// <summary>
    /// Pockets within the players bag used for storing items by category.
    /// </summary>
    public sealed record ItemPocket : NamedApiResource
    {
        /// <summary>
        /// The identifier for this resource.
        /// </summary>
        public override int Id { get; init; }

        internal new static string ApiEndpoint { get; } = "item-pocket";

        /// <summary>
        /// The name for this resource.
        /// </summary>
        public required override string Name { get; init; }

        /// <summary>
        /// A list of item categories that are relevant to this item pocket.
        /// </summary>
        public required List<NamedApiResource<ItemCategory>> Categories { get; init; }

        /// <summary>
        /// The name of this resource listed in different languages.
        /// </summary>
        public required List<Names> Names { get; init; }
    }
}
