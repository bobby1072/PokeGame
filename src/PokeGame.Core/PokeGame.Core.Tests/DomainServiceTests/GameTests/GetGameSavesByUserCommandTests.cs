using AutoFixture;
using BT.Common.Persistence.Shared.Models;
using Microsoft.Extensions.Logging.Abstractions;
using Moq;
using PokeGame.Core.Common.Exceptions;
using PokeGame.Core.Domain.Services.Game.Commands;
using PokeGame.Core.Domain.Services.Models;
using PokeGame.Core.Persistence.Entities;
using PokeGame.Core.Persistence.Repositories.Abstract;
using PokeGame.Core.Schemas.Game;

namespace PokeGame.Core.Tests.DomainServiceTests.GameTests;

public sealed class GetGameSavesByUserCommandTests
{
    private static readonly Fixture _fixture = new();
    private readonly Mock<IGameSaveRepository> _mockGameSaveRepository = new();
    private readonly GetGameSavesByUserCommand _command;

    public GetGameSavesByUserCommandTests()
    {
        _command = new GetGameSavesByUserCommand(
            new NullLogger<GetGameSavesByUserCommand>(),
            _mockGameSaveRepository.Object
        );
    }

    [Fact]
    public async Task ExecuteAsync_Should_Return_GameSaves_When_Repository_Returns_Data()
    {
        // Arrange
        var user = _fixture.Create<User>();
        var expectedGameSaves = _fixture.CreateMany<GameSave>(3).ToArray();
        var dbResult = new DbGetManyResult<GameSave>(expectedGameSaves);

        _mockGameSaveRepository
            .Setup(x => x.GetMany<Guid>((Guid)user.Id!, nameof(GameSaveEntity.UserId)))
            .ReturnsAsync(dbResult);

        // Act
        var result = await _command.ExecuteAsync(user);

        // Assert
        Assert.NotNull(result);
        Assert.Equal(expectedGameSaves, result.CommandResult);
        _mockGameSaveRepository.Verify(x => x.GetMany<Guid>((Guid)user.Id!, nameof(GameSaveEntity.UserId)), Times.Once);
    }

    [Fact]
    public async Task ExecuteAsync_Should_Throw_PokeGameApiServerException_When_Repository_Returns_Null()
    {
        // Arrange
        var user = _fixture.Create<User>();

        _mockGameSaveRepository
            .Setup(x => x.GetMany<Guid>((Guid)user.Id!, nameof(GameSaveEntity.UserId)))
            .ReturnsAsync((DbGetManyResult<GameSave>)null!);

        // Act & Assert
        var exception = await Assert.ThrowsAsync<PokeGameApiServerException>(
            () => _command.ExecuteAsync(user)
        );

        Assert.Equal("Failed to fetch game saves", exception.Message);
        _mockGameSaveRepository.Verify(x => x.GetMany<Guid>((Guid)user.Id!, nameof(GameSaveEntity.UserId)), Times.Once);
    }

    [Fact]
    public async Task ExecuteAsync_Should_Return_Empty_Collection_When_No_GameSaves_Found()
    {
        // Arrange
        var user = _fixture.Create<User>();
        var emptyResult = new DbGetManyResult<GameSave>(Array.Empty<GameSave>());

        _mockGameSaveRepository
            .Setup(x => x.GetMany<Guid>((Guid)user.Id!, nameof(GameSaveEntity.UserId)))
            .ReturnsAsync(emptyResult);

        // Act
        var result = await _command.ExecuteAsync(user);

        // Assert
        Assert.NotNull(result);
        Assert.Empty(result.CommandResult);
        _mockGameSaveRepository.Verify(x => x.GetMany<Guid>((Guid)user.Id!, nameof(GameSaveEntity.UserId)), Times.Once);
    }

    [Fact]
    public void CommandName_Should_Return_Correct_Name()
    {
        // Act & Assert
        Assert.Equal(nameof(GetGameSavesByUserCommand), _command.CommandName);
    }
}