using System.ComponentModel.DataAnnotations.Schema;
using PokeGame.Core.Schemas;
using PokeGame.Core.Schemas.Game;

namespace PokeGame.Core.Persistence.Entities;

[Table("user", Schema = "public")]
public sealed class UserEntity: BasePokeGameEntity<Guid?, User>
{
    public required string Email { get; set; }
    public required string Name { get; set; }
    public DateTime DateCreated { get; set; } = DateTime.UtcNow;
    public DateTime DateModified { get; set; } = DateTime.UtcNow; 
    public override User ToModel()
    {
        return new User
        {
            Id = Id,
            Email = Email,
            Name = Name,
            DateCreated = DateCreated,
            DateModified = DateModified
        };
    }
}