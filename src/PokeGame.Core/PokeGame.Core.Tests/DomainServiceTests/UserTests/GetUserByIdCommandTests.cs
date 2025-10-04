using AutoFixture;
using BT.Common.Persistence.Shared.Models;
using Microsoft.Extensions.Logging.Abstractions;
using Moq;
using PokeGame.Core.Common.Exceptions;
using PokeGame.Core.Domain.Services.Models;
using PokeGame.Core.Domain.Services.User.Commands;
using PokeGame.Core.Persistence.Repositories.Abstract;
using PokeGame.Core.Schemas.Game;
using System.Net;

namespace PokeGame.Core.Tests.DomainServiceTests.UserTests;

public sealed class GetUserByIdCommandTests
{
    private static readonly Fixture _fixture = new();
    private readonly Mock<IUserRepository> _mockUserRepository = new();
    private readonly GetUserByIdCommand _command;

    public GetUserByIdCommandTests()
    {
        _command = new GetUserByIdCommand(
            _mockUserRepository.Object,
            new NullLogger<GetUserByIdCommand>()
        );
    }

    [Fact]
    public async Task ExecuteAsync_Should_Return_User_When_Valid_Id_And_User_Found()
    {
        // Arrange
        var userId = Guid.NewGuid();
        var expectedUser = _fixture.Create<User>();
        var dbResult = new DbGetOneResult<User>(expectedUser);

        _mockUserRepository
            .Setup(x => x.GetOne(userId))
            .ReturnsAsync(dbResult);

        // Act
        var result = await _command.ExecuteAsync(userId);

        // Assert
        Assert.NotNull(result);
        Assert.Equal(expectedUser, result.CommandResult);
        _mockUserRepository.Verify(x => x.GetOne(userId), Times.Once);
    }

    [Fact]
    public async Task ExecuteAsync_Should_Throw_PokeGameApiServerException_When_Repository_Returns_Null()
    {
        // Arrange
        var userId = Guid.NewGuid();

        _mockUserRepository
            .Setup(x => x.GetOne(userId))
            .ReturnsAsync((DbGetOneResult<User>)null!);

        // Act & Assert
        var exception = await Assert.ThrowsAsync<PokeGameApiServerException>(
            () => _command.ExecuteAsync(userId)
        );

        Assert.Equal("Failed to retrieve user", exception.Message);
        _mockUserRepository.Verify(x => x.GetOne(userId), Times.Once);
    }

    [Fact]
    public async Task ExecuteAsync_Should_Throw_PokeGameApiUserException_When_User_Not_Found()
    {
        // Arrange
        var userId = Guid.NewGuid();
        var dbResult = new DbGetOneResult<User>(null!);

        _mockUserRepository
            .Setup(x => x.GetOne(userId))
            .ReturnsAsync(dbResult);

        // Act & Assert
        var exception = await Assert.ThrowsAsync<PokeGameApiUserException>(
            () => _command.ExecuteAsync(userId)
        );

        Assert.Equal("User not found", exception.Message);
        Assert.Equal(HttpStatusCode.NotFound, exception.StatusCode);
        _mockUserRepository.Verify(x => x.GetOne(userId), Times.Once);
    }

    [Fact]
    public void CommandName_Should_Return_Correct_Name()
    {
        // Act & Assert
        Assert.Equal(nameof(GetUserByIdCommand), _command.CommandName);
    }
}