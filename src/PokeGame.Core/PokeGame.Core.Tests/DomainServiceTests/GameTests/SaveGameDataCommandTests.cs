using System.Net;
using AutoFixture;
using BT.Common.Persistence.Shared.Models;
using FluentValidation;
using Microsoft.Extensions.Logging.Abstractions;
using Moq;
using PokeGame.Core.Common.Exceptions;
using PokeGame.Core.Domain.Services.Abstract;
using PokeGame.Core.Domain.Services.Game.Commands;
using PokeGame.Core.Persistence.Entities;
using PokeGame.Core.Persistence.Repositories.Abstract;
using PokeGame.Core.Schemas.Game;
using User = PokeGame.Core.Schemas.Game.User;

namespace PokeGame.Core.Tests.DomainServiceTests.GameTests;

public sealed class SaveGameDataCommandTests
{
    private static readonly Fixture _fixture = new();
    private readonly Mock<IGameSessionRepository> _mockGameSessionRepository = new();
    private readonly Mock<IGameSaveDataRepository> _mockGameSaveDataRepository = new();
    private readonly Mock<IOwnedPokemonRepository> _mockOwnedPokemonRepository = new();
    private readonly Mock<IValidatorService> _mockValidatorService = new();
    private readonly SaveGameDataCommand _command;

    public SaveGameDataCommandTests()
    {
        _command = new SaveGameDataCommand(
            _mockGameSessionRepository.Object,
            _mockGameSaveDataRepository.Object,
            _mockOwnedPokemonRepository.Object,
            _mockValidatorService.Object,
            new NullLogger<SaveGameDataCommand>()
        );
    }

    [Fact]
    public async Task ExecuteAsync_Should_Save_And_Return_GameSaveData_When_Valid_Input()
    {
        // Arrange
        var userId = Guid.NewGuid();
        var gameSaveId = Guid.NewGuid();
        var connectionId = "test-connection-id";
        var user = new User
        {
            Id = userId,
            Email = _fixture.Create<string>(),
            Name = _fixture.Create<string>(),
        };

        var existingGameData = new GameSaveData
        {
            Id = 1,
            GameSaveId = gameSaveId,
            GameData = new GameSaveDataActual
            {
                LastPlayedScene = "OldScene",
                LastPlayedLocationX = 10,
                LastPlayedLocationY = 20,
                DeckPokemon = [],
                UnlockedGameResources =  []
            },
        };

        var newGameData = new GameSaveData
        {
            GameSaveId = Guid.Empty, // Will be set by command
            GameData = new GameSaveDataActual
            {
                LastPlayedScene = "NewScene",
                LastPlayedLocationX = 30,
                LastPlayedLocationY = 40,
                DeckPokemon = [],
                UnlockedGameResources =  []
            },
        };

        var gameSession = new GameSession
        {
            Id = Guid.NewGuid(),
            ConnectionId = connectionId,
            GameSaveId = gameSaveId,
            UserId = userId,
        };

        var input = (newGameData, connectionId, user);

        var gameSessionResult = new DbGetOneResult<GameSession>(gameSession);
        var existingGameDataResult = new DbGetOneResult<GameSaveData>(existingGameData);
        var updatedGameData = new GameSaveData
        {
            Id = 1,
            GameSaveId = gameSaveId,
            GameData = newGameData.GameData,
        };
        var updateResult = new DbSaveResult<GameSaveData>(new[] { updatedGameData });

        _mockValidatorService
            .Setup(x =>
                x.ValidateAndThrowAsync(It.IsAny<GameSaveData>(), It.IsAny<CancellationToken>())
            )
            .Returns(Task.CompletedTask);

        _mockGameSessionRepository
            .Setup(x => x.GetOneWithGameSaveAndDataByConnectionIdAsync(connectionId))
            .ReturnsAsync(gameSessionResult);
        
        _mockGameSaveDataRepository
            .Setup(x => x.Update(It.IsAny<GameSaveData>()))
            .ReturnsAsync(updateResult);

        // Act
        var result = await _command.ExecuteAsync(input);

        // Assert
        Assert.NotNull(result);
        Assert.NotNull(result.CommandResult);
        Assert.Equal(gameSaveId, result.CommandResult.GameSaveId);
        Assert.Equal(1, result.CommandResult.Id);
        Assert.Equal("NewScene", result.CommandResult.GameData.LastPlayedScene);
        Assert.Equal(30, result.CommandResult.GameData.LastPlayedLocationX);
        Assert.Equal(40, result.CommandResult.GameData.LastPlayedLocationY);

        _mockValidatorService.Verify(
            x => x.ValidateAndThrowAsync(It.IsAny<GameSaveData>(), It.IsAny<CancellationToken>()),
            Times.Once
        );
        _mockGameSessionRepository.Verify(
            x => x.GetOneWithGameSaveAndDataByConnectionIdAsync(connectionId),
            Times.Once
        );
        _mockGameSaveDataRepository.Verify(x => x.Update(It.IsAny<GameSaveData>()), Times.Once);
    }

    [Fact]
    public async Task ExecuteAsync_Should_Validate_New_Pokemon_Against_Old_When_Pokemon_Added()
    {
        // Arrange
        var userId = Guid.NewGuid();
        var gameSaveId = Guid.NewGuid();
        var connectionId = "test-connection-id";
        var oldPokemonId = Guid.NewGuid();
        var newPokemonId = Guid.NewGuid();

        var user = new User
        {
            Id = userId,
            Email = _fixture.Create<string>(),
            Name = _fixture.Create<string>(),
        };

        var existingGameData = new GameSaveData
        {
            Id = 1,
            GameSaveId = gameSaveId,
            GameData = new GameSaveDataActual
            {
                LastPlayedScene = "Scene",
                LastPlayedLocationX = 10,
                LastPlayedLocationY = 20,
                DeckPokemon = [new GameSaveDataActualDeckPokemon { OwnedPokemonId = oldPokemonId }],
            },
        };

        var newGameData = new GameSaveData
        {
            GameSaveId = Guid.Empty,
            GameData = new GameSaveDataActual
            {
                LastPlayedScene = "Scene",
                LastPlayedLocationX = 10,
                LastPlayedLocationY = 20,
                DeckPokemon =
                [
                    new GameSaveDataActualDeckPokemon { OwnedPokemonId = oldPokemonId },
                    new GameSaveDataActualDeckPokemon { OwnedPokemonId = newPokemonId },
                ],
            },
        };

        var gameSession = new GameSession
        {
            Id = Guid.NewGuid(),
            ConnectionId = connectionId,
            GameSaveId = gameSaveId,
            UserId = userId,
        };

        var ownedPokemon = new OwnedPokemon
        {
            Id = newPokemonId,
            GameSaveId = gameSaveId,
            PokemonResourceName = "pikachu",
            PokemonLevel = 5,
            CurrentHp = 20,
            MoveOneResourceName = "tackle",
            GameSave = new GameSave
            {
                Id = gameSaveId,
                UserId = userId,
                CharacterName = "Test",
            },
        };

        var input = (newGameData, connectionId, user);

        var gameSessionResult = new DbGetOneResult<GameSession>(gameSession);
        var existingGameDataResult = new DbGetOneResult<GameSaveData>(existingGameData);
        var ownedPokemonResult = new DbGetManyResult<OwnedPokemon>(new[] { ownedPokemon });
        var updatedGameData = new GameSaveData
        {
            Id = 1,
            GameSaveId = gameSaveId,
            GameData = newGameData.GameData,
        };
        var updateResult = new DbSaveResult<GameSaveData>(new[] { updatedGameData });

        _mockValidatorService
            .Setup(x =>
                x.ValidateAndThrowAsync(It.IsAny<GameSaveData>(), It.IsAny<CancellationToken>())
            )
            .Returns(Task.CompletedTask);

        _mockGameSessionRepository
            .Setup(x => x.GetOneWithGameSaveAndDataByConnectionIdAsync(connectionId))
            .ReturnsAsync(gameSessionResult);
        
        _mockOwnedPokemonRepository
            .Setup(x => x.GetMany(It.IsAny<Guid?[]>(), It.IsAny<string[]>()))
            .ReturnsAsync(ownedPokemonResult);

        _mockGameSaveDataRepository
            .Setup(x => x.Update(It.IsAny<GameSaveData>()))
            .ReturnsAsync(updateResult);

        // Act
        var result = await _command.ExecuteAsync(input);

        // Assert
        Assert.NotNull(result);
        Assert.NotNull(result.CommandResult);
        Assert.Equal(2, result.CommandResult.GameData.DeckPokemon.Count);

        _mockOwnedPokemonRepository.Verify(
            x => x.GetMany(It.IsAny<Guid?[]>(), It.IsAny<string[]>()),
            Times.Once
        );
    }

    [Fact]
    public async Task ExecuteAsync_Should_Throw_ValidationException_When_Validation_Fails()
    {
        // Arrange
        var userId = Guid.NewGuid();
        var connectionId = "test-connection-id";
        var user = new User
        {
            Id = userId,
            Email = _fixture.Create<string>(),
            Name = _fixture.Create<string>(),
        };

        var newGameData = new GameSaveData
        {
            GameSaveId = Guid.Empty,
            GameData = new GameSaveDataActual
            {
                LastPlayedScene = "Scene",
                LastPlayedLocationX = 10,
                LastPlayedLocationY = 20,
                DeckPokemon = [],
                UnlockedGameResources =  []
            },
        };

        var input = (newGameData, connectionId, user);

        var validationException = new ValidationException("Validation failed");

        _mockValidatorService
            .Setup(x =>
                x.ValidateAndThrowAsync(It.IsAny<GameSaveData>(), It.IsAny<CancellationToken>())
            )
            .ThrowsAsync(validationException);

        // Act & Assert
        var exception = await Assert.ThrowsAsync<ValidationException>(
            () => _command.ExecuteAsync(input)
        );

        Assert.Equal("Validation failed", exception.Message);
        _mockValidatorService.Verify(
            x => x.ValidateAndThrowAsync(It.IsAny<GameSaveData>(), It.IsAny<CancellationToken>()),
            Times.Once
        );
        _mockGameSessionRepository.Verify(
            x => x.GetOneWithGameSaveAndDataByConnectionIdAsync(It.IsAny<string>()),
            Times.Never
        );
    }

    [Fact]
    public async Task ExecuteAsync_Should_Throw_PokeGameApiServerException_When_GameSession_Fetch_Fails()
    {
        // Arrange
        var userId = Guid.NewGuid();
        var connectionId = "test-connection-id";
        var user = new User
        {
            Id = userId,
            Email = _fixture.Create<string>(),
            Name = _fixture.Create<string>(),
        };

        var newGameData = new GameSaveData
        {
            GameSaveId = Guid.Empty,
            GameData = new GameSaveDataActual
            {
                LastPlayedScene = "Scene",
                LastPlayedLocationX = 10,
                LastPlayedLocationY = 20,
                DeckPokemon = [],
                UnlockedGameResources =  []
            },
        };

        var input = (newGameData, connectionId, user);

        _mockValidatorService
            .Setup(x =>
                x.ValidateAndThrowAsync(It.IsAny<GameSaveData>(), It.IsAny<CancellationToken>())
            )
            .Returns(Task.CompletedTask);

        _mockGameSessionRepository
            .Setup(x => x.GetOneWithGameSaveAndDataByConnectionIdAsync(connectionId))
            .ReturnsAsync((DbGetOneResult<GameSession>)null!);

        // Act & Assert
        var exception = await Assert.ThrowsAsync<PokeGameApiServerException>(
            () => _command.ExecuteAsync(input)
        );

        Assert.Equal("Failed to fetch game session results", exception.Message);
        _mockGameSessionRepository.Verify(
            x => x.GetOneWithGameSaveAndDataByConnectionIdAsync(connectionId),
            Times.Once
        );
    }

    [Fact]
    public async Task ExecuteAsync_Should_Throw_PokeGameApiUserException_When_GameSession_Not_Found()
    {
        // Arrange
        var userId = Guid.NewGuid();
        var connectionId = "test-connection-id";
        var user = new User
        {
            Id = userId,
            Email = _fixture.Create<string>(),
            Name = _fixture.Create<string>(),
        };

        var newGameData = new GameSaveData
        {
            GameSaveId = Guid.Empty,
            GameData = new GameSaveDataActual
            {
                LastPlayedScene = "Scene",
                LastPlayedLocationX = 10,
                LastPlayedLocationY = 20,
                DeckPokemon = [],
                UnlockedGameResources =  []
            },
        };

        var input = (newGameData, connectionId, user);

        var gameSessionResult = new DbGetOneResult<GameSession>(null!) { IsSuccessful = true };

        _mockValidatorService
            .Setup(x =>
                x.ValidateAndThrowAsync(It.IsAny<GameSaveData>(), It.IsAny<CancellationToken>())
            )
            .Returns(Task.CompletedTask);

        _mockGameSessionRepository
            .Setup(x => x.GetOneWithGameSaveAndDataByConnectionIdAsync(connectionId))
            .ReturnsAsync(gameSessionResult);

        // Act & Assert
        var exception = await Assert.ThrowsAsync<PokeGameApiUserException>(
            () => _command.ExecuteAsync(input)
        );

        Assert.Equal(HttpStatusCode.BadRequest, exception.StatusCode);
        Assert.Equal("Failed to find game session for that connection id", exception.Message);
        _mockGameSessionRepository.Verify(
            x => x.GetOneWithGameSaveAndDataByConnectionIdAsync(connectionId),
            Times.Once
        );
    }

    [Fact]
    public async Task ExecuteAsync_Should_Throw_PokeGameApiUserException_When_GameSession_Belongs_To_Different_User()
    {
        // Arrange
        var userId = Guid.NewGuid();
        var differentUserId = Guid.NewGuid();
        var gameSaveId = Guid.NewGuid();
        var connectionId = "test-connection-id";

        var user = new User
        {
            Id = userId,
            Email = _fixture.Create<string>(),
            Name = _fixture.Create<string>(),
        };

        var newGameData = new GameSaveData
        {
            GameSaveId = Guid.Empty,
            GameData = new GameSaveDataActual
            {
                LastPlayedScene = "Scene",
                LastPlayedLocationX = 10,
                LastPlayedLocationY = 20,
                DeckPokemon = [],
                UnlockedGameResources =  []
            },
        };

        var gameSession = new GameSession
        {
            Id = Guid.NewGuid(),
            ConnectionId = connectionId,
            GameSaveId = gameSaveId,
            UserId =
                differentUserId // Different user
            ,
        };

        var input = (newGameData, connectionId, user);

        var gameSessionResult = new DbGetOneResult<GameSession>(gameSession);

        _mockValidatorService
            .Setup(x =>
                x.ValidateAndThrowAsync(It.IsAny<GameSaveData>(), It.IsAny<CancellationToken>())
            )
            .Returns(Task.CompletedTask);

        _mockGameSessionRepository
            .Setup(x => x.GetOneWithGameSaveAndDataByConnectionIdAsync(connectionId))
            .ReturnsAsync(gameSessionResult);

        // Act & Assert
        var exception = await Assert.ThrowsAsync<PokeGameApiUserException>(
            () => _command.ExecuteAsync(input)
        );

        Assert.Equal(HttpStatusCode.Unauthorized, exception.StatusCode);
        Assert.Equal("The current user is not the owner of the game session", exception.Message);
        _mockGameSessionRepository.Verify(
            x => x.GetOneWithGameSaveAndDataByConnectionIdAsync(connectionId),
            Times.Once
        );
    }

    [Fact]
    public async Task ExecuteAsync_Should_Throw_PokeGameApiUserException_When_New_Pokemon_Not_Found()
    {
        // Arrange
        var userId = Guid.NewGuid();
        var gameSaveId = Guid.NewGuid();
        var connectionId = "test-connection-id";
        var newPokemonId = Guid.NewGuid();

        var user = new User
        {
            Id = userId,
            Email = _fixture.Create<string>(),
            Name = _fixture.Create<string>(),
        };

        var existingGameData = new GameSaveData
        {
            Id = 1,
            GameSaveId = gameSaveId,
            GameData = new GameSaveDataActual
            {
                LastPlayedScene = "Scene",
                LastPlayedLocationX = 10,
                LastPlayedLocationY = 20,
                DeckPokemon = [],
                UnlockedGameResources =  []
            },
        };

        var newGameData = new GameSaveData
        {
            GameSaveId = Guid.Empty,
            GameData = new GameSaveDataActual
            {
                LastPlayedScene = "Scene",
                LastPlayedLocationX = 10,
                LastPlayedLocationY = 20,
                DeckPokemon = [new GameSaveDataActualDeckPokemon { OwnedPokemonId = newPokemonId }],
            },
        };

        var gameSession = new GameSession
        {
            Id = Guid.NewGuid(),
            ConnectionId = connectionId,
            GameSaveId = gameSaveId,
            UserId = userId,
        };

        var input = (newGameData, connectionId, user);

        var gameSessionResult = new DbGetOneResult<GameSession>(gameSession);
        var existingGameDataResult = new DbGetOneResult<GameSaveData>(existingGameData);
        var ownedPokemonResult = new DbGetManyResult<OwnedPokemon>(Array.Empty<OwnedPokemon>());

        _mockValidatorService
            .Setup(x =>
                x.ValidateAndThrowAsync(It.IsAny<GameSaveData>(), It.IsAny<CancellationToken>())
            )
            .Returns(Task.CompletedTask);

        _mockGameSessionRepository
            .Setup(x => x.GetOneWithGameSaveAndDataByConnectionIdAsync(connectionId))
            .ReturnsAsync(gameSessionResult);
        
        _mockOwnedPokemonRepository
            .Setup(x => x.GetMany(It.IsAny<Guid?[]>(), It.IsAny<string[]>()))
            .ReturnsAsync(ownedPokemonResult);

        // Act & Assert
        var exception = await Assert.ThrowsAsync<PokeGameApiUserException>(
            () => _command.ExecuteAsync(input)
        );

        Assert.Equal(HttpStatusCode.BadRequest, exception.StatusCode);
        Assert.Equal("Failed to find the pokemon you're adding to you deck", exception.Message);
        _mockOwnedPokemonRepository.Verify(
            x => x.GetMany(It.IsAny<Guid?[]>(), It.IsAny<string[]>()),
            Times.Once
        );
    }

    [Fact]
    public async Task ExecuteAsync_Should_Throw_PokeGameApiUserException_When_New_Pokemon_Belongs_To_Different_User()
    {
        // Arrange
        var userId = Guid.NewGuid();
        var differentUserId = Guid.NewGuid();
        var gameSaveId = Guid.NewGuid();
        var connectionId = "test-connection-id";
        var newPokemonId = Guid.NewGuid();

        var user = new User
        {
            Id = userId,
            Email = _fixture.Create<string>(),
            Name = _fixture.Create<string>(),
        };

        var existingGameData = new GameSaveData
        {
            Id = 1,
            GameSaveId = gameSaveId,
            GameData = new GameSaveDataActual
            {
                LastPlayedScene = "Scene",
                LastPlayedLocationX = 10,
                LastPlayedLocationY = 20,
                DeckPokemon = [],
                UnlockedGameResources =  []
            },
        };

        var newGameData = new GameSaveData
        {
            GameSaveId = Guid.Empty,
            GameData = new GameSaveDataActual
            {
                LastPlayedScene = "Scene",
                LastPlayedLocationX = 10,
                LastPlayedLocationY = 20,
                DeckPokemon = [new GameSaveDataActualDeckPokemon { OwnedPokemonId = newPokemonId }],
            },
        };

        var gameSession = new GameSession
        {
            Id = Guid.NewGuid(),
            ConnectionId = connectionId,
            GameSaveId = gameSaveId,
            UserId = userId,
        };

        var ownedPokemon = new OwnedPokemon
        {
            Id = newPokemonId,
            GameSaveId = Guid.NewGuid(),
            PokemonResourceName = "pikachu",
            PokemonLevel = 5,
            CurrentHp = 20,
            MoveOneResourceName = "tackle",
            GameSave = new GameSave
            {
                Id = Guid.NewGuid(),
                UserId = differentUserId,
                CharacterName = "Test",
            },
        };

        var input = (newGameData, connectionId, user);

        var gameSessionResult = new DbGetOneResult<GameSession>(gameSession);
        var existingGameDataResult = new DbGetOneResult<GameSaveData>(existingGameData);
        var ownedPokemonResult = new DbGetManyResult<OwnedPokemon>(new[] { ownedPokemon });

        _mockValidatorService
            .Setup(x =>
                x.ValidateAndThrowAsync(It.IsAny<GameSaveData>(), It.IsAny<CancellationToken>())
            )
            .Returns(Task.CompletedTask);

        _mockGameSessionRepository
            .Setup(x => x.GetOneWithGameSaveAndDataByConnectionIdAsync(connectionId))
            .ReturnsAsync(gameSessionResult);
        
        _mockOwnedPokemonRepository
            .Setup(x => x.GetMany(It.IsAny<Guid?[]>(), It.IsAny<string[]>()))
            .ReturnsAsync(ownedPokemonResult);

        // Act & Assert
        var exception = await Assert.ThrowsAsync<PokeGameApiUserException>(
            () => _command.ExecuteAsync(input)
        );

        Assert.Equal(HttpStatusCode.Unauthorized, exception.StatusCode);
        Assert.Equal("The pokemon in your deck have to belong to you", exception.Message);
        _mockOwnedPokemonRepository.Verify(
            x => x.GetMany(It.IsAny<Guid?[]>(), It.IsAny<string[]>()),
            Times.Once
        );
    }

    [Fact]
    public async Task ExecuteAsync_Should_Throw_PokeGameApiServerException_When_OwnedPokemon_Fetch_Fails()
    {
        // Arrange
        var userId = Guid.NewGuid();
        var gameSaveId = Guid.NewGuid();
        var connectionId = "test-connection-id";
        var newPokemonId = Guid.NewGuid();

        var user = new User
        {
            Id = userId,
            Email = _fixture.Create<string>(),
            Name = _fixture.Create<string>(),
        };

        var existingGameData = new GameSaveData
        {
            Id = 1,
            GameSaveId = gameSaveId,
            GameData = new GameSaveDataActual
            {
                LastPlayedScene = "Scene",
                LastPlayedLocationX = 10,
                LastPlayedLocationY = 20,
                DeckPokemon = [],
                UnlockedGameResources =  []
            },
        };

        var newGameData = new GameSaveData
        {
            GameSaveId = Guid.Empty,
            GameData = new GameSaveDataActual
            {
                LastPlayedScene = "Scene",
                LastPlayedLocationX = 10,
                LastPlayedLocationY = 20,
                DeckPokemon = [new GameSaveDataActualDeckPokemon { OwnedPokemonId = newPokemonId }],
            },
        };

        var gameSession = new GameSession
        {
            Id = Guid.NewGuid(),
            ConnectionId = connectionId,
            GameSaveId = gameSaveId,
            UserId = userId,
        };

        var input = (newGameData, connectionId, user);

        var gameSessionResult = new DbGetOneResult<GameSession>(gameSession);
        var existingGameDataResult = new DbGetOneResult<GameSaveData>(existingGameData);

        _mockValidatorService
            .Setup(x =>
                x.ValidateAndThrowAsync(It.IsAny<GameSaveData>(), It.IsAny<CancellationToken>())
            )
            .Returns(Task.CompletedTask);

        _mockGameSessionRepository
            .Setup(x => x.GetOneWithGameSaveAndDataByConnectionIdAsync(connectionId))
            .ReturnsAsync(gameSessionResult);
        
        _mockOwnedPokemonRepository
            .Setup(x => x.GetMany(It.IsAny<Guid?[]>(), It.IsAny<string[]>()))
            .ReturnsAsync((DbGetManyResult<OwnedPokemon>)null!);

        // Act & Assert
        var exception = await Assert.ThrowsAsync<PokeGameApiServerException>(
            () => _command.ExecuteAsync(input)
        );

        Assert.Equal("Failed to fetch owned pokemon", exception.Message);
        _mockOwnedPokemonRepository.Verify(
            x => x.GetMany(It.IsAny<Guid?[]>(), It.IsAny<string[]>()),
            Times.Once
        );
    }

    [Fact]
    public async Task ExecuteAsync_Should_Throw_PokeGameApiServerException_When_Update_Fails()
    {
        // Arrange
        var userId = Guid.NewGuid();
        var gameSaveId = Guid.NewGuid();
        var connectionId = "test-connection-id";

        var user = new User
        {
            Id = userId,
            Email = _fixture.Create<string>(),
            Name = _fixture.Create<string>(),
        };

        var existingGameData = new GameSaveData
        {
            Id = 1,
            GameSaveId = gameSaveId,
            GameData = new GameSaveDataActual
            {
                LastPlayedScene = "Scene",
                LastPlayedLocationX = 10,
                LastPlayedLocationY = 20,
                DeckPokemon = [],
                UnlockedGameResources =  []
            },
        };

        var newGameData = new GameSaveData
        {
            GameSaveId = Guid.Empty,
            GameData = new GameSaveDataActual
            {
                LastPlayedScene = "NewScene",
                LastPlayedLocationX = 30,
                LastPlayedLocationY = 40,
                DeckPokemon = [],
                UnlockedGameResources =  []
            },
        };

        var gameSession = new GameSession
        {
            Id = Guid.NewGuid(),
            ConnectionId = connectionId,
            GameSaveId = gameSaveId,
            UserId = userId,
        };

        var input = (newGameData, connectionId, user);

        var gameSessionResult = new DbGetOneResult<GameSession>(gameSession);
        var existingGameDataResult = new DbGetOneResult<GameSaveData>(existingGameData);

        _mockValidatorService
            .Setup(x =>
                x.ValidateAndThrowAsync(It.IsAny<GameSaveData>(), It.IsAny<CancellationToken>())
            )
            .Returns(Task.CompletedTask);

        _mockGameSessionRepository
            .Setup(x => x.GetOneWithGameSaveAndDataByConnectionIdAsync(connectionId))
            .ReturnsAsync(gameSessionResult);
        
        _mockGameSaveDataRepository
            .Setup(x => x.Update(It.IsAny<GameSaveData>()))
            .ReturnsAsync((DbSaveResult<GameSaveData>)null!);

        // Act & Assert
        var exception = await Assert.ThrowsAsync<PokeGameApiServerException>(
            () => _command.ExecuteAsync(input)
        );

        Assert.Equal("Failed to save game data", exception.Message);
        _mockGameSaveDataRepository.Verify(x => x.Update(It.IsAny<GameSaveData>()), Times.Once);
    }

    [Fact]
    public async Task ExecuteAsync_Should_Throw_PokeGameApiServerException_When_Update_Returns_Unsuccessful_Result()
    {
        // Arrange
        var userId = Guid.NewGuid();
        var gameSaveId = Guid.NewGuid();
        var connectionId = "test-connection-id";

        var user = new User
        {
            Id = userId,
            Email = _fixture.Create<string>(),
            Name = _fixture.Create<string>(),
        };

        var existingGameData = new GameSaveData
        {
            Id = 1,
            GameSaveId = gameSaveId,
            GameData = new GameSaveDataActual
            {
                LastPlayedScene = "Scene",
                LastPlayedLocationX = 10,
                LastPlayedLocationY = 20,
                DeckPokemon = [],
                UnlockedGameResources =  []
            },
        };

        var newGameData = new GameSaveData
        {
            GameSaveId = Guid.Empty,
            GameData = new GameSaveDataActual
            {
                LastPlayedScene = "NewScene",
                LastPlayedLocationX = 30,
                LastPlayedLocationY = 40,
                DeckPokemon = [],
                UnlockedGameResources =  []
            },
        };

        var gameSession = new GameSession
        {
            Id = Guid.NewGuid(),
            ConnectionId = connectionId,
            GameSaveId = gameSaveId,
            UserId = userId,
        };

        var input = (newGameData, connectionId, user);

        var gameSessionResult = new DbGetOneResult<GameSession>(gameSession);
        var existingGameDataResult = new DbGetOneResult<GameSaveData>(existingGameData);
        var updateResult = new DbSaveResult<GameSaveData>(Array.Empty<GameSaveData>())
        {
            IsSuccessful = false,
        };

        _mockValidatorService
            .Setup(x =>
                x.ValidateAndThrowAsync(It.IsAny<GameSaveData>(), It.IsAny<CancellationToken>())
            )
            .Returns(Task.CompletedTask);

        _mockGameSessionRepository
            .Setup(x => x.GetOneWithGameSaveAndDataByConnectionIdAsync(connectionId))
            .ReturnsAsync(gameSessionResult);
        
        _mockGameSaveDataRepository
            .Setup(x => x.Update(It.IsAny<GameSaveData>()))
            .ReturnsAsync(updateResult);

        // Act & Assert
        var exception = await Assert.ThrowsAsync<PokeGameApiServerException>(
            () => _command.ExecuteAsync(input)
        );

        Assert.Equal("Failed to save game data", exception.Message);
        _mockGameSaveDataRepository.Verify(x => x.Update(It.IsAny<GameSaveData>()), Times.Once);
    }

    [Fact]
    public void CommandName_Should_Return_Correct_Name()
    {
        // Act & Assert
        Assert.Equal(nameof(SaveGameDataCommand), _command.CommandName);
    }
}
