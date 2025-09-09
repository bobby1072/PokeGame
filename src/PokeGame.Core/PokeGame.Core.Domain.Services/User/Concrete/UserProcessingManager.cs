using Microsoft.Extensions.Logging;
using PokeGame.Core.Domain.Services.Abstract;
using PokeGame.Core.Domain.Services.Models;
using PokeGame.Core.Domain.Services.User.Abstract;
using PokeGame.Core.Domain.Services.User.Commands;
using PokeGame.Core.Schemas.Input;

namespace PokeGame.Core.Domain.Services.User.Concrete;

internal sealed class UserProcessingManager : IUserProcessingManager
{
    private readonly IScopedDomainServiceCommandExecutor _commandExecutor;
    public UserProcessingManager(IScopedDomainServiceCommandExecutor commandExecutor)
    {
        _commandExecutor = commandExecutor;
    }

    public async Task<Schemas.User> GetUserAsync(string email) => (await _commandExecutor
        .RunCommandAsync<GetUserByEmailCommand, string, DomainCommandResult<Schemas.User>>(email)).CommandResult;
    public async Task<Schemas.User> SaveUserAsync(SaveUserInput input) => (await _commandExecutor
        .RunCommandAsync<SaveUserCommand, SaveUserInput, DomainCommandResult<Schemas.User>>(input)).CommandResult;
}