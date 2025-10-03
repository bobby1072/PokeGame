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

public sealed class GetUserByEmailCommandTests
{
    private static readonly Fixture _fixture = new();
    private readonly Mock<IUserRepository> _mockUserRepository = new();
    private readonly GetUserByEmailCommand _command;

    public GetUserByEmailCommandTests()
    {
        _command = new GetUserByEmailCommand(
            _mockUserRepository.Object,
            new NullLogger<GetUserByEmailCommand>()
        );
    }

    [Fact]
    public async Task ExecuteAsync_Should_Return_User_When_Valid_Email_And_User_Found()
    {
        // Arrange
        var email = "test@example.com";
        var expectedUser = _fixture.Create<User>();
        var dbResult = new DbGetOneResult<User>(expectedUser);

        _mockUserRepository
            .Setup(x => x.GetOne(email, "Email"))
            .ReturnsAsync(dbResult);

        // Act
        var result = await _command.ExecuteAsync(email);

        // Assert
        Assert.NotNull(result);
        Assert.Equal(expectedUser, result.CommandResult);
        _mockUserRepository.Verify(x => x.GetOne(email, "Email"), Times.Once);
    }

    [Theory]
    [InlineData("invalid-email")]
    [InlineData("test@")]
    [InlineData("@example.com")]
    [InlineData("")]
    public async Task ExecuteAsync_Should_Throw_PokeGameApiUserException_When_Invalid_Email(string invalidEmail)
    {
        // Act & Assert
        var exception = await Assert.ThrowsAsync<PokeGameApiUserException>(
            () => _command.ExecuteAsync(invalidEmail)
        );

        Assert.Equal("Invalid email", exception.Message);
        Assert.Equal(HttpStatusCode.BadRequest, exception.StatusCode);
        _mockUserRepository.Verify(x => x.GetOne(It.IsAny<string>(), It.IsAny<string>()), Times.Never);
    }

    [Fact]
    public async Task ExecuteAsync_Should_Throw_PokeGameApiServerException_When_Repository_Returns_Null()
    {
        // Arrange
        var email = "test@example.com";

        _mockUserRepository
            .Setup(x => x.GetOne(email, "Email"))
            .ReturnsAsync((DbGetOneResult<User>)null!);

        // Act & Assert
        var exception = await Assert.ThrowsAsync<PokeGameApiServerException>(
            () => _command.ExecuteAsync(email)
        );

        Assert.Equal("Failed to retrieve user", exception.Message);
        _mockUserRepository.Verify(x => x.GetOne(email, "Email"), Times.Once);
    }

    [Fact]
    public async Task ExecuteAsync_Should_Throw_PokeGameApiUserException_When_User_Not_Found()
    {
        // Arrange
        var email = "test@example.com";
        var dbResult = new DbGetOneResult<User>(null!);

        _mockUserRepository
            .Setup(x => x.GetOne(email, "Email"))
            .ReturnsAsync(dbResult);

        // Act & Assert
        var exception = await Assert.ThrowsAsync<PokeGameApiUserException>(
            () => _command.ExecuteAsync(email)
        );

        Assert.Equal("User not found", exception.Message);
        Assert.Equal(HttpStatusCode.NotFound, exception.StatusCode);
        _mockUserRepository.Verify(x => x.GetOne(email, "Email"), Times.Once);
    }

    [Fact]
    public void CommandName_Should_Return_Correct_Name()
    {
        // Act & Assert
        Assert.Equal(nameof(GetUserByEmailCommand), _command.CommandName);
    }
}