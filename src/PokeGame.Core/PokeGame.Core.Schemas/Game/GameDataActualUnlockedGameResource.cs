using System.Text.Json.Serialization;

namespace PokeGame.Core.Schemas.Game;

public sealed class GameDataActualUnlockedGameResource: DomainModel<GameDataActualUnlockedGameResource>
{
    [JsonConverter(typeof(JsonStringEnumConverter))]
    public required GameDataActualUnlockedGameResourceType Type { get; set; }
    public required string ResourceName { get; set; }

    public override bool Equals(GameDataActualUnlockedGameResource? other)
    {
        return other?.ResourceName == ResourceName &&
               other.Type == Type;
    }
}