using AutoFixture;
using BT.Common.Persistence.Shared.Models;
using Microsoft.Extensions.Logging.Abstractions;
using Moq;
using PokeGame.Core.Common.Exceptions;
using PokeGame.Core.Domain.Services.Game.Abstract;
using PokeGame.Core.Domain.Services.Game.Concrete;
using PokeGame.Core.Persistence.Repositories.Abstract;
using PokeGame.Core.Schemas.Game;
using PokeGame.Core.Schemas.PokeApi;

namespace PokeGame.Core.Tests.DomainServiceTests.GameTests;

public sealed class GameAndPokeApiResourceManagerServiceTests
{
    private static readonly Fixture _fixture = new();
    private readonly Mock<IOwnedPokemonRepository> _mockOwnedPokemonRepository = new();
    private readonly Mock<IPokeApiClient> _mockPokeApiClient = new();
    private readonly GameAndPokeApiResourceManagerService _service;

    public GameAndPokeApiResourceManagerServiceTests()
    {
        _service = new GameAndPokeApiResourceManagerService(
            _mockOwnedPokemonRepository.Object,
            _mockPokeApiClient.Object,
            new NullLogger<GameAndPokeApiResourceManagerService>()
        );
    }

    [Fact]
    public async Task GetDeepOwnedPokemon_Should_Enrich_Pokemon_With_Api_Data()
    {
        // Arrange
        var ownedPokemon = new OwnedPokemon
        {
            Id = Guid.NewGuid(),
            GameSaveId = Guid.NewGuid(),
            PokemonResourceName = "pikachu",
            MoveOneResourceName = "thunderbolt",
            MoveTwoResourceName = "quick-attack",
            PokemonLevel = 25,
            CurrentHp = 50,
            CurrentExperience = 1000,
        };

        var pokemon = _fixture
            .Build<Pokemon>()
            .With(p => p.Id, 25)
            .With(p => p.Name, "pikachu")
            .Create();

        var species = _fixture
            .Build<PokemonSpecies>()
            .With(s => s.Id, 25)
            .With(s => s.Name, "pikachu")
            .Create();

        var moveOne = _fixture
            .Build<Move>()
            .With(m => m.Id, 85)
            .With(m => m.Name, "thunderbolt")
            .Create();

        var moveTwo = _fixture
            .Build<Move>()
            .With(m => m.Id, 98)
            .With(m => m.Name, "quick-attack")
            .Create();

        _mockPokeApiClient
            .Setup(x => x.GetResourceAsync<Pokemon>("pikachu", It.IsAny<CancellationToken>()))
            .ReturnsAsync(pokemon);

        _mockPokeApiClient
            .Setup(x =>
                x.GetResourceAsync<PokemonSpecies>("pikachu", It.IsAny<CancellationToken>())
            )
            .ReturnsAsync(species);

        _mockPokeApiClient
            .Setup(x => x.GetResourceAsync<Move>("thunderbolt", It.IsAny<CancellationToken>()))
            .ReturnsAsync(moveOne);

        _mockPokeApiClient
            .Setup(x => x.GetResourceAsync<Move>("quick-attack", It.IsAny<CancellationToken>()))
            .ReturnsAsync(moveTwo);

        // Act
        var result = await _service.GetDeepOwnedPokemon(new[] { ownedPokemon });

        // Assert
        Assert.NotNull(result);
        var enrichedPokemon = result.First();
        Assert.Equal(ownedPokemon.Id, enrichedPokemon.Id);
        Assert.NotNull(enrichedPokemon.Pokemon);
        Assert.Equal("pikachu", enrichedPokemon.Pokemon.Name);
        Assert.NotNull(enrichedPokemon.PokemonSpecies);
        Assert.Equal("pikachu", enrichedPokemon.PokemonSpecies.Name);
        Assert.NotNull(enrichedPokemon.MoveOne);
        Assert.Equal("thunderbolt", enrichedPokemon.MoveOne.Name);
        Assert.NotNull(enrichedPokemon.MoveTwo);
        Assert.Equal("quick-attack", enrichedPokemon.MoveTwo.Name);
    }

    [Fact]
    public async Task GetDeepOwnedPokemon_Should_Handle_Pokemon_With_Only_One_Move()
    {
        // Arrange
        var ownedPokemon = new OwnedPokemon
        {
            Id = Guid.NewGuid(),
            GameSaveId = Guid.NewGuid(),
            PokemonResourceName = "bulbasaur",
            MoveOneResourceName = "tackle",
            MoveTwoResourceName = null,
            MoveThreeResourceName = null,
            MoveFourResourceName = null,
            PokemonLevel = 5,
            CurrentHp = 20,
            CurrentExperience = 50,
        };

        var pokemon = _fixture
            .Build<Pokemon>()
            .With(p => p.Id, 1)
            .With(p => p.Name, "bulbasaur")
            .Create();
        var species = _fixture
            .Build<PokemonSpecies>()
            .With(s => s.Id, 1)
            .With(s => s.Name, "bulbasaur")
            .Create();
        var move = _fixture.Build<Move>().With(m => m.Id, 33).With(m => m.Name, "tackle").Create();

        _mockPokeApiClient
            .Setup(x => x.GetResourceAsync<Pokemon>("bulbasaur", It.IsAny<CancellationToken>()))
            .ReturnsAsync(pokemon);

        _mockPokeApiClient
            .Setup(x =>
                x.GetResourceAsync<PokemonSpecies>("bulbasaur", It.IsAny<CancellationToken>())
            )
            .ReturnsAsync(species);

        _mockPokeApiClient
            .Setup(x => x.GetResourceAsync<Move>("tackle", It.IsAny<CancellationToken>()))
            .ReturnsAsync(move);

        // Act
        var result = await _service.GetDeepOwnedPokemon(new[] { ownedPokemon });

        // Assert
        Assert.NotNull(result);
        var enrichedPokemon = result.First();
        Assert.NotNull(enrichedPokemon.MoveOne);
        Assert.Equal("tackle", enrichedPokemon.MoveOne.Name);
        Assert.Null(enrichedPokemon.MoveTwo);
        Assert.Null(enrichedPokemon.MoveThree);
        Assert.Null(enrichedPokemon.MoveFour);
    }

    [Fact]
    public async Task GetFullOwnedPokemon_Should_Fetch_From_Database_And_Enrich()
    {
        // Arrange
        var pokemonId = Guid.NewGuid();
        var ownedPokemon = new OwnedPokemon
        {
            Id = pokemonId,
            GameSaveId = Guid.NewGuid(),
            PokemonResourceName = "charmander",
            MoveOneResourceName = "ember",
            PokemonLevel = 10,
            CurrentHp = 30,
            CurrentExperience = 200,
        };

        var pokemon = _fixture
            .Build<Pokemon>()
            .With(p => p.Id, 4)
            .With(p => p.Name, "charmander")
            .Create();
        var species = _fixture
            .Build<PokemonSpecies>()
            .With(s => s.Id, 4)
            .With(s => s.Name, "charmander")
            .Create();
        var move = _fixture.Build<Move>().With(m => m.Id, 52).With(m => m.Name, "ember").Create();

        var dbResult = new DbGetManyResult<OwnedPokemon>(new[] { ownedPokemon });

        _mockOwnedPokemonRepository
            .Setup(x => x.GetMany(It.Is<Guid?[]>(ids => ids.Contains(pokemonId))))
            .ReturnsAsync(dbResult);

        _mockPokeApiClient
            .Setup(x => x.GetResourceAsync<Pokemon>("charmander", It.IsAny<CancellationToken>()))
            .ReturnsAsync(pokemon);

        _mockPokeApiClient
            .Setup(x =>
                x.GetResourceAsync<PokemonSpecies>("charmander", It.IsAny<CancellationToken>())
            )
            .ReturnsAsync(species);

        _mockPokeApiClient
            .Setup(x => x.GetResourceAsync<Move>("ember", It.IsAny<CancellationToken>()))
            .ReturnsAsync(move);

        // Act
        var result = await _service.GetFullOwnedPokemon(new[] { pokemonId });

        // Assert
        Assert.NotNull(result);
        Assert.Single(result);
        var enrichedPokemon = result.First();
        Assert.Equal(pokemonId, enrichedPokemon.Id);
        Assert.NotNull(enrichedPokemon.Pokemon);
        Assert.Equal("charmander", enrichedPokemon.Pokemon.Name);
        Assert.NotNull(enrichedPokemon.PokemonSpecies);
        Assert.NotNull(enrichedPokemon.MoveOne);
        Assert.Equal("ember", enrichedPokemon.MoveOne.Name);

        _mockOwnedPokemonRepository.Verify(
            x => x.GetMany(It.Is<Guid?[]>(ids => ids.Contains(pokemonId))),
            Times.Once
        );
    }
}
