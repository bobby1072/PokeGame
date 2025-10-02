namespace PokeGame.Core.Schemas;

public sealed class User: PersistableDomainModel<User, Guid?>
{
    public required string Email { get; set; }
    public required string Name { get; set; }
    public DateTime DateCreated { get; set; } = DateTime.UtcNow;
    public DateTime DateModified { get; set; } = DateTime.UtcNow; 
    
    public override bool Equals(User? other)
    {
        return Email == other?.Email &&
               Name == other.Name &&
               DateCreated == other.DateCreated && 
               DateModified == other.DateModified;
    }
    
}