namespace PokeGame.Core.Schemas;

public sealed class ItemStack : PersistableDomainModel<ItemStack, Guid?>
{
    public required Guid GameSaveId { get; set; }
    public required string ResourceName { get; set; }
    public required int Quantity { get; set; }

    public override bool Equals(ItemStack? other)
    {
        return other is not null &&
               GameSaveId == other.GameSaveId &&
               ResourceName == other.ResourceName &&
               Quantity == other.Quantity;
    }
}