using System.ComponentModel.DataAnnotations.Schema;
using PokeGame.Core.Schemas;

namespace PokeGame.Core.Persistence.Entities;

[Table("item_stack", Schema = "public")]
public sealed class ItemStackEntity : BasePokeGameEntity<Guid?, ItemStack>
{
    public required Guid GameSaveId { get; set; }
    public required string ResourceName { get; set; }
    public required int Quantity { get; set; }

    public override ItemStack ToModel()
    {
        return new ItemStack
        {
            Id = Id,
            GameSaveId = GameSaveId,
            ResourceName = ResourceName,
            Quantity = Quantity
        };
    }
}