using AutoFixture;
using BT.Common.Persistence.Shared.Models;
using FluentValidation;
using Microsoft.Extensions.Logging.Abstractions;
using Moq;
using PokeGame.Core.Common.Configurations;
using PokeGame.Core.Common.Exceptions;
using PokeGame.Core.Domain.Services.Abstract;
using PokeGame.Core.Domain.Services.Game.Commands;
using PokeGame.Core.Domain.Services.Models;
using PokeGame.Core.Persistence.Repositories.Abstract;
using PokeGame.Core.Schemas.Game;

namespace PokeGame.Core.Tests.DomainServiceTests.GameTests;

public sealed class CreateNewGameCommandTests
{
    private static readonly Fixture _fixture = new();
    private readonly Mock<IGameSaveRepository> _mockGameSaveRepository = new();
    private readonly Mock<IValidatorService> _mockValidatorService = new();
    private readonly CreateNewGameCommand _command;

    public CreateNewGameCommandTests()
    {
        var pokeGameRules = new PokeGameRules
        {
            XpMultiplier = 1.052m,
            BaseXpCeiling = 100,
            LegendaryXpMultiplier = 1.055m,
            HpCalculationStats = new HpCalculationStats { DefaultIV = 31, DefaultEV = 0 },
            StandardPokemonPokedexRange = new PokedexRange { Min = 1, Max = 143 },
            LegendaryPokemonPokedexRange = new PokedexRange { Min = 144, Max = 151 },
            DefaultStarterScene = new DefaultStarterScene
            {
                SceneName = "BasiliaTownStarterHomeScene",
                SceneLocation = new DefaultStarterSceneLocation { X = 15, Y = 17 }
            }
        };
        
        _command = new CreateNewGameCommand(
            _mockGameSaveRepository.Object,
            _mockValidatorService.Object,
            pokeGameRules,
            new NullLogger<CreateNewGameCommand>()
        );
    }

    [Fact]
    public async Task ExecuteAsync_Should_Create_And_Return_GameSave_When_Valid_Input()
    {
        // Arrange
        var characterName = _fixture.Create<string>();
        var user = _fixture.Create<User>();
        var input = (characterName, user);
        
        var expectedGameSave = new GameSave
        {
            Id = Guid.NewGuid(),
            CharacterName = characterName,
            UserId = user.Id!.Value,
            LastPlayedScene = "BasiliaTownStarterHomeScene",
            LastPlayedLocationX = 15,
            LastPlayedLocationY = 17,
        };
        
        var dbResult = new DbSaveResult<GameSave>(new[] { expectedGameSave });

        _mockValidatorService
            .Setup(x => x.ValidateAndThrowAsync(It.IsAny<GameSave>(), It.IsAny<CancellationToken>()))
            .Returns(Task.CompletedTask);

        _mockGameSaveRepository
            .Setup(x => x.Create(It.IsAny<GameSave>()))
            .ReturnsAsync(dbResult);

        // Act
        var result = await _command.ExecuteAsync(input);

        // Assert
        Assert.NotNull(result);
        Assert.NotNull(result.CommandResult);
        Assert.Equal(characterName, result.CommandResult.CharacterName);
        Assert.Equal(user.Id, result.CommandResult.UserId);
        
        _mockValidatorService.Verify(x => x.ValidateAndThrowAsync(It.IsAny<GameSave>(), It.IsAny<CancellationToken>()), Times.Once);
        _mockGameSaveRepository.Verify(x => x.Create(It.IsAny<GameSave>()), Times.Once);
    }

    [Fact]
    public async Task ExecuteAsync_Should_Throw_ValidationException_When_Validation_Fails()
    {
        // Arrange
        var characterName = _fixture.Create<string>();
        var user = _fixture.Create<User>();
        var input = (characterName, user);
        
        var validationException = new ValidationException("Validation failed");

        _mockValidatorService
            .Setup(x => x.ValidateAndThrowAsync(It.IsAny<GameSave>(), It.IsAny<CancellationToken>()))
            .ThrowsAsync(validationException);

        // Act & Assert
        var exception = await Assert.ThrowsAsync<ValidationException>(
            () => _command.ExecuteAsync(input)
        );

        Assert.Equal("Validation failed", exception.Message);
        _mockValidatorService.Verify(x => x.ValidateAndThrowAsync(It.IsAny<GameSave>(), It.IsAny<CancellationToken>()), Times.Once);
        _mockGameSaveRepository.Verify(x => x.Create(It.IsAny<GameSave>()), Times.Never);
    }

    [Fact]
    public async Task ExecuteAsync_Should_Throw_PokeGameApiServerException_When_Repository_Returns_Null()
    {
        // Arrange
        var characterName = _fixture.Create<string>();
        var user = _fixture.Create<User>();
        var input = (characterName, user);

        _mockValidatorService
            .Setup(x => x.ValidateAndThrowAsync(It.IsAny<GameSave>(), It.IsAny<CancellationToken>()))
            .Returns(Task.CompletedTask);

        _mockGameSaveRepository
            .Setup(x => x.Create(It.IsAny<GameSave>()))
            .ReturnsAsync((DbSaveResult<GameSave>)null!);

        // Act & Assert
        var exception = await Assert.ThrowsAsync<PokeGameApiServerException>(
            () => _command.ExecuteAsync(input)
        );

        Assert.Equal("Failed to save game save", exception.Message);
        _mockValidatorService.Verify(x => x.ValidateAndThrowAsync(It.IsAny<GameSave>(), It.IsAny<CancellationToken>()), Times.Once);
        _mockGameSaveRepository.Verify(x => x.Create(It.IsAny<GameSave>()), Times.Once);
    }

    [Fact]
    public async Task ExecuteAsync_Should_Throw_PokeGameApiServerException_When_Repository_Returns_Unsuccessful_Result()
    {
        // Arrange
        var characterName = _fixture.Create<string>();
        var user = _fixture.Create<User>();
        var input = (characterName, user);
        
        var dbResult = new DbSaveResult<GameSave>(Array.Empty<GameSave>()) { IsSuccessful = false };

        _mockValidatorService
            .Setup(x => x.ValidateAndThrowAsync(It.IsAny<GameSave>(), It.IsAny<CancellationToken>()))
            .Returns(Task.CompletedTask);

        _mockGameSaveRepository
            .Setup(x => x.Create(It.IsAny<GameSave>()))
            .ReturnsAsync(dbResult);

        // Act & Assert
        var exception = await Assert.ThrowsAsync<PokeGameApiServerException>(
            () => _command.ExecuteAsync(input)
        );

        Assert.Equal("Failed to save game save", exception.Message);
        _mockValidatorService.Verify(x => x.ValidateAndThrowAsync(It.IsAny<GameSave>(), It.IsAny<CancellationToken>()), Times.Once);
        _mockGameSaveRepository.Verify(x => x.Create(It.IsAny<GameSave>()), Times.Once);
    }

    [Fact]
    public async Task ExecuteAsync_Should_Throw_PokeGameApiServerException_When_Repository_Returns_Empty_Data()
    {
        // Arrange
        var characterName = _fixture.Create<string>();
        var user = _fixture.Create<User>();
        var input = (characterName, user);
        
        var dbResult = new DbSaveResult<GameSave>(Array.Empty<GameSave>()) { IsSuccessful = true };

        _mockValidatorService
            .Setup(x => x.ValidateAndThrowAsync(It.IsAny<GameSave>(), It.IsAny<CancellationToken>()))
            .Returns(Task.CompletedTask);

        _mockGameSaveRepository
            .Setup(x => x.Create(It.IsAny<GameSave>()))
            .ReturnsAsync(dbResult);

        // Act & Assert
        var exception = await Assert.ThrowsAsync<PokeGameApiServerException>(
            () => _command.ExecuteAsync(input)
        );

        Assert.Equal("Failed to save game save", exception.Message);
        _mockValidatorService.Verify(x => x.ValidateAndThrowAsync(It.IsAny<GameSave>(), It.IsAny<CancellationToken>()), Times.Once);
        _mockGameSaveRepository.Verify(x => x.Create(It.IsAny<GameSave>()), Times.Once);
    }

    [Fact]
    public void CommandName_Should_Return_Correct_Name()
    {
        // Act & Assert
        Assert.Equal(nameof(CreateNewGameCommand), _command.CommandName);
    }
}