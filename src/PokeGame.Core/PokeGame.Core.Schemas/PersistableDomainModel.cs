namespace PokeGame.Core.Schemas;

public abstract class PersistableDomainModel<TEquatable, TId> : DomainModel<TEquatable>
    where TEquatable : DomainModel<TEquatable>
{
    public required TId Id { get; set; }
}