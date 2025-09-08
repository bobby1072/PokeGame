using System.Net;
using BT.Common.Persistence.Shared.Utils;
using FluentValidation;
using Microsoft.Extensions.Logging;
using PokeGame.Core.Common.Exceptions;
using PokeGame.Core.Domain.Services.Abstract;
using PokeGame.Core.Domain.Services.Models;
using PokeGame.Core.Persistence.Repositories.Abstract;
using PokeGame.Core.Schemas.Input;

namespace PokeGame.Core.Domain.Services.User.Commands;

internal sealed class SaveUserCommand: IDomainCommand<SaveUserInput, DomainCommandResult<Schemas.User>>
{
    public string CommandName => nameof(SaveUserCommand);
    private readonly IUserRepository _userRepository;
    private readonly IValidator<Schemas.User> _validator;
    private readonly ILogger<SaveUserCommand> _logger;
    public SaveUserCommand(IUserRepository userRepository, IValidator<Schemas.User> validator, ILogger<SaveUserCommand> logger)
    {
        _userRepository = userRepository;
        _validator = validator;
        _logger = logger;
    }
    
    
    public async Task<DomainCommandResult<Schemas.User>> ExecuteAsync(SaveUserInput email, CancellationToken cancellationToken = default)
    {
        _logger.LogInformation("About to attempt to save user with name: {Name}...", email.Name);

        var parsedUser = email.ToUserModel();
        var validationResult = await _validator.ValidateAsync(parsedUser);

        if (!validationResult.IsValid)
        {
            _logger.LogInformation("User to save with name: {Name} failed validation...", email.Name);
            
            throw new PokeGameApiUserException(HttpStatusCode.BadRequest, "Invalid email address");
        }

        if (parsedUser.Id is not null)
        {
            _logger.LogInformation("The user is to be updated not created. About to attempt to retrieve existing user with id: {UserId}...", parsedUser.Id);
            
            var foundExistingUser = await EntityFrameworkUtils
                .TryDbOperation(() => _userRepository.GetOne(parsedUser.Id))
                    ?? throw new PokeGameApiServerException("Failed to retrieve existing user");
            
            if (!foundExistingUser.IsSuccessful || foundExistingUser.Data is null)
            {
                throw new PokeGameApiServerException("Failed to retrieve existing user");
            }
            
            parsedUser.DateCreated = foundExistingUser.Data.DateCreated;
            parsedUser.DateModified = DateTime.UtcNow;
        }
        
        var createdUser = await EntityFrameworkUtils
            .TryDbOperation(() => parsedUser.Id is null ? _userRepository.Create(parsedUser): _userRepository.Update(parsedUser), _logger)
                ?? throw new PokeGameApiServerException("Failed to save user");

        _logger.LogDebug("User saved: {@FullUser}", createdUser);
        
        if (!createdUser.IsSuccessful || createdUser.Data.Count == 0)
        {
            throw new PokeGameApiServerException("Failed to save user");
        }

        var result = createdUser.FirstResult;
        
        _logger.LogInformation("Successfully saved user with name: {Name}", result);
        
        return new DomainCommandResult<Schemas.User> 
        {
            CommandResult = result
        };
    }
}