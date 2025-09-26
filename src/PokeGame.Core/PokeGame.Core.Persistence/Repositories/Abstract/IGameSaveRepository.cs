using BT.Common.Persistence.Shared.Repositories.Abstract;
using PokeGame.Core.Persistence.Entities;
using PokeGame.Core.Schemas;

namespace PokeGame.Core.Persistence.Repositories.Abstract;

public interface IGameSaveRepository: IRepository<GameSaveEntity, Guid?, GameSave>
{ }