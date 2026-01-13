using System.Net;
using AutoFixture;
using BT.Common.Persistence.Shared.Models;
using Microsoft.Extensions.Logging.Abstractions;
using Moq;
using PokeGame.Core.Common.Exceptions;
using PokeGame.Core.Domain.Services.Game.Abstract;
using PokeGame.Core.Domain.Services.Game.Commands;
using PokeGame.Core.Persistence.Repositories.Abstract;
using PokeGame.Core.Schemas.Game;
using PokeGame.Core.Schemas.Game.PokemonRelated;

namespace PokeGame.Core.Tests.DomainServiceTests.GameTests;

public sealed class GetOwnedPokemonByIdCommandTests
{
    private static readonly Fixture _fixture = new();
    private readonly Mock<IGameAndPokeApiResourceManagerService> _mockGameAndPokeApiResourceManagerService =
        new();
    private readonly Mock<IOwnedPokemonRepository> _mockOwnedPokemonRepository = new();
    private readonly GetOwnedPokemonByIdCommand _command;

    public GetOwnedPokemonByIdCommandTests()
    {
        _command = new GetOwnedPokemonByIdCommand(
            _mockGameAndPokeApiResourceManagerService.Object,
            _mockOwnedPokemonRepository.Object,
            new NullLogger<GetOwnedPokemonByIdCommand>()
        );
    }

    [Fact]
    public async Task ExecuteAsync_Should_Return_Shallow_OwnedPokemon_When_DeepVersion_Is_False()
    {
        // Arrange
        var userId = Guid.NewGuid();
        var gameSaveId = Guid.NewGuid();
        var ownedPokemonId = Guid.NewGuid();

        var user = new User
        {
            Id = userId,
            Email = _fixture.Create<string>(),
            Name = _fixture.Create<string>(),
        };

        var ownedPokemon = new OwnedPokemon
        {
            PokedexId = 1,
            Id = ownedPokemonId,
            GameSaveId = gameSaveId,
            PokemonResourceName = "pikachu",
            PokemonLevel = 5,
            CurrentHp = 20,
            CurrentExperience = 50,
            MoveOneResourceName = "tackle",
            GameSave = new GameSave
            {
                Id = gameSaveId,
                UserId = userId,
                CharacterName = "Test",
            },
        };

        var input = (DeepVersion: false, OwnedPokemonId: ownedPokemonId, CurrentUser: user);

        var dbResult = new DbGetOneResult<OwnedPokemon>(ownedPokemon);

        _mockOwnedPokemonRepository
            .Setup(x => x.GetOne(ownedPokemonId, It.IsAny<string[]>()))
            .ReturnsAsync(dbResult);

        // Act
        var result = await _command.ExecuteAsync(input);

        // Assert
        Assert.NotNull(result);
        Assert.NotNull(result.CommandResult);
        Assert.Equal(ownedPokemonId, result.CommandResult.Id);
        Assert.Equal(gameSaveId, result.CommandResult.GameSaveId);
        Assert.Equal("pikachu", result.CommandResult.PokemonResourceName);
        Assert.Equal(5, result.CommandResult.PokemonLevel);
        Assert.Equal(20, result.CommandResult.CurrentHp);
        Assert.Equal(50, result.CommandResult.CurrentExperience);

        _mockOwnedPokemonRepository.Verify(
            x => x.GetOne(ownedPokemonId, It.IsAny<string[]>()),
            Times.Once
        );
        _mockGameAndPokeApiResourceManagerService.Verify(
            x =>
                x.GetDeepOwnedPokemon(
                    It.IsAny<IReadOnlyCollection<OwnedPokemon>>(),
                    It.IsAny<CancellationToken>()
                ),
            Times.Never
        );
    }

    [Fact]
    public async Task ExecuteAsync_Should_Return_Deep_OwnedPokemon_When_DeepVersion_Is_True()
    {
        // Arrange
        var userId = Guid.NewGuid();
        var gameSaveId = Guid.NewGuid();
        var ownedPokemonId = Guid.NewGuid();

        var user = new User
        {
            Id = userId,
            Email = _fixture.Create<string>(),
            Name = _fixture.Create<string>(),
        };

        var ownedPokemon = new OwnedPokemon
        {
            PokedexId = 1,
            Id = ownedPokemonId,
            GameSaveId = gameSaveId,
            PokemonResourceName = "pikachu",
            PokemonLevel = 5,
            CurrentHp = 20,
            CurrentExperience = 50,
            MoveOneResourceName = "tackle",
            GameSave = new GameSave
            {
                Id = gameSaveId,
                UserId = userId,
                CharacterName = "Test",
            },
        };

        var deepOwnedPokemon = new OwnedPokemon
        {
            PokedexId = 1,
            Id = ownedPokemonId,
            GameSaveId = gameSaveId,
            PokemonResourceName = "pikachu",
            PokemonLevel = 5,
            CurrentHp = 20,
            CurrentExperience = 50,
            MoveOneResourceName = "tackle",
            Pokemon = _fixture.Create<Schemas.PokeApi.Pokemon>(),
            GameSave = new GameSave
            {
                Id = gameSaveId,
                UserId = userId,
                CharacterName = "Test",
            },
        };

        var input = (DeepVersion: true, OwnedPokemonId: ownedPokemonId, CurrentUser: user);

        var dbResult = new DbGetOneResult<OwnedPokemon>(ownedPokemon);

        _mockOwnedPokemonRepository
            .Setup(x => x.GetOne(ownedPokemonId, It.IsAny<string[]>()))
            .ReturnsAsync(dbResult);

        _mockGameAndPokeApiResourceManagerService
            .Setup(x =>
                x.GetDeepOwnedPokemon(
                    It.IsAny<IReadOnlyCollection<OwnedPokemon>>(),
                    It.IsAny<CancellationToken>()
                )
            )
            .ReturnsAsync(new[] { deepOwnedPokemon });

        // Act
        var result = await _command.ExecuteAsync(input);

        // Assert
        Assert.NotNull(result);
        Assert.NotNull(result.CommandResult);
        Assert.Equal(ownedPokemonId, result.CommandResult.Id);
        Assert.NotNull(result.CommandResult.Pokemon);

        _mockOwnedPokemonRepository.Verify(
            x => x.GetOne(ownedPokemonId, It.IsAny<string[]>()),
            Times.Once
        );
        _mockGameAndPokeApiResourceManagerService.Verify(
            x =>
                x.GetDeepOwnedPokemon(
                    It.IsAny<IReadOnlyCollection<OwnedPokemon>>(),
                    It.IsAny<CancellationToken>()
                ),
            Times.Once
        );
    }

    [Fact]
    public async Task ExecuteAsync_Should_Throw_PokeGameApiServerException_When_Repository_Returns_Null()
    {
        // Arrange
        var userId = Guid.NewGuid();
        var ownedPokemonId = Guid.NewGuid();

        var user = new User
        {
            Id = userId,
            Email = _fixture.Create<string>(),
            Name = _fixture.Create<string>(),
        };

        var input = (DeepVersion: false, OwnedPokemonId: ownedPokemonId, CurrentUser: user);

        _mockOwnedPokemonRepository
            .Setup(x => x.GetOne(ownedPokemonId, It.IsAny<string[]>()))
            .ReturnsAsync((DbGetOneResult<OwnedPokemon>)null!);

        // Act & Assert
        var exception = await Assert.ThrowsAsync<PokeGameApiServerException>(
            () => _command.ExecuteAsync(input)
        );

        Assert.Equal("Failed to fetch owned pokemon", exception.Message);
        _mockOwnedPokemonRepository.Verify(
            x => x.GetOne(ownedPokemonId, It.IsAny<string[]>()),
            Times.Once
        );
    }

    [Fact]
    public async Task ExecuteAsync_Should_Throw_PokeGameApiUserException_When_OwnedPokemon_Data_Is_Null()
    {
        // Arrange
        var userId = Guid.NewGuid();
        var ownedPokemonId = Guid.NewGuid();

        var user = new User
        {
            Id = userId,
            Email = _fixture.Create<string>(),
            Name = _fixture.Create<string>(),
        };

        var input = (DeepVersion: false, OwnedPokemonId: ownedPokemonId, CurrentUser: user);

        var dbResult = new DbGetOneResult<OwnedPokemon>(null!) { IsSuccessful = true };

        _mockOwnedPokemonRepository
            .Setup(x => x.GetOne(ownedPokemonId, It.IsAny<string[]>()))
            .ReturnsAsync(dbResult);

        // Act & Assert
        var exception = await Assert.ThrowsAsync<PokeGameApiUserException>(
            () => _command.ExecuteAsync(input)
        );

        Assert.Equal(HttpStatusCode.BadRequest, exception.StatusCode);
        Assert.Equal("Failed to fetch owned pokemon with that id", exception.Message);
        _mockOwnedPokemonRepository.Verify(
            x => x.GetOne(ownedPokemonId, It.IsAny<string[]>()),
            Times.Once
        );
    }

    [Fact]
    public async Task ExecuteAsync_Should_Throw_PokeGameApiUserException_When_Result_Is_Not_Successful()
    {
        // Arrange
        var userId = Guid.NewGuid();
        var ownedPokemonId = Guid.NewGuid();
        var gameSaveId = Guid.NewGuid();

        var user = new User
        {
            Id = userId,
            Email = _fixture.Create<string>(),
            Name = _fixture.Create<string>(),
        };

        var ownedPokemon = new OwnedPokemon
        {
            PokedexId = 1,
            Id = ownedPokemonId,
            GameSaveId = gameSaveId,
            PokemonResourceName = "pikachu",
            PokemonLevel = 5,
            CurrentHp = 20,
            CurrentExperience = 0,
            MoveOneResourceName = "tackle",
            GameSave = new GameSave
            {
                Id = gameSaveId,
                UserId = userId,
                CharacterName = "Test",
            },
        };

        var input = (DeepVersion: false, OwnedPokemonId: ownedPokemonId, CurrentUser: user);

        var dbResult = new DbGetOneResult<OwnedPokemon>(ownedPokemon) { IsSuccessful = false };

        _mockOwnedPokemonRepository
            .Setup(x => x.GetOne(ownedPokemonId, It.IsAny<string[]>()))
            .ReturnsAsync(dbResult);

        // Act & Assert
        var exception = await Assert.ThrowsAsync<PokeGameApiUserException>(
            () => _command.ExecuteAsync(input)
        );

        Assert.Equal(HttpStatusCode.BadRequest, exception.StatusCode);
        Assert.Equal("Failed to fetch owned pokemon with that id", exception.Message);
        _mockOwnedPokemonRepository.Verify(
            x => x.GetOne(ownedPokemonId, It.IsAny<string[]>()),
            Times.Once
        );
    }

    [Fact]
    public async Task ExecuteAsync_Should_Throw_PokeGameApiUserException_When_User_Does_Not_Own_Pokemon()
    {
        // Arrange
        var userId = Guid.NewGuid();
        var differentUserId = Guid.NewGuid();
        var ownedPokemonId = Guid.NewGuid();
        var gameSaveId = Guid.NewGuid();

        var user = new User
        {
            Id = userId,
            Email = _fixture.Create<string>(),
            Name = _fixture.Create<string>(),
        };

        var ownedPokemon = new OwnedPokemon
        {
            PokedexId = 1,
            Id = ownedPokemonId,
            GameSaveId = gameSaveId,
            PokemonResourceName = "pikachu",
            PokemonLevel = 5,
            CurrentHp = 20,
            MoveOneResourceName = "tackle",
            CurrentExperience = 0,
            GameSave = new GameSave
            {
                Id = gameSaveId,
                UserId = differentUserId, // Different user
                CharacterName = "Test",
            },
        };

        var input = (DeepVersion: false, OwnedPokemonId: ownedPokemonId, CurrentUser: user);

        var dbResult = new DbGetOneResult<OwnedPokemon>(ownedPokemon);

        _mockOwnedPokemonRepository
            .Setup(x => x.GetOne(ownedPokemonId, It.IsAny<string[]>()))
            .ReturnsAsync(dbResult);

        // Act & Assert
        var exception = await Assert.ThrowsAsync<PokeGameApiUserException>(
            () => _command.ExecuteAsync(input)
        );

        Assert.Equal(HttpStatusCode.Unauthorized, exception.StatusCode);
        Assert.Equal("User does not have permission to get owned pokemon", exception.Message);
        _mockOwnedPokemonRepository.Verify(
            x => x.GetOne(ownedPokemonId, It.IsAny<string[]>()),
            Times.Once
        );
    }

    [Fact]
    public async Task ExecuteAsync_Should_Throw_PokeGameApiUserException_When_GameSave_Is_Null()
    {
        // Arrange
        var userId = Guid.NewGuid();
        var ownedPokemonId = Guid.NewGuid();
        var gameSaveId = Guid.NewGuid();

        var user = new User
        {
            Id = userId,
            Email = _fixture.Create<string>(),
            Name = _fixture.Create<string>(),
        };

        var ownedPokemon = new OwnedPokemon
        {
            PokedexId = 1,
            Id = ownedPokemonId,
            GameSaveId = gameSaveId,
            PokemonResourceName = "pikachu",
            PokemonLevel = 5,
            CurrentHp = 20,
            CurrentExperience = 0,
            MoveOneResourceName = "tackle",
            GameSave = null, // Null GameSave
        };

        var input = (DeepVersion: false, OwnedPokemonId: ownedPokemonId, CurrentUser: user);

        var dbResult = new DbGetOneResult<OwnedPokemon>(ownedPokemon);

        _mockOwnedPokemonRepository
            .Setup(x => x.GetOne(ownedPokemonId, It.IsAny<string[]>()))
            .ReturnsAsync(dbResult);

        // Act & Assert
        var exception = await Assert.ThrowsAsync<PokeGameApiUserException>(
            () => _command.ExecuteAsync(input)
        );

        Assert.Equal(HttpStatusCode.Unauthorized, exception.StatusCode);
        Assert.Equal("User does not have permission to get owned pokemon", exception.Message);
        _mockOwnedPokemonRepository.Verify(
            x => x.GetOne(ownedPokemonId, It.IsAny<string[]>()),
            Times.Once
        );
    }

    [Fact]
    public void CommandName_Should_Return_Correct_Name()
    {
        // Act & Assert
        Assert.Equal(nameof(GetOwnedPokemonByIdCommand), _command.CommandName);
    }
}
