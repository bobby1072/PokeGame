using BT.Common.Persistence.Shared.Models;
using BT.Common.Persistence.Shared.Repositories.Abstract;
using PokeGame.Core.Persistence.Entities;
using PokeGame.Core.Schemas.Game;

namespace PokeGame.Core.Persistence.Repositories.Abstract;

public interface IGameSaveRepository : IRepository<GameSaveEntity, Guid?, GameSave>
{
    Task<DbResult> CreateGameSaveWithData(GameSave gameSave, GameSaveData data);
}