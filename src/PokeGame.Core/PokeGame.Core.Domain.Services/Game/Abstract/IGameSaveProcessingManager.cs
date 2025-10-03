using PokeGame.Core.Schemas.Game;

namespace PokeGame.Core.Domain.Services.Game.Abstract;

public interface IGameSaveProcessingManager
{
    Task<GameSave> SaveGameAsync(string characterName, Schemas.Game.User currentUser);
    Task<IReadOnlyCollection<GameSave>> GetGameSavesForUserAsync(Schemas.Game.User currentUser);
}