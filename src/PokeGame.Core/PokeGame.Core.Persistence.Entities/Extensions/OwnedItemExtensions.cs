using PokeGame.Core.Schemas.Game;
using PokeGame.Core.Schemas.Game.PokemonRelated;

namespace PokeGame.Core.Persistence.Entities.Extensions;

public static class OwnedItemExtensions
{
    public static OwnedItemEntity ToEntity(this OwnedItem ownedItem)
    {
        return new OwnedItemEntity
        {
            Id = ownedItem.Id,
            GameSaveId = ownedItem.GameSaveId,
            ResourceName = ownedItem.ResourceName,
            Quantity = ownedItem.Quantity,
        };
    }
}
