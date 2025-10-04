using AutoFixture;
using BT.Common.Persistence.Shared.Models;
using FluentValidation;
using Microsoft.Extensions.Logging.Abstractions;
using Moq;
using PokeGame.Core.Common.Exceptions;
using PokeGame.Core.Domain.Services.Abstract;
using PokeGame.Core.Domain.Services.Models;
using PokeGame.Core.Domain.Services.User.Commands;
using PokeGame.Core.Persistence.Repositories.Abstract;
using PokeGame.Core.Schemas.Game;
using PokeGame.Core.Schemas.Input;

namespace PokeGame.Core.Tests.DomainServiceTests.UserTests;

public sealed class SaveUserCommandTests
{
    private static readonly Fixture _fixture = new();
    private readonly Mock<IUserRepository> _mockUserRepository = new();
    private readonly Mock<IValidatorService> _mockValidatorService = new();
    private readonly SaveUserCommand _command;

    public SaveUserCommandTests()
    {
        _command = new SaveUserCommand(
            _mockUserRepository.Object,
            _mockValidatorService.Object,
            new NullLogger<SaveUserCommand>()
        );
    }

    [Fact]
    public async Task ExecuteAsync_Should_Create_New_User_When_Id_Is_Null()
    {
        // Arrange
        var input = new SaveUserInput
        {
            Id = null,
            Email = "test@example.com",
            Name = "Test User"
        };

        var expectedUser = input.ToUserModel();
        expectedUser.Id = Guid.NewGuid();
        var dbResult = new DbSaveResult<User>(new[] { expectedUser });

        _mockValidatorService
            .Setup(x => x.ValidateAndThrowAsync(It.IsAny<User>(), It.IsAny<CancellationToken>()))
            .Returns(Task.CompletedTask);

        _mockUserRepository
            .Setup(x => x.Create(It.IsAny<User>()))
            .ReturnsAsync(dbResult);

        // Act
        var result = await _command.ExecuteAsync(input);

        // Assert
        Assert.NotNull(result);
        Assert.Equal(expectedUser, result.CommandResult);
        _mockValidatorService.Verify(x => x.ValidateAndThrowAsync(It.IsAny<User>(), It.IsAny<CancellationToken>()), Times.Once);
        _mockUserRepository.Verify(x => x.Create(It.IsAny<User>()), Times.Once);
        _mockUserRepository.Verify(x => x.GetOne(It.IsAny<Guid?>()), Times.Never);
        _mockUserRepository.Verify(x => x.Update(It.IsAny<User>()), Times.Never);
    }

    [Fact]
    public async Task ExecuteAsync_Should_Update_Existing_User_When_Id_Is_Not_Null()
    {
        // Arrange
        var userId = Guid.NewGuid();
        var input = new SaveUserInput
        {
            Id = userId,
            Email = "test@example.com",
            Name = "Updated User"
        };

        var existingUser = _fixture.Create<User>();
        existingUser.Id = userId;
        var existingUserDbResult = new DbGetOneResult<User>(existingUser);

        var updatedUser = input.ToUserModel();
        updatedUser.Id = userId;
        updatedUser.DateCreated = existingUser.DateCreated;
        var updateDbResult = new DbSaveResult<User>(new[] { updatedUser });

        _mockValidatorService
            .Setup(x => x.ValidateAndThrowAsync(It.IsAny<User>(), It.IsAny<CancellationToken>()))
            .Returns(Task.CompletedTask);

        _mockUserRepository
            .Setup(x => x.GetOne(userId))
            .ReturnsAsync(existingUserDbResult);

        _mockUserRepository
            .Setup(x => x.Update(It.IsAny<User>()))
            .ReturnsAsync(updateDbResult);

        // Act
        var result = await _command.ExecuteAsync(input);

        // Assert
        Assert.NotNull(result);
        Assert.Equal(updatedUser, result.CommandResult);
        _mockValidatorService.Verify(x => x.ValidateAndThrowAsync(It.IsAny<User>(), It.IsAny<CancellationToken>()), Times.Once);
        _mockUserRepository.Verify(x => x.GetOne(userId), Times.Once);
        _mockUserRepository.Verify(x => x.Update(It.IsAny<User>()), Times.Once);
        _mockUserRepository.Verify(x => x.Create(It.IsAny<User>()), Times.Never);
    }

    [Fact]
    public async Task ExecuteAsync_Should_Throw_ValidationException_When_Validation_Fails()
    {
        // Arrange
        var input = new SaveUserInput
        {
            Id = null,
            Email = "invalid-email",
            Name = "Test User"
        };

        var validationException = new ValidationException("Validation failed");

        _mockValidatorService
            .Setup(x => x.ValidateAndThrowAsync(It.IsAny<User>(), It.IsAny<CancellationToken>()))
            .ThrowsAsync(validationException);

        // Act & Assert
        var exception = await Assert.ThrowsAsync<ValidationException>(
            () => _command.ExecuteAsync(input)
        );

        Assert.Equal("Validation failed", exception.Message);
        _mockValidatorService.Verify(x => x.ValidateAndThrowAsync(It.IsAny<User>(), It.IsAny<CancellationToken>()), Times.Once);
        _mockUserRepository.Verify(x => x.Create(It.IsAny<User>()), Times.Never);
        _mockUserRepository.Verify(x => x.Update(It.IsAny<User>()), Times.Never);
    }

    [Fact]
    public async Task ExecuteAsync_Should_Throw_PokeGameApiServerException_When_GetOne_Returns_Null_For_Update()
    {
        // Arrange
        var userId = Guid.NewGuid();
        var input = new SaveUserInput
        {
            Id = userId,
            Email = "test@example.com",
            Name = "Test User"
        };

        _mockValidatorService
            .Setup(x => x.ValidateAndThrowAsync(It.IsAny<User>(), It.IsAny<CancellationToken>()))
            .Returns(Task.CompletedTask);

        _mockUserRepository
            .Setup(x => x.GetOne(userId))
            .ReturnsAsync((DbGetOneResult<User>)null!);

        // Act & Assert
        var exception = await Assert.ThrowsAsync<PokeGameApiServerException>(
            () => _command.ExecuteAsync(input)
        );

        Assert.Equal("Failed to retrieve existing user", exception.Message);
        _mockValidatorService.Verify(x => x.ValidateAndThrowAsync(It.IsAny<User>(), It.IsAny<CancellationToken>()), Times.Once);
        _mockUserRepository.Verify(x => x.GetOne(userId), Times.Once);
    }

    [Fact]
    public async Task ExecuteAsync_Should_Throw_PokeGameApiServerException_When_GetOne_Returns_Unsuccessful_For_Update()
    {
        // Arrange
        var userId = Guid.NewGuid();
        var input = new SaveUserInput
        {
            Id = userId,
            Email = "test@example.com",
            Name = "Test User"
        };

        var existingUserDbResult = new DbGetOneResult<User>(null!);

        _mockValidatorService
            .Setup(x => x.ValidateAndThrowAsync(It.IsAny<User>(), It.IsAny<CancellationToken>()))
            .Returns(Task.CompletedTask);

        _mockUserRepository
            .Setup(x => x.GetOne(userId))
            .ReturnsAsync(existingUserDbResult);

        // Act & Assert
        var exception = await Assert.ThrowsAsync<PokeGameApiServerException>(
            () => _command.ExecuteAsync(input)
        );

        Assert.Equal("Failed to retrieve existing user", exception.Message);
        _mockValidatorService.Verify(x => x.ValidateAndThrowAsync(It.IsAny<User>(), It.IsAny<CancellationToken>()), Times.Once);
        _mockUserRepository.Verify(x => x.GetOne(userId), Times.Once);
    }

    [Fact]
    public async Task ExecuteAsync_Should_Throw_PokeGameApiServerException_When_Create_Returns_Null()
    {
        // Arrange
        var input = new SaveUserInput
        {
            Id = null,
            Email = "test@example.com",
            Name = "Test User"
        };

        _mockValidatorService
            .Setup(x => x.ValidateAndThrowAsync(It.IsAny<User>(), It.IsAny<CancellationToken>()))
            .Returns(Task.CompletedTask);

        _mockUserRepository
            .Setup(x => x.Create(It.IsAny<User>()))
            .ReturnsAsync((DbSaveResult<User>)null!);

        // Act & Assert
        var exception = await Assert.ThrowsAsync<PokeGameApiServerException>(
            () => _command.ExecuteAsync(input)
        );

        Assert.Equal("Failed to save user", exception.Message);
        _mockValidatorService.Verify(x => x.ValidateAndThrowAsync(It.IsAny<User>(), It.IsAny<CancellationToken>()), Times.Once);
        _mockUserRepository.Verify(x => x.Create(It.IsAny<User>()), Times.Once);
    }

    [Fact]
    public async Task ExecuteAsync_Should_Throw_PokeGameApiServerException_When_Create_Returns_Unsuccessful()
    {
        // Arrange
        var input = new SaveUserInput
        {
            Id = null,
            Email = "test@example.com",
            Name = "Test User"
        };

        var dbResult = new DbSaveResult<User>(Array.Empty<User>());

        _mockValidatorService
            .Setup(x => x.ValidateAndThrowAsync(It.IsAny<User>(), It.IsAny<CancellationToken>()))
            .Returns(Task.CompletedTask);

        _mockUserRepository
            .Setup(x => x.Create(It.IsAny<User>()))
            .ReturnsAsync(dbResult);

        // Act & Assert
        var exception = await Assert.ThrowsAsync<PokeGameApiServerException>(
            () => _command.ExecuteAsync(input)
        );

        Assert.Equal("Failed to save user", exception.Message);
        _mockValidatorService.Verify(x => x.ValidateAndThrowAsync(It.IsAny<User>(), It.IsAny<CancellationToken>()), Times.Once);
        _mockUserRepository.Verify(x => x.Create(It.IsAny<User>()), Times.Once);
    }

    [Fact]
    public async Task ExecuteAsync_Should_Throw_PokeGameApiServerException_When_Create_Returns_Empty_Data()
    {
        // Arrange
        var input = new SaveUserInput
        {
            Id = null,
            Email = "test@example.com",
            Name = "Test User"
        };

        var dbResult = new DbSaveResult<User>(Array.Empty<User>());

        _mockValidatorService
            .Setup(x => x.ValidateAndThrowAsync(It.IsAny<User>(), It.IsAny<CancellationToken>()))
            .Returns(Task.CompletedTask);

        _mockUserRepository
            .Setup(x => x.Create(It.IsAny<User>()))
            .ReturnsAsync(dbResult);

        // Act & Assert
        var exception = await Assert.ThrowsAsync<PokeGameApiServerException>(
            () => _command.ExecuteAsync(input)
        );

        Assert.Equal("Failed to save user", exception.Message);
        _mockValidatorService.Verify(x => x.ValidateAndThrowAsync(It.IsAny<User>(), It.IsAny<CancellationToken>()), Times.Once);
        _mockUserRepository.Verify(x => x.Create(It.IsAny<User>()), Times.Once);
    }

    [Fact]
    public void CommandName_Should_Return_Correct_Name()
    {
        // Act & Assert
        Assert.Equal(nameof(SaveUserCommand), _command.CommandName);
    }
}