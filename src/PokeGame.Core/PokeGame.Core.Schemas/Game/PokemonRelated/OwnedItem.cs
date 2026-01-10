namespace PokeGame.Core.Schemas.Game.PokemonRelated;

public sealed class OwnedItem : PersistableDomainModel<OwnedItem, Guid?>
{
    public required Guid GameSaveId { get; set; }
    public required string ResourceName { get; set; }
    public required int Quantity { get; set; }

    public override bool Equals(OwnedItem? other)
    {
        return other is not null &&
               GameSaveId == other.GameSaveId &&
               ResourceName == other.ResourceName &&
               Quantity == other.Quantity;
    }
}