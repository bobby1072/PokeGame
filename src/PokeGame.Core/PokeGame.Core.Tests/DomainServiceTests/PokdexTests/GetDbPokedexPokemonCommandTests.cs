using AutoFixture;
using BT.Common.Persistence.Shared.Models;
using Microsoft.Extensions.Logging.Abstractions;
using Moq;
using PokeGame.Core.Common.Exceptions;
using PokeGame.Core.Domain.Services.Models;
using PokeGame.Core.Domain.Services.Pokedex.Commands;
using PokeGame.Core.Persistence.Repositories.Abstract;
using PokeGame.Core.Schemas.Input;
using PokeGame.Core.Schemas.Pokedex;
using System.Net;

namespace PokeGame.Core.Tests.DomainServiceTests.PokdexTests;

public sealed class GetDbPokedexPokemonCommandTests
{
    private static readonly Fixture _fixture = new();
    private readonly Mock<IPokedexPokemonRepository> _mockPokedexPokemonRepository = new();
    private readonly GetDbPokedexPokemonCommand _command;

    public GetDbPokedexPokemonCommandTests()
    {
        _command = new GetDbPokedexPokemonCommand(
            _mockPokedexPokemonRepository.Object,
            new NullLogger<GetDbPokedexPokemonCommand>()
        );
    }

    [Fact]
    public async Task ExecuteAsync_Should_Return_All_Pokemon_When_Input_Has_No_Properties()
    {
        // Arrange
        var input = new GetPokedexPokemonInput();
        var expectedPokemon = _fixture.CreateMany<PokedexPokemon>(5).ToArray();
        var dbResult = new DbGetManyResult<PokedexPokemon>(expectedPokemon) { IsSuccessful = true };

        _mockPokedexPokemonRepository
            .Setup(x => x.GetAll())
            .ReturnsAsync(dbResult);

        // Act
        var result = await _command.ExecuteAsync(input);

        // Assert
        Assert.NotNull(result);
        Assert.Equal(expectedPokemon, result.CommandResult);
        _mockPokedexPokemonRepository.Verify(x => x.GetAll(), Times.Once);
    }

    [Fact]
    public async Task ExecuteAsync_Should_Throw_PokeGameApiServerException_When_GetAll_Returns_Null()
    {
        // Arrange
        var input = new GetPokedexPokemonInput();

        _mockPokedexPokemonRepository
            .Setup(x => x.GetAll())
            .ReturnsAsync((DbGetManyResult<PokedexPokemon>)null!);

        // Act & Assert
        var exception = await Assert.ThrowsAsync<PokeGameApiServerException>(
            () => _command.ExecuteAsync(input)
        );

        Assert.Equal("Failed to fetch pokedex pokemon records", exception.Message);
        _mockPokedexPokemonRepository.Verify(x => x.GetAll(), Times.Once);
    }

    [Fact]
    public async Task ExecuteAsync_Should_Throw_PokeGameApiServerException_When_GetAll_Returns_Unsuccessful()
    {
        // Arrange
        var input = new GetPokedexPokemonInput();
        var dbResult = new DbGetManyResult<PokedexPokemon>(Array.Empty<PokedexPokemon>());

        _mockPokedexPokemonRepository
            .Setup(x => x.GetAll())
            .ReturnsAsync(dbResult);

        // Act & Assert
        var exception = await Assert.ThrowsAsync<PokeGameApiServerException>(
            () => _command.ExecuteAsync(input)
        );

        Assert.Equal("Failed to fetch pokedex pokemon records", exception.Message);
        _mockPokedexPokemonRepository.Verify(x => x.GetAll(), Times.Once);
    }

    [Fact]
    public async Task ExecuteAsync_Should_Return_Multiple_Pokemon_When_FetchMultiple_Is_True()
    {
        // Arrange
        var input = new GetPokedexPokemonInput { Id = 1, FetchMultiple = true };
        var expectedPokemon = _fixture.CreateMany<PokedexPokemon>(3).ToArray();
        var dbResult = new DbGetManyResult<PokedexPokemon>(expectedPokemon) { IsSuccessful = true };

        _mockPokedexPokemonRepository
            .Setup(x => x.GetMany(It.IsAny<Dictionary<string, object?>>()))
            .ReturnsAsync(dbResult);

        // Act
        var result = await _command.ExecuteAsync(input);

        // Assert
        Assert.NotNull(result);
        Assert.Equal(expectedPokemon, result.CommandResult);
        _mockPokedexPokemonRepository.Verify(x => x.GetMany(It.IsAny<Dictionary<string, object?>>()), Times.Once);
    }

    [Fact]
    public async Task ExecuteAsync_Should_Throw_PokeGameApiServerException_When_GetMany_Returns_Null()
    {
        // Arrange
        var input = new GetPokedexPokemonInput { Id = 1, FetchMultiple = true };

        _mockPokedexPokemonRepository
            .Setup(x => x.GetMany(It.IsAny<Dictionary<string, object?>>()))
            .ReturnsAsync((DbGetManyResult<PokedexPokemon>)null!);

        // Act & Assert
        var exception = await Assert.ThrowsAsync<PokeGameApiServerException>(
            () => _command.ExecuteAsync(input)
        );

        Assert.Equal("Failed to fetch pokedex pokemon records", exception.Message);
        _mockPokedexPokemonRepository.Verify(x => x.GetMany(It.IsAny<Dictionary<string, object?>>()), Times.Once);
    }

    [Fact]
    public async Task ExecuteAsync_Should_Throw_PokeGameApiUserException_When_GetMany_Returns_No_Data()
    {
        // Arrange
        var input = new GetPokedexPokemonInput { Id = 1, FetchMultiple = true };
        var dbResult = new DbGetManyResult<PokedexPokemon>(Array.Empty<PokedexPokemon>());

        _mockPokedexPokemonRepository
            .Setup(x => x.GetMany(It.IsAny<Dictionary<string, object?>>()))
            .ReturnsAsync(dbResult);

        // Act & Assert
        var exception = await Assert.ThrowsAsync<PokeGameApiUserException>(
            () => _command.ExecuteAsync(input)
        );

        Assert.Equal("Failed to fetch pokedex pokemon records", exception.Message);
        Assert.Equal(HttpStatusCode.NotFound, exception.StatusCode);
        _mockPokedexPokemonRepository.Verify(x => x.GetMany(It.IsAny<Dictionary<string, object?>>()), Times.Once);
    }

    [Fact]
    public async Task ExecuteAsync_Should_Return_Single_Pokemon_When_FetchMultiple_Is_False()
    {
        // Arrange
        var input = new GetPokedexPokemonInput { Id = 1, FetchMultiple = false };
        var expectedPokemon = _fixture.Create<PokedexPokemon>();
        var dbResult = new DbGetOneResult<PokedexPokemon>(expectedPokemon);

        _mockPokedexPokemonRepository
            .Setup(x => x.GetOne(It.IsAny<Dictionary<string, object?>>()))
            .ReturnsAsync(dbResult);

        // Act
        var result = await _command.ExecuteAsync(input);

        // Assert
        Assert.NotNull(result);
        Assert.Single(result.CommandResult);
        Assert.Equal(expectedPokemon, result.CommandResult.First());
        _mockPokedexPokemonRepository.Verify(x => x.GetOne(It.IsAny<Dictionary<string, object?>>()), Times.Once);
    }

    [Fact]
    public async Task ExecuteAsync_Should_Throw_PokeGameApiServerException_When_GetOne_Returns_Null()
    {
        // Arrange
        var input = new GetPokedexPokemonInput { Id = 1, FetchMultiple = false };

        _mockPokedexPokemonRepository
            .Setup(x => x.GetOne(It.IsAny<Dictionary<string, object?>>()))
            .ReturnsAsync((DbGetOneResult<PokedexPokemon>)null!);

        // Act & Assert
        var exception = await Assert.ThrowsAsync<PokeGameApiServerException>(
            () => _command.ExecuteAsync(input)
        );

        Assert.Equal("Failed to fetch pokedex pokemon records", exception.Message);
        _mockPokedexPokemonRepository.Verify(x => x.GetOne(It.IsAny<Dictionary<string, object?>>()), Times.Once);
    }

    [Fact]
    public async Task ExecuteAsync_Should_Throw_PokeGameApiUserException_When_GetOne_Returns_No_Data()
    {
        // Arrange
        var input = new GetPokedexPokemonInput { Id = 1, FetchMultiple = false };
        var dbResult = new DbGetOneResult<PokedexPokemon>(null!);

        _mockPokedexPokemonRepository
            .Setup(x => x.GetOne(It.IsAny<Dictionary<string, object?>>()))
            .ReturnsAsync(dbResult);

        // Act & Assert
        var exception = await Assert.ThrowsAsync<PokeGameApiUserException>(
            () => _command.ExecuteAsync(input)
        );

        Assert.Equal("Failed to fetch pokedex pokemon records", exception.Message);
        Assert.Equal(HttpStatusCode.NotFound, exception.StatusCode);
        _mockPokedexPokemonRepository.Verify(x => x.GetOne(It.IsAny<Dictionary<string, object?>>()), Times.Once);
    }

    [Fact]
    public void CommandName_Should_Return_Correct_Name()
    {
        // Act & Assert
        Assert.Equal(nameof(GetDbPokedexPokemonCommand), _command.CommandName);
    }
}