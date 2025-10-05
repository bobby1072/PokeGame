using PokeGame.Core.Schemas.Game;

namespace PokeGame.Core.Persistence.Entities.Extensions;

public static class UserExtensions
{
    public static UserEntity ToEntity(this User runtimeObj)
    {
        return new UserEntity
        {
            Id = runtimeObj.Id,
            Email = runtimeObj.Email,
            Name = runtimeObj.Name,
            DateCreated = runtimeObj.DateCreated,
            DateModified = runtimeObj.DateModified,
        };
    }
}
