using System.Net;
using BT.Common.Persistence.Shared.Utils;
using BT.Common.Services.Concrete;
using Microsoft.Extensions.Logging;
using PokeGame.Core.Common.Configurations;
using PokeGame.Core.Common.Exceptions;
using PokeGame.Core.Domain.Services.Abstract;
using PokeGame.Core.Domain.Services.Models;
using PokeGame.Core.Persistence.Entities;
using PokeGame.Core.Persistence.Repositories.Abstract;

namespace PokeGame.Core.Domain.Services.User.Commands;

internal sealed class GetUserByIdCommand
    : IDomainCommand<Guid, DomainCommandResult<Schemas.Game.User>>
{
    public string CommandName => nameof(GetUserByIdCommand);
    private readonly IUserRepository _userRepository;
    private readonly DbOperationRetrySettings _dbRetryOperation;
    private readonly ILogger<GetUserByIdCommand> _logger;

    public GetUserByIdCommand(
        IUserRepository userRepository,
        DbOperationRetrySettings dbOperationRetrySettings,
        ILogger<GetUserByIdCommand> logger
    )
    {
        _userRepository = userRepository;
        _dbRetryOperation = dbOperationRetrySettings;
        _logger = logger;
    }

    public async Task<DomainCommandResult<Schemas.Game.User>> ExecuteAsync(
        Guid input,
        CancellationToken cancellationToken = default
    )
    {
        using var activity = TelemetryHelperService.ActivitySource.StartActivity(CommandName);
        activity?.SetTag("userId", input.ToString());

        _logger.LogInformation("About to attempt to get user with id: {Email}", input);

        var foundUser =
            await EntityFrameworkUtils.TryDbOperation(
                () => _userRepository.GetOne(input),
                _logger,
                _dbRetryOperation
            ) ?? throw new PokeGameApiServerException("Failed to retrieve user");

        if (foundUser.Data is null)
        {
            throw new PokeGameApiUserException(HttpStatusCode.NotFound, "User not found");
        }

        return new DomainCommandResult<Schemas.Game.User> { CommandResult = foundUser.Data };
    }
}
