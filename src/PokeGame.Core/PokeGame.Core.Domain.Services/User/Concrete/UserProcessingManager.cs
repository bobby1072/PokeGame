using Microsoft.Extensions.Logging;
using PokeGame.Core.Domain.Services.Abstract;
using PokeGame.Core.Domain.Services.Models;
using PokeGame.Core.Domain.Services.User.Abstract;
using PokeGame.Core.Domain.Services.User.Commands;
using PokeGame.Core.Schemas.Input;

namespace PokeGame.Core.Domain.Services.User.Concrete;

internal sealed class UserProcessingManager : IUserProcessingManager
{
    private readonly IDomainServiceCommandExecutor _commandExecutor;
    public UserProcessingManager(IDomainServiceCommandExecutor commandExecutor)
    {
        _commandExecutor = commandExecutor;
    }

    public async Task<Schemas.Game.User> GetUserAsync(string email) => (await _commandExecutor
        .RunCommandAsync<GetUserByEmailCommand, string, DomainCommandResult<Schemas.Game.User>>(email)).CommandResult;
    public async Task<Schemas.Game.User> SaveUserAsync(SaveUserInput input) => (await _commandExecutor
        .RunCommandAsync<SaveUserCommand, SaveUserInput, DomainCommandResult<Schemas.Game.User>>(input)).CommandResult;
}