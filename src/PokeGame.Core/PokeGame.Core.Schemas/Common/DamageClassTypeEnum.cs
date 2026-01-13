using System.Text.Json.Serialization;

namespace PokeGame.Core.Schemas.Common;

[JsonConverter(typeof(JsonStringEnumConverter<DamageClassTypeEnum>))]
public enum DamageClassTypeEnum
{
    Physical = 1,
    Special = 2,
}
