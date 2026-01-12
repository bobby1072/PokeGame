using System.Net;
using AutoFixture;
using BT.Common.Persistence.Shared.Models;
using Microsoft.Extensions.Logging.Abstractions;
using Moq;
using PokeGame.Core.Common.Exceptions;
using PokeGame.Core.Common.GameInformationData;
using PokeGame.Core.Domain.Services.Abstract;
using PokeGame.Core.Domain.Services.Game.Abstract;
using PokeGame.Core.Domain.Services.Game.Commands;
using PokeGame.Core.Persistence.Repositories.Abstract;
using PokeGame.Core.Schemas.Game;
using PokeGame.Core.Schemas.Game.PokemonRelated;
using PokeGame.Core.Schemas.PokeApi;

namespace PokeGame.Core.Tests.DomainServiceTests.GameTests;

public sealed class InGrassRandomPokemonEncounterCommandTests
{
    private static readonly Fixture _fixture = new();
    private readonly Mock<IGameSessionRepository> _mockGameSessionRepository = new();
    private readonly Mock<IPokeGameRuleHelperService> _mockPokeGameRuleHelperService = new();
    private readonly Mock<IGameAndPokeApiResourceManagerService> _mockGameAndPokeApiResourceManagerService =
        new();
    private readonly InGrassRandomPokemonEncounterCommand _command;

    public InGrassRandomPokemonEncounterCommandTests()
    {
        _command = new InGrassRandomPokemonEncounterCommand(
            _mockGameSessionRepository.Object,
            _mockPokeGameRuleHelperService.Object,
            _mockGameAndPokeApiResourceManagerService.Object,
            new NullLogger<InGrassRandomPokemonEncounterCommand>()
        );
    }

    [Fact]
    public async Task ExecuteAsync_Should_Return_Null_When_No_Random_Encounter_Occurs()
    {
        // Arrange
        var userId = Guid.NewGuid();
        var connectionId = _fixture.Create<string>();
        var sceneName = "BasiliaForestScene";

        var user = new User
        {
            Id = userId,
            Email = _fixture.Create<string>(),
            Name = _fixture.Create<string>(),
        };

        var input = (SceneName: sceneName, ConnectionId: connectionId, CurrentUser: user);

        _mockPokeGameRuleHelperService
            .Setup(x =>
                x.GetRandomPokedexNumberFromIntRangeWithRandomEncounterIncluded(
                    It.IsAny<IntRange>()
                )
            )
            .Returns((int?)null);

        // Act
        var result = await _command.ExecuteAsync(input);

        // Assert
        Assert.NotNull(result);
        Assert.Null(result.CommandResult);

        _mockGameSessionRepository.Verify(
            x => x.GetOneWithGameSaveAndDataByConnectionIdAsync(It.IsAny<string>()),
            Times.Never
        );
    }

    [Fact]
    public async Task ExecuteAsync_Should_Throw_PokeGameApiUserException_When_Invalid_Scene_Name()
    {
        // Arrange
        var userId = Guid.NewGuid();
        var connectionId = _fixture.Create<string>();
        var invalidSceneName = "InvalidSceneName";

        var user = new User
        {
            Id = userId,
            Email = _fixture.Create<string>(),
            Name = _fixture.Create<string>(),
        };

        var input = (SceneName: invalidSceneName, ConnectionId: connectionId, CurrentUser: user);

        // Act & Assert
        var exception = await Assert.ThrowsAsync<PokeGameApiUserException>(
            () => _command.ExecuteAsync(input)
        );

        Assert.Equal(HttpStatusCode.BadRequest, exception.StatusCode);
        Assert.Equal("Invalid pokegame scene name", exception.Message);
    }

    [Fact]
    public async Task ExecuteAsync_Should_Return_WildPokemon_When_Successful_Encounter()
    {
        // Arrange
        var userId = Guid.NewGuid();
        var gameSaveId = Guid.NewGuid();
        var connectionId = _fixture.Create<string>();
        var sceneName = "BasiliaForestScene";
        var randomPokemonId = 25; // Pikachu
        var wildPokemonLevel = 5;
        var maxHp = 20;

        var user = new User
        {
            Id = userId,
            Email = _fixture.Create<string>(),
            Name = _fixture.Create<string>(),
        };

        var pokemon = CreateTestPokemon(randomPokemonId, "pikachu");

        var pokemonSpecies = CreateTestPokemonSpecies(randomPokemonId, "pikachu");

        var gameSaveData = CreateTestGameSaveData(gameSaveId, sceneName);

        var gameSession = CreateTestGameSession(userId, connectionId, gameSaveId, gameSaveData);

        var moveSet = (
            MoveOne: CreateTestMove("tackle", 40, 100),
            MoveTwo: (Move?)null,
            MoveThree: (Move?)null,
            MoveFour: (Move?)null
        );

        var moveSetResourceNames = (
            MoveOneResourceName: "tackle",
            MoveTwoResourceName: (string?)null,
            MoveThreeResourceName: (string?)null,
            MoveFourResourceName: (string?)null
        );

        var input = (SceneName: sceneName, ConnectionId: connectionId, CurrentUser: user);

        _mockPokeGameRuleHelperService
            .Setup(x =>
                x.GetRandomPokedexNumberFromIntRangeWithRandomEncounterIncluded(
                    It.IsAny<IntRange>()
                )
            )
            .Returns(randomPokemonId);

        _mockGameSessionRepository
            .Setup(x => x.GetOneWithGameSaveAndDataByConnectionIdAsync(connectionId))
            .ReturnsAsync(new DbGetOneResult<GameSession>(gameSession));

        _mockGameAndPokeApiResourceManagerService
            .Setup(x => x.GetPokemonAndSpecies(It.IsAny<int>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync((Pokemon: pokemon, PokemonSpecies: pokemonSpecies));

        _mockPokeGameRuleHelperService
            .Setup(x => x.GetRandomNumberFromIntRange(It.IsAny<IntRange>()))
            .Returns(wildPokemonLevel);

        _mockPokeGameRuleHelperService
            .Setup(x => x.GetRandomMoveSetFromPokemon(pokemon, wildPokemonLevel))
            .Returns(moveSetResourceNames);

        _mockGameAndPokeApiResourceManagerService
            .Setup(x =>
                x.GetMoveSet(
                    moveSetResourceNames.MoveOneResourceName,
                    moveSetResourceNames.MoveTwoResourceName,
                    moveSetResourceNames.MoveThreeResourceName,
                    moveSetResourceNames.MoveFourResourceName,
                    It.IsAny<CancellationToken>()
                )
            )
            .ReturnsAsync(moveSet);

        _mockPokeGameRuleHelperService
            .Setup(x => x.GetPokemonMaxHp(pokemon, wildPokemonLevel))
            .Returns(maxHp);

        // Act
        var result = await _command.ExecuteAsync(input);

        // Assert
        Assert.NotNull(result);
        Assert.NotNull(result.CommandResult);
        Assert.Equal(pokemon, result.CommandResult.Pokemon);
        Assert.Equal("pikachu", result.CommandResult.PokemonResourceName);
        Assert.Equal(wildPokemonLevel, result.CommandResult.PokemonLevel);
        Assert.Equal(maxHp, result.CommandResult.CurrentHp);
        Assert.Equal("tackle", result.CommandResult.MoveOneResourceName);
        Assert.Null(result.CommandResult.MoveTwoResourceName);
        Assert.Equal(moveSet.MoveOne, result.CommandResult.MoveOne);

        _mockGameSessionRepository.Verify(
            x => x.GetOneWithGameSaveAndDataByConnectionIdAsync(connectionId),
            Times.Once
        );
        _mockPokeGameRuleHelperService.Verify(
            x => x.GetRandomMoveSetFromPokemon(pokemon, wildPokemonLevel),
            Times.Once
        );
    }

    [Fact]
    public async Task ExecuteAsync_Should_Throw_PokeGameApiUserException_When_User_Has_No_Scene_Permission()
    {
        // Arrange
        var userId = Guid.NewGuid();
        var gameSaveId = Guid.NewGuid();
        var connectionId = _fixture.Create<string>();
        var sceneName = "BasiliaForestScene";
        var randomPokemonId = 25;

        var user = new User
        {
            Id = userId,
            Email = _fixture.Create<string>(),
            Name = _fixture.Create<string>(),
        };

        var gameSaveData = new GameSaveData
        {
            GameSaveId = gameSaveId,
            GameData = new GameSaveDataActual
            {
                LastPlayedScene = "SomeOtherScene",
                LastPlayedLocationX = 10,
                LastPlayedLocationY = 10,
                UnlockedGameResources = new List<GameDataActualUnlockedGameResource>(),
            },
        };

        var gameSession = CreateTestGameSession(userId, connectionId, gameSaveId, gameSaveData);

        var input = (SceneName: sceneName, ConnectionId: connectionId, CurrentUser: user);

        _mockPokeGameRuleHelperService
            .Setup(x =>
                x.GetRandomPokedexNumberFromIntRangeWithRandomEncounterIncluded(
                    It.IsAny<IntRange>()
                )
            )
            .Returns(randomPokemonId);

        _mockGameSessionRepository
            .Setup(x => x.GetOneWithGameSaveAndDataByConnectionIdAsync(connectionId))
            .ReturnsAsync(new DbGetOneResult<GameSession>(gameSession));

        // Act & Assert
        var exception = await Assert.ThrowsAsync<PokeGameApiUserException>(
            () => _command.ExecuteAsync(input)
        );

        Assert.Equal(HttpStatusCode.BadRequest, exception.StatusCode);
        Assert.Equal("You do not have access to this pokegame scene", exception.Message);
    }

    [Fact]
    public async Task ExecuteAsync_Should_Throw_PokeGameApiUserException_When_GameSession_Not_Found()
    {
        // Arrange
        var userId = Guid.NewGuid();
        var connectionId = _fixture.Create<string>();
        var sceneName = "BasiliaForestScene";
        var randomPokemonId = 25;

        var user = new User
        {
            Id = userId,
            Email = _fixture.Create<string>(),
            Name = _fixture.Create<string>(),
        };

        var input = (SceneName: sceneName, ConnectionId: connectionId, CurrentUser: user);

        _mockPokeGameRuleHelperService
            .Setup(x =>
                x.GetRandomPokedexNumberFromIntRangeWithRandomEncounterIncluded(
                    It.IsAny<IntRange>()
                )
            )
            .Returns(randomPokemonId);

        _mockGameSessionRepository
            .Setup(x => x.GetOneWithGameSaveAndDataByConnectionIdAsync(connectionId))
            .ReturnsAsync(new DbGetOneResult<GameSession>((GameSession?)null));

        // Act & Assert
        var exception = await Assert.ThrowsAsync<PokeGameApiUserException>(
            () => _command.ExecuteAsync(input)
        );

        Assert.Equal(HttpStatusCode.BadRequest, exception.StatusCode);
        Assert.Equal("Failed to find game session for that connection id", exception.Message);
    }

    [Fact]
    public async Task ExecuteAsync_Should_Throw_PokeGameApiServerException_When_GameSession_Query_Fails()
    {
        // Arrange
        var userId = Guid.NewGuid();
        var connectionId = _fixture.Create<string>();
        var sceneName = "BasiliaForestScene";
        var randomPokemonId = 25;

        var user = new User
        {
            Id = userId,
            Email = _fixture.Create<string>(),
            Name = _fixture.Create<string>(),
        };

        var input = (SceneName: sceneName, ConnectionId: connectionId, CurrentUser: user);

        _mockPokeGameRuleHelperService
            .Setup(x =>
                x.GetRandomPokedexNumberFromIntRangeWithRandomEncounterIncluded(
                    It.IsAny<IntRange>()
                )
            )
            .Returns(randomPokemonId);

        _mockGameSessionRepository
            .Setup(x => x.GetOneWithGameSaveAndDataByConnectionIdAsync(connectionId))
            .ReturnsAsync((DbGetOneResult<GameSession>)null!);

        // Act & Assert
        var exception = await Assert.ThrowsAsync<PokeGameApiServerException>(
            () => _command.ExecuteAsync(input)
        );

        Assert.Equal("Failed to fetch game session results", exception.Message);
    }

    [Fact]
    public async Task ExecuteAsync_Should_Throw_PokeGameApiServerException_When_GameSaveData_Is_Null()
    {
        // Arrange
        var userId = Guid.NewGuid();
        var connectionId = _fixture.Create<string>();
        var sceneName = "BasiliaForestScene";
        var randomPokemonId = 25;

        var user = new User
        {
            Id = userId,
            Email = _fixture.Create<string>(),
            Name = _fixture.Create<string>(),
        };

        var gameSession = new GameSession
        {
            Id = Guid.NewGuid(),
            ConnectionId = connectionId,
            UserId = userId,
            GameSaveId = Guid.NewGuid(),
            GameSave = null,
        };

        var input = (SceneName: sceneName, ConnectionId: connectionId, CurrentUser: user);

        _mockPokeGameRuleHelperService
            .Setup(x =>
                x.GetRandomPokedexNumberFromIntRangeWithRandomEncounterIncluded(
                    It.IsAny<IntRange>()
                )
            )
            .Returns(randomPokemonId);

        _mockGameSessionRepository
            .Setup(x => x.GetOneWithGameSaveAndDataByConnectionIdAsync(connectionId))
            .ReturnsAsync(new DbGetOneResult<GameSession>(gameSession));

        // Act & Assert
        var exception = await Assert.ThrowsAsync<PokeGameApiServerException>(
            () => _command.ExecuteAsync(input)
        );

        Assert.Equal(
            "Failed to properly fetch game session with game save data",
            exception.Message
        );
    }

    [Fact]
    public async Task ExecuteAsync_Should_Throw_PokeGameApiUserException_When_User_Is_Not_GameSession_Owner()
    {
        // Arrange
        var userId = Guid.NewGuid();
        var otherUserId = Guid.NewGuid();
        var gameSaveId = Guid.NewGuid();
        var connectionId = _fixture.Create<string>();
        var sceneName = "BasiliaForestScene";
        var randomPokemonId = 25;

        var user = new User
        {
            Id = userId,
            Email = _fixture.Create<string>(),
            Name = _fixture.Create<string>(),
        };

        var gameSaveData = new GameSaveData
        {
            GameSaveId = gameSaveId,
            GameData = new GameSaveDataActual
            {
                LastPlayedScene = sceneName,
                LastPlayedLocationX = 10,
                LastPlayedLocationY = 10,
                UnlockedGameResources = new List<GameDataActualUnlockedGameResource>(),
            },
        };

        var gameSession = new GameSession
        {
            Id = Guid.NewGuid(),
            ConnectionId = connectionId,
            UserId = otherUserId, // Different user
            GameSaveId = gameSaveId,
            GameSave = new GameSave
            {
                Id = gameSaveId,
                UserId = otherUserId,
                CharacterName = "Test",
                GameSaveData = gameSaveData,
            },
        };

        var input = (SceneName: sceneName, ConnectionId: connectionId, CurrentUser: user);

        _mockPokeGameRuleHelperService
            .Setup(x =>
                x.GetRandomPokedexNumberFromIntRangeWithRandomEncounterIncluded(
                    It.IsAny<IntRange>()
                )
            )
            .Returns(randomPokemonId);

        _mockGameSessionRepository
            .Setup(x => x.GetOneWithGameSaveAndDataByConnectionIdAsync(connectionId))
            .ReturnsAsync(new DbGetOneResult<GameSession>(gameSession));

        // Act & Assert
        var exception = await Assert.ThrowsAsync<PokeGameApiUserException>(
            () => _command.ExecuteAsync(input)
        );

        Assert.Equal(HttpStatusCode.Unauthorized, exception.StatusCode);
        Assert.Equal("The current user is not the owner of the game session", exception.Message);
    }

    [Fact]
    public async Task ExecuteAsync_Should_Create_WildPokemon_With_Multiple_Moves()
    {
        // Arrange
        var userId = Guid.NewGuid();
        var gameSaveId = Guid.NewGuid();
        var connectionId = _fixture.Create<string>();
        var sceneName = "BasiliaForestScene";
        var randomPokemonId = 25;
        var wildPokemonLevel = 10;
        var maxHp = 35;

        var user = new User
        {
            Id = userId,
            Email = _fixture.Create<string>(),
            Name = _fixture.Create<string>(),
        };

        var pokemon = CreateTestPokemon(randomPokemonId, "pikachu");

        var pokemonSpecies = CreateTestPokemonSpecies(randomPokemonId, "pikachu");

        var gameSaveData = CreateTestGameSaveData(gameSaveId, sceneName);

        var gameSession = CreateTestGameSession(userId, connectionId, gameSaveId, gameSaveData);

        var moveSet = (
            MoveOne: CreateTestMove("tackle", 40, 100),
            MoveTwo: CreateTestMove("thundershock", 40, 100),
            MoveThree: CreateTestMove("quick-attack", 40, 100),
            MoveFour: (Move?)CreateTestMove("thunder-wave", 0, 90)
        );

        var moveSetResourceNames = (
            MoveOneResourceName: "tackle",
            MoveTwoResourceName: (string?)"thundershock",
            MoveThreeResourceName: (string?)"quick-attack",
            MoveFourResourceName: (string?)"thunder-wave"
        );

        var input = (SceneName: sceneName, ConnectionId: connectionId, CurrentUser: user);

        _mockPokeGameRuleHelperService
            .Setup(x =>
                x.GetRandomPokedexNumberFromIntRangeWithRandomEncounterIncluded(
                    It.IsAny<IntRange>()
                )
            )
            .Returns(randomPokemonId);

        _mockGameSessionRepository
            .Setup(x => x.GetOneWithGameSaveAndDataByConnectionIdAsync(connectionId))
            .ReturnsAsync(new DbGetOneResult<GameSession>(gameSession));

        _mockGameAndPokeApiResourceManagerService
            .Setup(x => x.GetPokemonAndSpecies(It.IsAny<int>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync((Pokemon: pokemon, PokemonSpecies: pokemonSpecies));

        _mockPokeGameRuleHelperService
            .Setup(x => x.GetRandomNumberFromIntRange(It.IsAny<IntRange>()))
            .Returns(wildPokemonLevel);

        _mockPokeGameRuleHelperService
            .Setup(x => x.GetRandomMoveSetFromPokemon(pokemon, wildPokemonLevel))
            .Returns(moveSetResourceNames);

        _mockGameAndPokeApiResourceManagerService
            .Setup(x =>
                x.GetMoveSet(
                    moveSetResourceNames.MoveOneResourceName,
                    moveSetResourceNames.MoveTwoResourceName,
                    moveSetResourceNames.MoveThreeResourceName,
                    moveSetResourceNames.MoveFourResourceName,
                    It.IsAny<CancellationToken>()
                )
            )
            .ReturnsAsync(moveSet);

        _mockPokeGameRuleHelperService
            .Setup(x => x.GetPokemonMaxHp(pokemon, wildPokemonLevel))
            .Returns(maxHp);

        // Act
        var result = await _command.ExecuteAsync(input);

        // Assert
        Assert.NotNull(result);
        Assert.NotNull(result.CommandResult);
        Assert.Equal("tackle", result.CommandResult.MoveOneResourceName);
        Assert.Equal("thundershock", result.CommandResult.MoveTwoResourceName);
        Assert.Equal("quick-attack", result.CommandResult.MoveThreeResourceName);
        Assert.Equal("thunder-wave", result.CommandResult.MoveFourResourceName);
        Assert.Equal(moveSet.MoveOne, result.CommandResult.MoveOne);
        Assert.Equal(moveSet.MoveTwo, result.CommandResult.MoveTwo);
        Assert.Equal(moveSet.MoveThree, result.CommandResult.MoveThree);
        Assert.Equal(moveSet.MoveFour, result.CommandResult.MoveFour);
    }

    [Fact]
    public async Task ExecuteAsync_Should_Handle_Empty_MoveSet_ResourceName()
    {
        // Arrange
        var userId = Guid.NewGuid();
        var gameSaveId = Guid.NewGuid();
        var connectionId = _fixture.Create<string>();
        var sceneName = "BasiliaForestScene";
        var randomPokemonId = 25;
        var wildPokemonLevel = 5;
        var maxHp = 20;

        var user = new User
        {
            Id = userId,
            Email = _fixture.Create<string>(),
            Name = _fixture.Create<string>(),
        };

        var pokemon = CreateTestPokemon(randomPokemonId, "pikachu");

        var pokemonSpecies = CreateTestPokemonSpecies(randomPokemonId, "pikachu");

        var gameSaveData = CreateTestGameSaveData(gameSaveId, sceneName);

        var gameSession = CreateTestGameSession(userId, connectionId, gameSaveId, gameSaveData);

        var moveSetResourceNames = (
            MoveOneResourceName: string.Empty,
            MoveTwoResourceName: (string?)null,
            MoveThreeResourceName: (string?)null,
            MoveFourResourceName: (string?)null
        );

        var moveSet = (
            MoveOne: (Move?)null,
            MoveTwo: (Move?)null,
            MoveThree: (Move?)null,
            MoveFour: (Move?)null
        );

        var input = (SceneName: sceneName, ConnectionId: connectionId, CurrentUser: user);

        _mockPokeGameRuleHelperService
            .Setup(x =>
                x.GetRandomPokedexNumberFromIntRangeWithRandomEncounterIncluded(
                    It.IsAny<IntRange>()
                )
            )
            .Returns(randomPokemonId);

        _mockGameSessionRepository
            .Setup(x => x.GetOneWithGameSaveAndDataByConnectionIdAsync(connectionId))
            .ReturnsAsync(new DbGetOneResult<GameSession>(gameSession));

        _mockGameAndPokeApiResourceManagerService
            .Setup(x => x.GetPokemonAndSpecies(It.IsAny<int>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync((Pokemon: pokemon, PokemonSpecies: pokemonSpecies));

        _mockPokeGameRuleHelperService
            .Setup(x => x.GetRandomNumberFromIntRange(It.IsAny<IntRange>()))
            .Returns(wildPokemonLevel);

        _mockPokeGameRuleHelperService
            .Setup(x => x.GetRandomMoveSetFromPokemon(pokemon, wildPokemonLevel))
            .Returns(moveSetResourceNames);

        _mockGameAndPokeApiResourceManagerService
            .Setup(x =>
                x.GetMoveSet(
                    moveSetResourceNames.MoveOneResourceName,
                    moveSetResourceNames.MoveTwoResourceName,
                    moveSetResourceNames.MoveThreeResourceName,
                    moveSetResourceNames.MoveFourResourceName,
                    It.IsAny<CancellationToken>()
                )
            )
            .ReturnsAsync(moveSet);

        _mockPokeGameRuleHelperService
            .Setup(x => x.GetPokemonMaxHp(pokemon, wildPokemonLevel))
            .Returns(maxHp);

        // Act
        var result = await _command.ExecuteAsync(input);

        // Assert
        Assert.NotNull(result);
        Assert.NotNull(result.CommandResult);
        Assert.Equal(string.Empty, result.CommandResult.MoveOneResourceName);
        Assert.Null(result.CommandResult.MoveOne);
        Assert.Null(result.CommandResult.MoveTwo);
        Assert.Null(result.CommandResult.MoveThree);
        Assert.Null(result.CommandResult.MoveFour);

        _mockGameAndPokeApiResourceManagerService.Verify(
            x =>
                x.GetMoveSet(
                    moveSetResourceNames.MoveOneResourceName,
                    moveSetResourceNames.MoveTwoResourceName,
                    moveSetResourceNames.MoveThreeResourceName,
                    moveSetResourceNames.MoveFourResourceName,
                    It.IsAny<CancellationToken>()
                ),
            Times.Once
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
        return new PokemonSpecies
        {
            Id = id,
            Name = name,
            BaseHappiness = 50,
            CaptureRate = 45,
            Color = new NamedApiResource<PokemonColor> { Name = string.Empty, Url = string.Empty },
            EggGroups = new List<NamedApiResource<EggGroup>>(),
            EvolutionChain = new ApiResource<EvolutionChain> { Url = string.Empty },
            EvolvesFromSpecies = new NamedApiResource<PokemonSpecies>
            {
                Name = string.Empty,
                Url = string.Empty,
            },
            FlavorTextEntries = new List<PokemonSpeciesFlavorTexts>(),
            FormDescriptions = new List<Descriptions>(),
            FormsSwitchable = false,
            GenderRate = -1,
            Genera = new List<Genuses>(),
            Generation = new NamedApiResource<Generation>
            {
                Name = string.Empty,
                Url = string.Empty,
            },
            GrowthRate = new NamedApiResource<GrowthRate>
            {
                Name = string.Empty,
                Url = string.Empty,
            },
            Habitat = new NamedApiResource<PokemonHabitat>
            {
                Name = string.Empty,
                Url = string.Empty,
            },
            HasGenderDifferences = false,
            HatchCounter = 20,
            IsBaby = false,
            IsLegendary = false,
            IsMythical = false,
            Names = new List<Names>(),
            Order = id,
            PalParkEncounters = new List<PalParkEncounterArea>(),
            PokedexNumbers = new List<PokemonSpeciesDexEntry>(),
            Shape = new NamedApiResource<PokemonShape> { Name = string.Empty, Url = string.Empty },
            Varieties = new List<PokemonSpeciesVariety>(),
        };
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
            Type = new NamedApiResource<PokeGame.Core.Schemas.PokeApi.Type>
            {
                Name = string.Empty,
                Url = string.Empty,
            },
        };
    }

    private static GameSaveData CreateTestGameSaveData(Guid gameSaveId, string sceneName)
    {
        return new GameSaveData
        {
            GameSaveId = gameSaveId,
            GameData = new GameSaveDataActual
            {
                LastPlayedScene = sceneName,
                LastPlayedLocationX = 10,
                LastPlayedLocationY = 10,
                UnlockedGameResources = new List<GameDataActualUnlockedGameResource>
                {
                    new GameDataActualUnlockedGameResource
                    {
                        ResourceName = sceneName,
                        Type = GameDataActualUnlockedGameResourceType.Scene,
                    },
                },
            },
        };
    }

    private static GameSession CreateTestGameSession(
        Guid userId,
        string connectionId,
        Guid gameSaveId,
        GameSaveData gameSaveData
    )
    {
        return new GameSession
        {
            Id = Guid.NewGuid(),
            ConnectionId = connectionId,
            UserId = userId,
            GameSaveId = gameSaveId,
            GameSave = new GameSave
            {
                Id = gameSaveId,
                UserId = userId,
                CharacterName = "Test",
                GameSaveData = gameSaveData,
            },
        };
    }
}
