using PokeGame.Core.Schemas.Input;

namespace PokeGame.Core.Domain.Services.User.Abstract;

public interface IUserProcessingManager
{
    Task<Schemas.Game.User> GetUserAsync(Guid id);
    Task<Schemas.Game.User> GetUserAsync(string email);
    Task<Schemas.Game.User> SaveUserAsync(SaveUserInput input);
}