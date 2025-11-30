using System.Net;
using BT.Common.Helpers.Extensions;
using BT.Common.Persistence.Shared.Utils;
using BT.Common.Services.Concrete;
using Microsoft.Extensions.Logging;
using PokeGame.Core.Common.Exceptions;
using PokeGame.Core.Domain.Services.Abstract;
using PokeGame.Core.Domain.Services.Models;
using PokeGame.Core.Persistence.Entities;
using PokeGame.Core.Persistence.Repositories.Abstract;

namespace PokeGame.Core.Domain.Services.User.Commands;

internal sealed class GetUserByEmailCommand
    : IDomainCommand<string, DomainCommandResult<Schemas.Game.User>>
{
    public string CommandName => nameof(GetUserByEmailCommand);
    private readonly IUserRepository _userRepository;
    private readonly ILogger<GetUserByEmailCommand> _logger;

    public GetUserByEmailCommand(
        IUserRepository userRepository,
        ILogger<GetUserByEmailCommand> logger
    )
    {
        _userRepository = userRepository;
        _logger = logger;
    }

    public async Task<DomainCommandResult<Schemas.Game.User>> ExecuteAsync(
        string input,
        CancellationToken cancellationToken = default
    )
    {
        using var activity = TelemetryHelperService.ActivitySource.StartActivity(CommandName);
        activity?.SetTag("email", input);

        _logger.LogInformation("About to attempt to get user with id: {Email}", input);

        if (!input.IsValidEmail())
        {
            throw new PokeGameApiUserException(HttpStatusCode.BadRequest, "Invalid email");
        }

        var foundUser =
            await EntityFrameworkUtils.TryDbOperation(
                () => _userRepository.GetOne(input, nameof(UserEntity.Email)),
                _logger
            ) ?? throw new PokeGameApiServerException("Failed to retrieve user");

        if (foundUser.Data is null)
        {
            throw new PokeGameApiUserException(HttpStatusCode.NotFound, "User not found");
        }

        return new DomainCommandResult<Schemas.Game.User> { CommandResult = foundUser.Data };
    }
}
