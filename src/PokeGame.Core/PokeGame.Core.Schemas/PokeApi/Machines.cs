using System.Text.Json.Serialization;

namespace PokeGame.Core.Schemas.PokeApi
{
    /// <summary>
    /// Machines are the representation of items that teach moves
    /// to Pokémon. They vary from version to version, so it is
    /// not certain that one specific TM or HM corresponds to a
    /// single Machine.
    /// </summary>
    public sealed record Machine : ApiResource
    {
        /// <summary>
        /// The identifier for this resource.
        /// </summary>
        public override int Id { get; init; }

        internal new static string ApiEndpoint { get; } = "machine";

        /// <summary>
        /// The TM or HM item that corresponds to this machine.
        /// </summary>
        public required NamedApiResource<Item> Item { get; init; }

        /// <summary>
        /// The move that is taught by this machine.
        /// </summary>
        public required NamedApiResource<Move> Move { get; init; }

        /// <summary>
        /// The version group that this machine applies to.
        /// </summary>
        [JsonPropertyName("version_group")]
        public required NamedApiResource<VersionGroup> VersionGroup { get; init; }
    }
}
