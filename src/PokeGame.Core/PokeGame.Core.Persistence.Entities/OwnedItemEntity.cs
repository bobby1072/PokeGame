using System.ComponentModel.DataAnnotations.Schema;
using PokeGame.Core.Schemas;
using PokeGame.Core.Schemas.Game;
using PokeGame.Core.Schemas.Game.PokemonRelated;

namespace PokeGame.Core.Persistence.Entities;

[Table("owned_item", Schema = "public")]
public sealed class OwnedItemEntity : BasePokeGameEntity<Guid?, OwnedItem>
{
    public required Guid GameSaveId { get; set; }
    public required string ResourceName { get; set; }
    public required int Quantity { get; set; }

    public override OwnedItem ToModel()
    {
        return new OwnedItem
        {
            Id = Id,
            GameSaveId = GameSaveId,
            ResourceName = ResourceName,
            Quantity = Quantity
        };
    }
}