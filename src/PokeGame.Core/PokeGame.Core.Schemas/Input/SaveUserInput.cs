
using PokeGame.Core.Schemas.Game;

namespace PokeGame.Core.Schemas.Input;

public sealed class SaveUserInput
{
    public Guid? Id { get; init; }
    public required string Email { get; init; }
    public required string Name { get; init; }


    public User ToUserModel() =>
        new()
        {
            Id = Id,
            Email = Email,
            Name = Name,
        };
}