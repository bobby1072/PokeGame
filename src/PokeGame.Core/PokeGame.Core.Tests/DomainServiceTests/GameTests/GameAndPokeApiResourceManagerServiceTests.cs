using AutoFixture;
using BT.Common.Persistence.Shared.Models;
using Microsoft.Extensions.Logging.Abstractions;
using Moq;
using PokeGame.Core.Common.Exceptions;
using PokeGame.Core.Domain.Services.Game.Abstract;
using PokeGame.Core.Domain.Services.Game.Concrete;
using PokeGame.Core.Persistence.Repositories.Abstract;
using PokeGame.Core.Schemas.Game;
using PokeGame.Core.Schemas.Game.PokemonRelated;
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
            PokedexId = 1,
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
            PokedexId = 1,
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
            PokedexId = 1,
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

    [Fact]
    public async Task GetMoveSet_Should_Return_Single_Move_When_Only_First_Move_Provided()
    {
        // Arrange
        var moveOne = CreateTestMove("tackle", 40, 100);

        _mockPokeApiClient
            .Setup(x => x.GetResourceAsync<Move>("tackle", It.IsAny<CancellationToken>()))
            .ReturnsAsync(moveOne);

        // Act
        var result = await _service.GetMoveSet("tackle", null, null, null);

        // Assert
        Assert.NotNull(result.MoveOne);
        Assert.Equal("tackle", result.MoveOne.Name);
        Assert.Null(result.MoveTwo);
        Assert.Null(result.MoveThree);
        Assert.Null(result.MoveFour);

        _mockPokeApiClient.Verify(
            x => x.GetResourceAsync<Move>("tackle", It.IsAny<CancellationToken>()),
            Times.Once
        );
    }

    [Fact]
    public async Task GetMoveSet_Should_Return_All_Four_Moves_When_All_Provided()
    {
        // Arrange
        var moveOne = CreateTestMove("tackle", 40, 100);
        var moveTwo = CreateTestMove("thunderbolt", 90, 100);
        var moveThree = CreateTestMove("quick-attack", 40, 100);
        var moveFour = CreateTestMove("thunder-wave", 0, 90);

        _mockPokeApiClient
            .Setup(x => x.GetResourceAsync<Move>("tackle", It.IsAny<CancellationToken>()))
            .ReturnsAsync(moveOne);

        _mockPokeApiClient
            .Setup(x => x.GetResourceAsync<Move>("thunderbolt", It.IsAny<CancellationToken>()))
            .ReturnsAsync(moveTwo);

        _mockPokeApiClient
            .Setup(x => x.GetResourceAsync<Move>("quick-attack", It.IsAny<CancellationToken>()))
            .ReturnsAsync(moveThree);

        _mockPokeApiClient
            .Setup(x => x.GetResourceAsync<Move>("thunder-wave", It.IsAny<CancellationToken>()))
            .ReturnsAsync(moveFour);

        // Act
        var result = await _service.GetMoveSet(
            "tackle",
            "thunderbolt",
            "quick-attack",
            "thunder-wave"
        );

        // Assert
        Assert.NotNull(result.MoveOne);
        Assert.Equal("tackle", result.MoveOne.Name);
        Assert.NotNull(result.MoveTwo);
        Assert.Equal("thunderbolt", result.MoveTwo.Name);
        Assert.NotNull(result.MoveThree);
        Assert.Equal("quick-attack", result.MoveThree.Name);
        Assert.NotNull(result.MoveFour);
        Assert.Equal("thunder-wave", result.MoveFour.Name);
    }

    [Fact]
    public async Task GetMoveSet_Should_Handle_Empty_String_Move_Names()
    {
        // Arrange
        var moveOne = CreateTestMove("tackle", 40, 100);

        _mockPokeApiClient
            .Setup(x => x.GetResourceAsync<Move>("tackle", It.IsAny<CancellationToken>()))
            .ReturnsAsync(moveOne);

        // Act
        var result = await _service.GetMoveSet("tackle", "", "  ", null);

        // Assert
        Assert.NotNull(result.MoveOne);
        Assert.Equal("tackle", result.MoveOne.Name);
        Assert.Null(result.MoveTwo);
        Assert.Null(result.MoveThree);
        Assert.Null(result.MoveFour);

        _mockPokeApiClient.Verify(
            x => x.GetResourceAsync<Move>("tackle", It.IsAny<CancellationToken>()),
            Times.Once
        );
        _mockPokeApiClient.Verify(
            x => x.GetResourceAsync<Move>("", It.IsAny<CancellationToken>()),
            Times.Never
        );
    }

    [Fact]
    public async Task GetMoveSet_Should_Throw_PokeGameApiServerException_When_Api_Fails()
    {
        // Arrange
        _mockPokeApiClient
            .Setup(x => x.GetResourceAsync<Move>("tackle", It.IsAny<CancellationToken>()))
            .ThrowsAsync(new Exception("API error"));

        // Act & Assert
        var exception = await Assert.ThrowsAsync<PokeGameApiServerException>(
            () => _service.GetMoveSet("tackle", null, null, null)
        );

        Assert.Equal("Failed to fetch owned pokemon resources", exception.Message);
    }

    [Fact]
    public async Task GetMoveSet_Should_Rethrow_PokeGameApiException()
    {
        // Arrange
        var originalException = new PokeGameApiUserException(
            System.Net.HttpStatusCode.BadRequest,
            "Invalid move name"
        );

        _mockPokeApiClient
            .Setup(x => x.GetResourceAsync<Move>("tackle", It.IsAny<CancellationToken>()))
            .ThrowsAsync(originalException);

        // Act & Assert
        var exception = await Assert.ThrowsAsync<PokeGameApiUserException>(
            () => _service.GetMoveSet("tackle", null, null, null)
        );

        Assert.Equal(originalException, exception);
    }

    [Fact]
    public async Task GetDeepOwnedPokemon_Should_Handle_Multiple_Pokemon()
    {
        // Arrange
        var ownedPokemon1 = new OwnedPokemon
        {
            PokedexId = 1,
            Id = Guid.NewGuid(),
            GameSaveId = Guid.NewGuid(),
            PokemonResourceName = "pikachu",
            MoveOneResourceName = "thunderbolt",
            PokemonLevel = 25,
            CurrentHp = 50,
            CurrentExperience = 1000,
        };

        var ownedPokemon2 = new OwnedPokemon
        {
            PokedexId = 1,
            Id = Guid.NewGuid(),
            GameSaveId = Guid.NewGuid(),
            PokemonResourceName = "charizard",
            MoveOneResourceName = "flamethrower",
            PokemonLevel = 36,
            CurrentHp = 80,
            CurrentExperience = 5000,
        };

        var pokemon1 = CreateTestPokemon(25, "pikachu");
        var species1 = CreateTestPokemonSpecies(25, "pikachu");
        var move1 = CreateTestMove("thunderbolt", 90, 100);

        var pokemon2 = CreateTestPokemon(6, "charizard");
        var species2 = CreateTestPokemonSpecies(6, "charizard");
        var move2 = CreateTestMove("flamethrower", 90, 100);

        _mockPokeApiClient
            .Setup(x => x.GetResourceAsync<Pokemon>("pikachu", It.IsAny<CancellationToken>()))
            .ReturnsAsync(pokemon1);

        _mockPokeApiClient
            .Setup(x =>
                x.GetResourceAsync<PokemonSpecies>("pikachu", It.IsAny<CancellationToken>())
            )
            .ReturnsAsync(species1);

        _mockPokeApiClient
            .Setup(x => x.GetResourceAsync<Move>("thunderbolt", It.IsAny<CancellationToken>()))
            .ReturnsAsync(move1);

        _mockPokeApiClient
            .Setup(x => x.GetResourceAsync<Pokemon>("charizard", It.IsAny<CancellationToken>()))
            .ReturnsAsync(pokemon2);

        _mockPokeApiClient
            .Setup(x =>
                x.GetResourceAsync<PokemonSpecies>("charizard", It.IsAny<CancellationToken>())
            )
            .ReturnsAsync(species2);

        _mockPokeApiClient
            .Setup(x => x.GetResourceAsync<Move>("flamethrower", It.IsAny<CancellationToken>()))
            .ReturnsAsync(move2);

        // Act
        var result = await _service.GetDeepOwnedPokemon(new[] { ownedPokemon1, ownedPokemon2 });

        // Assert
        Assert.NotNull(result);
        Assert.Equal(2, result.Count);

        var enrichedPokemon1 = result.First(p => p.Id == ownedPokemon1.Id);
        Assert.Equal("pikachu", enrichedPokemon1.Pokemon?.Name);
        Assert.Equal("thunderbolt", enrichedPokemon1.MoveOne?.Name);

        var enrichedPokemon2 = result.First(p => p.Id == ownedPokemon2.Id);
        Assert.Equal("charizard", enrichedPokemon2.Pokemon?.Name);
        Assert.Equal("flamethrower", enrichedPokemon2.MoveOne?.Name);
    }

    [Fact]
    public async Task GetDeepOwnedPokemon_Should_Throw_PokeGameApiServerException_When_Api_Fails()
    {
        // Arrange
        var ownedPokemon = new OwnedPokemon
        {
            PokedexId = 1,
            Id = Guid.NewGuid(),
            GameSaveId = Guid.NewGuid(),
            PokemonResourceName = "pikachu",
            MoveOneResourceName = "thunderbolt",
            PokemonLevel = 25,
            CurrentHp = 50,
            CurrentExperience = 1000,
        };

        _mockPokeApiClient
            .Setup(x => x.GetResourceAsync<Pokemon>("pikachu", It.IsAny<CancellationToken>()))
            .ThrowsAsync(new Exception("API error"));

        // Act & Assert
        var exception = await Assert.ThrowsAsync<PokeGameApiServerException>(
            () => _service.GetDeepOwnedPokemon(new[] { ownedPokemon })
        );

        Assert.Equal("Failed to fetch pokemon resources", exception.Message);
    }

    [Fact]
    public async Task GetDeepOwnedPokemon_Should_Rethrow_PokeGameApiException()
    {
        // Arrange
        var ownedPokemon = new OwnedPokemon
        {
            PokedexId = 1,
            Id = Guid.NewGuid(),
            GameSaveId = Guid.NewGuid(),
            PokemonResourceName = "pikachu",
            MoveOneResourceName = "thunderbolt",
            PokemonLevel = 25,
            CurrentHp = 50,
            CurrentExperience = 1000,
        };

        var originalException = new PokeGameApiUserException(
            System.Net.HttpStatusCode.BadRequest,
            "Invalid pokemon name"
        );

        _mockPokeApiClient
            .Setup(x => x.GetResourceAsync<Pokemon>("pikachu", It.IsAny<CancellationToken>()))
            .ThrowsAsync(originalException);

        // Act & Assert
        var exception = await Assert.ThrowsAsync<PokeGameApiUserException>(
            () => _service.GetDeepOwnedPokemon(new[] { ownedPokemon })
        );

        Assert.Equal(originalException, exception);
    }

    [Fact]
    public async Task GetFullOwnedPokemon_Should_Throw_PokeGameApiServerException_When_Database_Returns_Null()
    {
        // Arrange
        var pokemonId = Guid.NewGuid();

        _mockOwnedPokemonRepository
            .Setup(x => x.GetMany(It.IsAny<Guid?[]>()))
            .ReturnsAsync((DbGetManyResult<OwnedPokemon>)null!);

        // Act & Assert
        var exception = await Assert.ThrowsAsync<PokeGameApiServerException>(
            () => _service.GetFullOwnedPokemon(new[] { pokemonId })
        );

        Assert.Equal("Failed to fetch owned pokemon from database", exception.Message);
    }

    [Fact]
    public async Task GetFullOwnedPokemon_Should_Throw_PokeGameApiServerException_When_Database_Returns_Unsuccessful_Result()
    {
        // Arrange
        var pokemonId = Guid.NewGuid();
        var dbResult = new DbGetManyResult<OwnedPokemon>();

        _mockOwnedPokemonRepository
            .Setup(x => x.GetMany(It.IsAny<Guid?[]>()))
            .ReturnsAsync(dbResult);

        // Act & Assert
        var exception = await Assert.ThrowsAsync<PokeGameApiServerException>(
            () => _service.GetFullOwnedPokemon(new[] { pokemonId })
        );

        Assert.Equal("Pokemon could not be retrieved from database", exception.Message);
    }

    [Fact]
    public async Task GetFullOwnedPokemon_Should_Throw_PokeGameApiServerException_When_Database_Returns_Empty_Collection()
    {
        // Arrange
        var pokemonId = Guid.NewGuid();
        var dbResult = new DbGetManyResult<OwnedPokemon>(Array.Empty<OwnedPokemon>());

        _mockOwnedPokemonRepository
            .Setup(x => x.GetMany(It.IsAny<Guid?[]>()))
            .ReturnsAsync(dbResult);

        // Act & Assert
        var exception = await Assert.ThrowsAsync<PokeGameApiServerException>(
            () => _service.GetFullOwnedPokemon(new[] { pokemonId })
        );

        Assert.Equal("Pokemon could not be retrieved from database", exception.Message);
    }

    [Fact]
    public async Task GetFullOwnedPokemon_Should_Handle_Multiple_Pokemon_Ids()
    {
        // Arrange
        var pokemonId1 = Guid.NewGuid();
        var pokemonId2 = Guid.NewGuid();

        var ownedPokemon1 = new OwnedPokemon
        {
            PokedexId = 1,
            Id = pokemonId1,
            GameSaveId = Guid.NewGuid(),
            PokemonResourceName = "pikachu",
            MoveOneResourceName = "thunderbolt",
            PokemonLevel = 25,
            CurrentHp = 50,
            CurrentExperience = 1000,
        };

        var ownedPokemon2 = new OwnedPokemon
        {
            PokedexId = 1,
            Id = pokemonId2,
            GameSaveId = Guid.NewGuid(),
            PokemonResourceName = "charizard",
            MoveOneResourceName = "flamethrower",
            PokemonLevel = 36,
            CurrentHp = 80,
            CurrentExperience = 5000,
        };

        var dbResult = new DbGetManyResult<OwnedPokemon>(new[] { ownedPokemon1, ownedPokemon2 });

        var pokemon1 = CreateTestPokemon(25, "pikachu");
        var species1 = CreateTestPokemonSpecies(25, "pikachu");
        var move1 = CreateTestMove("thunderbolt", 90, 100);

        var pokemon2 = CreateTestPokemon(6, "charizard");
        var species2 = CreateTestPokemonSpecies(6, "charizard");
        var move2 = CreateTestMove("flamethrower", 90, 100);

        _mockOwnedPokemonRepository
            .Setup(x =>
                x.GetMany(
                    It.Is<Guid?[]>(ids => ids.Contains(pokemonId1) && ids.Contains(pokemonId2))
                )
            )
            .ReturnsAsync(dbResult);

        _mockPokeApiClient
            .Setup(x => x.GetResourceAsync<Pokemon>("pikachu", It.IsAny<CancellationToken>()))
            .ReturnsAsync(pokemon1);

        _mockPokeApiClient
            .Setup(x =>
                x.GetResourceAsync<PokemonSpecies>("pikachu", It.IsAny<CancellationToken>())
            )
            .ReturnsAsync(species1);

        _mockPokeApiClient
            .Setup(x => x.GetResourceAsync<Move>("thunderbolt", It.IsAny<CancellationToken>()))
            .ReturnsAsync(move1);

        _mockPokeApiClient
            .Setup(x => x.GetResourceAsync<Pokemon>("charizard", It.IsAny<CancellationToken>()))
            .ReturnsAsync(pokemon2);

        _mockPokeApiClient
            .Setup(x =>
                x.GetResourceAsync<PokemonSpecies>("charizard", It.IsAny<CancellationToken>())
            )
            .ReturnsAsync(species2);

        _mockPokeApiClient
            .Setup(x => x.GetResourceAsync<Move>("flamethrower", It.IsAny<CancellationToken>()))
            .ReturnsAsync(move2);

        // Act
        var result = await _service.GetFullOwnedPokemon(new[] { pokemonId1, pokemonId2 });

        // Assert
        Assert.NotNull(result);
        Assert.Equal(2, result.Count);
        Assert.Contains(result, p => p.Id == pokemonId1 && p.Pokemon?.Name == "pikachu");
        Assert.Contains(result, p => p.Id == pokemonId2 && p.Pokemon?.Name == "charizard");
    }

    [Fact]
    public async Task GetFullOwnedPokemon_Should_Throw_PokeGameApiServerException_When_Repository_Throws()
    {
        // Arrange
        var pokemonId = Guid.NewGuid();

        var originalException = new PokeGameApiUserException(
            System.Net.HttpStatusCode.BadRequest,
            "Invalid request"
        );

        _mockOwnedPokemonRepository
            .Setup(x => x.GetMany(It.IsAny<Guid?[]>()))
            .ThrowsAsync(originalException);

        // Act & Assert
        var exception = await Assert.ThrowsAsync<PokeGameApiServerException>(
            () => _service.GetFullOwnedPokemon(new[] { pokemonId })
        );

        Assert.Equal("Failed to fetch owned pokemon from database", exception.Message);
    }

    [Fact]
    public async Task GetDeepOwnedPokemon_Should_Handle_Empty_Collection()
    {
        // Arrange
        var emptyCollection = Array.Empty<OwnedPokemon>();

        // Act
        var result = await _service.GetDeepOwnedPokemon(emptyCollection);

        // Assert
        Assert.NotNull(result);
        Assert.Empty(result);

        _mockPokeApiClient.Verify(
            x => x.GetResourceAsync<Pokemon>(It.IsAny<string>(), It.IsAny<CancellationToken>()),
            Times.Never
        );
    }

    // Helper methods
    private static Pokemon CreateTestPokemon(int id, string name)
    {
        return new Pokemon
        {
            Id = id,
            Name = name,
            Height = 4,
            Weight = 60,
            BaseExperienceFromDefeating = 112,
            Sprites = new PokemonSprites
            {
                FrontDefault = "front",
                FrontShiny = "front-shiny",
                BackDefault = "back",
                BackShiny = "back-shiny",
            },
            Types = new List<PokemonType>(),
            Stats = new List<PokemonStat>(),
            Moves = new List<PokemonMove>(),
            Abilities = new List<PokemonAbility>(),
            Forms = new List<NamedApiResource<PokemonForm>>(),
            GameIndicies = new List<VersionGameIndex>(),
            HeldItems = new List<PokemonHeldItem>(),
            LocationAreaEncounters = string.Empty,
            PastTypes = new List<PokemonPastTypes>(),
            Species = new NamedApiResource<PokemonSpecies>
            {
                Name = string.Empty,
                Url = string.Empty,
            },
        };
    }

    private static PokemonSpecies CreateTestPokemonSpecies(int id, string name)
    {
        return _fixture
            .Build<PokemonSpecies>()
            .With(s => s.Id, id)
            .With(s => s.Name, name)
            .Create();
    }

    private static Move CreateTestMove(string name, int? power, int? accuracy)
    {
        return new Move
        {
            Name = name,
            Power = power,
            Accuracy = accuracy,
            DamageClass = new NamedApiResource<MoveDamageClass>
            {
                Name = string.Empty,
                Url = string.Empty,
            },
            EffectChanges = new List<AbilityEffectChange>(),
            FlavorTextEntries = new List<MoveFlavorText>(),
            Generation = new NamedApiResource<Generation>
            {
                Name = string.Empty,
                Url = string.Empty,
            },
            LearnedByPokemon = new List<NamedApiResource<Pokemon>>(),
            Machines = new List<MachineVersionDetail>(),
            Meta = new MoveMetaData
            {
                Ailment = new NamedApiResource<MoveAilment>
                {
                    Name = string.Empty,
                    Url = string.Empty,
                },
                Category = new NamedApiResource<MoveCategory>
                {
                    Name = string.Empty,
                    Url = string.Empty,
                },
            },
            Names = new List<Names>(),
            PastValues = new List<PastMoveStatValues>(),
            StatChanges = new List<MoveStatChange>(),
            Target = new NamedApiResource<MoveTarget> { Name = string.Empty, Url = string.Empty },
            Type = new NamedApiResource<Schemas.PokeApi.Type>
            {
                Name = string.Empty,
                Url = string.Empty,
            },
        };
    }
}
