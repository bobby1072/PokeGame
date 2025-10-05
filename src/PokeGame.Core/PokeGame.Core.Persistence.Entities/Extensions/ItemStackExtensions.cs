using PokeGame.Core.Schemas.Game;

namespace PokeGame.Core.Persistence.Entities.Extensions;

public static class ItemStackExtensions
{
    public static ItemStackEntity ToEntity(this ItemStack itemStack)
    {
        return new ItemStackEntity
        {
            Id = itemStack.Id,
            GameSaveId = itemStack.GameSaveId,
            ResourceName = itemStack.ResourceName,
            Quantity = itemStack.Quantity,
        };
    }
}
