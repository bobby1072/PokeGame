using System.Net;
using AutoFixture;
using BT.Common.Persistence.Shared.Models;
using Microsoft.Extensions.Logging.Abstractions;
using Moq;
using PokeGame.Core.Common.Exceptions;
using PokeGame.Core.Domain.Services.Game.Commands;
using PokeGame.Core.Domain.Services.Models;
using PokeGame.Core.Persistence.Repositories.Abstract;
using PokeGame.Core.Schemas.Game;

namespace PokeGame.Core.Tests.DomainServiceTests.GameTests;

public sealed class StartGameSessionCommandTests
{
    private static readonly Fixture _fixture = new();
    private readonly Mock<IGameSessionRepository> _mockGameSessionRepository = new();
    private readonly Mock<IGameSaveRepository> _mockGameSaveRepository = new();
    private readonly StartGameSessionCommand _command;

    public StartGameSessionCommandTests()
    {
        _command = new StartGameSessionCommand(
            _mockGameSessionRepository.Object,
            _mockGameSaveRepository.Object,
            new NullLogger<StartGameSessionCommand>()
        );
    }

    [Fact]
    public async Task ExecuteAsync_Should_Create_And_Return_GameSession_When_Valid_Input()
    {
        // Arrange
        var gameSaveId = Guid.NewGuid();
        var user = _fixture.Create<User>();
        var input = (gameSaveId, user);

        var existingGameSave = new GameSave
        {
            Id = gameSaveId,
            UserId = user.Id!.Value,
            CharacterName = _fixture.Create<string>(),
        };

        var gameSaveResult = new DbGetOneResult<GameSave>(existingGameSave);
        var expectedGameSession = new GameSession
        {
            Id = Guid.NewGuid(),
            GameSaveId = gameSaveId,
            UserId = user.Id!.Value,
        };

        var sessionCreateResult = new DbSaveResult<GameSession>(new[] { expectedGameSession });

        _mockGameSaveRepository.Setup(x => x.GetOne(gameSaveId)).ReturnsAsync(gameSaveResult);

        _mockGameSessionRepository
            .Setup(x => x.DeleteAllCurrentSessionsAsync(gameSaveId))
            .Returns(Task.CompletedTask);

        _mockGameSessionRepository
            .Setup(x => x.Create(It.IsAny<GameSession>()))
            .ReturnsAsync(sessionCreateResult);

        // Act
        var result = await _command.ExecuteAsync(input);

        // Assert
        Assert.NotNull(result);
        Assert.NotNull(result.CommandResult);
        Assert.Equal(gameSaveId, result.CommandResult.GameSaveId);
        Assert.Equal(user.Id, result.CommandResult.UserId);

        _mockGameSaveRepository.Verify(x => x.GetOne(gameSaveId), Times.Once);
        _mockGameSessionRepository.Verify(
            x => x.DeleteAllCurrentSessionsAsync(gameSaveId),
            Times.Once
        );
        _mockGameSessionRepository.Verify(x => x.Create(It.IsAny<GameSession>()), Times.Once);
    }

    [Fact]
    public async Task ExecuteAsync_Should_Throw_PokeGameApiServerException_When_GameSave_Fetch_Fails()
    {
        // Arrange
        var gameSaveId = Guid.NewGuid();
        var user = _fixture.Create<User>();
        var input = (gameSaveId, user);

        _mockGameSaveRepository
            .Setup(x => x.GetOne(gameSaveId))
            .ReturnsAsync((DbGetOneResult<GameSave>)null!);

        // Act & Assert
        var exception = await Assert.ThrowsAsync<PokeGameApiServerException>(
            () => _command.ExecuteAsync(input)
        );

        Assert.Equal("Failed to fetch game save", exception.Message);
        _mockGameSaveRepository.Verify(x => x.GetOne(gameSaveId), Times.Once);
        _mockGameSessionRepository.Verify(
            x => x.DeleteAllCurrentSessionsAsync(It.IsAny<Guid>()),
            Times.Never
        );
        _mockGameSessionRepository.Verify(x => x.Create(It.IsAny<GameSession>()), Times.Never);
    }

    [Fact]
    public async Task ExecuteAsync_Should_Throw_PokeGameApiUserException_When_GameSave_Not_Found()
    {
        // Arrange
        var gameSaveId = Guid.NewGuid();
        var user = _fixture.Create<User>();
        var input = (gameSaveId, user);

        var gameSaveResult = new DbGetOneResult<GameSave>(null!) { IsSuccessful = true };

        _mockGameSaveRepository.Setup(x => x.GetOne(gameSaveId)).ReturnsAsync(gameSaveResult);

        // Act & Assert
        var exception = await Assert.ThrowsAsync<PokeGameApiUserException>(
            () => _command.ExecuteAsync(input)
        );

        Assert.Equal(HttpStatusCode.BadRequest, exception.StatusCode);
        Assert.Equal("Invalid game save id provided", exception.Message);
        _mockGameSaveRepository.Verify(x => x.GetOne(gameSaveId), Times.Once);
        _mockGameSessionRepository.Verify(
            x => x.DeleteAllCurrentSessionsAsync(It.IsAny<Guid>()),
            Times.Never
        );
        _mockGameSessionRepository.Verify(x => x.Create(It.IsAny<GameSession>()), Times.Never);
    }

    [Fact]
    public async Task ExecuteAsync_Should_Throw_PokeGameApiUserException_When_GameSave_Belongs_To_Different_User()
    {
        // Arrange
        var gameSaveId = Guid.NewGuid();
        var user = _fixture.Create<User>();
        var input = (gameSaveId, user);

        var existingGameSave = new GameSave
        {
            Id = gameSaveId,
            UserId = Guid.NewGuid(), // Different user
            CharacterName = _fixture.Create<string>(),
        };

        var gameSaveResult = new DbGetOneResult<GameSave>(existingGameSave);

        _mockGameSaveRepository.Setup(x => x.GetOne(gameSaveId)).ReturnsAsync(gameSaveResult);

        // Act & Assert
        var exception = await Assert.ThrowsAsync<PokeGameApiUserException>(
            () => _command.ExecuteAsync(input)
        );

        Assert.Equal(HttpStatusCode.BadRequest, exception.StatusCode);
        Assert.Equal("Invalid game save id provided", exception.Message);
        _mockGameSaveRepository.Verify(x => x.GetOne(gameSaveId), Times.Once);
        _mockGameSessionRepository.Verify(
            x => x.DeleteAllCurrentSessionsAsync(It.IsAny<Guid>()),
            Times.Never
        );
        _mockGameSessionRepository.Verify(x => x.Create(It.IsAny<GameSession>()), Times.Never);
    }

    [Fact]
    public async Task ExecuteAsync_Should_Throw_PokeGameApiServerException_When_Session_Creation_Fails()
    {
        // Arrange
        var gameSaveId = Guid.NewGuid();
        var user = _fixture.Create<User>();
        var input = (gameSaveId, user);

        var existingGameSave = new GameSave
        {
            Id = gameSaveId,
            UserId = user.Id!.Value,
            CharacterName = _fixture.Create<string>(),
        };

        var gameSaveResult = new DbGetOneResult<GameSave>(existingGameSave);

        _mockGameSaveRepository.Setup(x => x.GetOne(gameSaveId)).ReturnsAsync(gameSaveResult);

        _mockGameSessionRepository
            .Setup(x => x.DeleteAllCurrentSessionsAsync(gameSaveId))
            .Returns(Task.CompletedTask);

        _mockGameSessionRepository
            .Setup(x => x.Create(It.IsAny<GameSession>()))
            .ReturnsAsync((DbSaveResult<GameSession>)null!);

        // Act & Assert
        var exception = await Assert.ThrowsAsync<PokeGameApiServerException>(
            () => _command.ExecuteAsync(input)
        );

        Assert.Equal("Failed to add new game session", exception.Message);
        _mockGameSaveRepository.Verify(x => x.GetOne(gameSaveId), Times.Once);
        _mockGameSessionRepository.Verify(
            x => x.DeleteAllCurrentSessionsAsync(gameSaveId),
            Times.Once
        );
        _mockGameSessionRepository.Verify(x => x.Create(It.IsAny<GameSession>()), Times.Once);
    }

    [Fact]
    public void CommandName_Should_Return_Correct_Name()
    {
        // Act & Assert
        Assert.Equal(nameof(StartGameSessionCommand), _command.CommandName);
    }
}
