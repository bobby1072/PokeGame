using BT.Common.Persistence.Shared.Repositories.Abstract;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using PokeGame.Core.Persistence.Contexts;
using PokeGame.Core.Persistence.Entities;
using PokeGame.Core.Persistence.Entities.Extensions;
using PokeGame.Core.Persistence.Repositories.Abstract;
using PokeGame.Core.Schemas;
using PokeGame.Core.Schemas.Game;

namespace PokeGame.Core.Persistence.Repositories.Concrete;

internal sealed class UserRepository: BaseRepository<UserEntity, Guid?, User, PokeGameContext>, IUserRepository
{
    public UserRepository(
        IDbContextFactory<PokeGameContext> dbContextFactory,
        ILogger<UserRepository> logger
    ): base(dbContextFactory, logger) {}
    protected override UserEntity RuntimeToEntity(User runtimeObj)
    {
        return runtimeObj.ToEntity();    
    }
}