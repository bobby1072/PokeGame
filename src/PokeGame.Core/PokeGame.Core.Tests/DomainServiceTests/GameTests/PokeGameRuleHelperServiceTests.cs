using AutoFixture;
using Microsoft.Extensions.Logging.Abstractions;
using PokeGame.Core.Common.Configurations;
using PokeGame.Core.Common.Exceptions;
using PokeGame.Core.Common.GameInformationData;
using PokeGame.Core.Domain.Services.Game.Concrete;
using PokeGame.Core.Schemas.Game;
using PokeGame.Core.Schemas.Game.PokemonRelated;
using PokeGame.Core.Schemas.PokeApi;

namespace PokeGame.Core.Tests.DomainServiceTests.GameTests;

public sealed class PokeGameRuleHelperServiceTests
{
    private static readonly Fixture _fixture = new();
    private readonly ConfigurablePokeGameRules _configurablePokeGameRules;
    private readonly PokeGameRuleHelperService _service;

    public PokeGameRuleHelperServiceTests()
    {
        _configurablePokeGameRules = new ConfigurablePokeGameRules
        {
            RandomPokemonEncounterLikelyhoodPercentage = 50,
            BaseXpCeiling = 100,
            XpMultiplier = 1.5m,
            LegendaryXpMultiplier = 2.0m,
            StatCalculationStats = new StatCalculationStats
            {
                DefaultEV = 0,
                DefaultIV = 15
            }
        };

        _service = new PokeGameRuleHelperService(
            _configurablePokeGameRules,
            new NullLogger<PokeGameRuleHelperService>()
        );
    }

    #region GetRandomMoveSetFromPokemon Tests

    [Fact]
    public void GetRandomMoveSetFromPokemon_Should_Return_Empty_When_Pokemon_Has_No_Moves()
    {
        // Arrange
        var pokemon = CreateTestPokemon(moves: new List<PokemonMove>());
        var level = 10;

        // Act
        var result = _service.GetRandomMoveSetFromPokemon(pokemon, level);

        // Assert
        Assert.Null(result.MoveOneResourceName);
        Assert.Null(result.MoveTwoResourceName);
        Assert.Null(result.MoveThreeResourceName);
        Assert.Null(result.MoveFourResourceName);
    }

    [Fact]
    public void GetRandomMoveSetFromPokemon_Should_Return_Single_Move_When_Only_One_Move_Available()
    {
        // Arrange
        var moves = new List<PokemonMove>
        {
            new PokemonMove
            {
                Move = new NamedApiResource<Move> { Name = "tackle", Url = "" },
                VersionGroupDetails = new List<PokemonMoveVersion>
                {
                    new PokemonMoveVersion
                    {
                        LevelLearnedAt = 1,
                        MoveLearnMethod = new NamedApiResource<MoveLearnMethod>
                        {
                            Name = "level-up",
                            Url = ""
                        },
                        VersionGroup = new NamedApiResource<VersionGroup> { Name = "", Url = "" }
                    }
                }
            }
        };
        var pokemon = CreateTestPokemon(moves: moves);
        var level = 10;

        // Act
        var result = _service.GetRandomMoveSetFromPokemon(pokemon, level);

        // Assert
        Assert.Equal("tackle", result.MoveOneResourceName);
        Assert.Null(result.MoveTwoResourceName);
        Assert.Null(result.MoveThreeResourceName);
        Assert.Null(result.MoveFourResourceName);
    }

    [Fact]
    public void GetRandomMoveSetFromPokemon_Should_Return_Up_To_Four_Moves()
    {
        // Arrange
        var moves = new List<PokemonMove>
        {
            CreatePokemonMove("tackle", 1),
            CreatePokemonMove("growl", 1),
            CreatePokemonMove("ember", 5),
            CreatePokemonMove("scratch", 5),
            CreatePokemonMove("fire-blast", 50),
            CreatePokemonMove("flamethrower", 30)
        };
        var pokemon = CreateTestPokemon(moves: moves);
        var level = 10;

        // Act
        var result = _service.GetRandomMoveSetFromPokemon(pokemon, level);

        // Assert
        Assert.NotNull(result.MoveOneResourceName);
        Assert.NotNull(result.MoveTwoResourceName);
        Assert.NotNull(result.MoveThreeResourceName);
        Assert.NotNull(result.MoveFourResourceName);

        // All selected moves should be from the available moves at this level
        var availableMoves = new[] { "tackle", "growl", "ember", "scratch" };
        Assert.Contains(result.MoveOneResourceName, availableMoves);
        Assert.Contains(result.MoveTwoResourceName, availableMoves);
        Assert.Contains(result.MoveThreeResourceName, availableMoves);
        Assert.Contains(result.MoveFourResourceName, availableMoves);
    }

    [Fact]
    public void GetRandomMoveSetFromPokemon_Should_Only_Return_Moves_Learned_At_Or_Below_Level()
    {
        // Arrange
        var moves = new List<PokemonMove>
        {
            CreatePokemonMove("tackle", 1),
            CreatePokemonMove("ember", 5),
            CreatePokemonMove("fire-blast", 50)
        };
        var pokemon = CreateTestPokemon(moves: moves);
        var level = 10;

        // Act
        var result = _service.GetRandomMoveSetFromPokemon(pokemon, level);

        // Assert
        var allMoves = new[]
        {
            result.MoveOneResourceName,
            result.MoveTwoResourceName,
            result.MoveThreeResourceName,
            result.MoveFourResourceName
        }.Where(m => m != null);

        Assert.All(allMoves, move => Assert.NotEqual("fire-blast", move));
    }

    [Fact]
    public void GetRandomMoveSetFromPokemon_Should_Return_Unique_Moves()
    {
        // Arrange
        var moves = new List<PokemonMove>
        {
            CreatePokemonMove("tackle", 1),
            CreatePokemonMove("growl", 1),
            CreatePokemonMove("ember", 5),
            CreatePokemonMove("scratch", 5)
        };
        var pokemon = CreateTestPokemon(moves: moves);
        var level = 10;

        // Act
        var result = _service.GetRandomMoveSetFromPokemon(pokemon, level);

        // Assert
        var allMoves = new[]
        {
            result.MoveOneResourceName,
            result.MoveTwoResourceName,
            result.MoveThreeResourceName,
            result.MoveFourResourceName
        }.Where(m => m != null).ToList();

        Assert.Equal(allMoves.Count, allMoves.Distinct().Count());
    }

    #endregion

    #region GetRandomPokedexNumberFromIntRangeWithRandomEncounterIncluded Tests

    [Fact]
    public void GetRandomPokedexNumberFromIntRangeWithRandomEncounterIncluded_Should_Sometimes_Return_Null()
    {
        // Arrange
        var intRange = new IntRange { Min = 1, Max = 10, Extras = Array.Empty<int>() };
        var results = new List<int?>();

        // Act - Run multiple times to account for randomness
        for (int i = 0; i < 100; i++)
        {
            results.Add(_service.GetRandomPokedexNumberFromIntRangeWithRandomEncounterIncluded(intRange));
        }

        // Assert - With 50% chance, we should get at least some nulls and some values
        Assert.Contains(results, r => r == null);
        Assert.Contains(results, r => r != null);
    }

    [Fact]
    public void GetRandomPokedexNumberFromIntRangeWithRandomEncounterIncluded_Should_Return_Number_In_Range_When_Encounter_Occurs()
    {
        // Arrange
        var intRange = new IntRange { Min = 1, Max = 10, Extras = new[] { 25 } };

        // Act - Run multiple times to get at least one non-null result
        var results = new List<int>();
        for (int i = 0; i < 100; i++)
        {
            var result = _service.GetRandomPokedexNumberFromIntRangeWithRandomEncounterIncluded(intRange);
            if (result.HasValue)
            {
                results.Add(result.Value);
            }
        }

        // Assert
        Assert.NotEmpty(results);
        Assert.All(results, r => Assert.True((r >= 1 && r <= 10) || r == 25));
    }

    #endregion

    #region GetRandomNumberFromIntRange Tests

    [Fact]
    public void GetRandomNumberFromIntRange_Should_Return_Number_Within_Min_And_Max()
    {
        // Arrange
        var intRange = new IntRange { Min = 5, Max = 15, Extras = Array.Empty<int>() };

        // Act - Run multiple times to verify randomness
        var results = new List<int>();
        for (int i = 0; i < 50; i++)
        {
            results.Add(_service.GetRandomNumberFromIntRange(intRange));
        }

        // Assert
        Assert.All(results, r => Assert.InRange(r, 5, 15));
    }

    [Fact]
    public void GetRandomNumberFromIntRange_Should_Include_Extras()
    {
        // Arrange
        var intRange = new IntRange { Min = 1, Max = 1, Extras = new[] { 100 } };

        // Act - Run multiple times to potentially get the extra value
        var results = new List<int>();
        for (int i = 0; i < 100; i++)
        {
            results.Add(_service.GetRandomNumberFromIntRange(intRange));
        }

        // Assert - Should have both 1 (from range) and 100 (from extras)
        Assert.Contains(1, results);
        Assert.Contains(100, results);
    }

    [Fact]
    public void GetRandomNumberFromIntRange_Should_Return_Value_From_Full_Range()
    {
        // Arrange
        var intRange = new IntRange { Min = 1, Max = 5, Extras = Array.Empty<int>() };

        // Act - Run many times to ensure we get variety
        var results = new HashSet<int>();
        for (int i = 0; i < 500; i++)
        {
            results.Add(_service.GetRandomNumberFromIntRange(intRange));
        }

        // Assert - Should eventually get all values from 1 to 5
        Assert.True(results.Count >= 3); // At least most values should appear
    }

    #endregion

    #region CalculateStat Tests

    [Fact]
    public void CalculateStat_Should_Calculate_Correct_Stat_Value()
    {
        // Arrange
        var level = 50;
        var baseStat = 100;

        // Act
        var result = _service.CalculateStat(level, baseStat);

        // Expected: ((2 * 100 + 15 + 0/4) * 50 / 100) + 5 = (215 * 50 / 100) + 5 = 107.5 + 5 = 112
        // Assert
        Assert.Equal(112, result);
    }

    [Fact]
    public void CalculateStat_Should_Return_Minimum_Value_At_Level_One()
    {
        // Arrange
        var level = 1;
        var baseStat = 50;

        // Act
        var result = _service.CalculateStat(level, baseStat);

        // Expected: ((2 * 50 + 15 + 0) * 1 / 100) + 5 = (115 * 1 / 100) + 5 = 1.15 + 5 = 6
        // Assert
        Assert.Equal(6, result);
    }

    [Fact]
    public void CalculateStat_Should_Scale_With_Level()
    {
        // Arrange
        var baseStat = 100;
        var level1 = 10;
        var level2 = 50;
        var level3 = 100;

        // Act
        var stat1 = _service.CalculateStat(level1, baseStat);
        var stat2 = _service.CalculateStat(level2, baseStat);
        var stat3 = _service.CalculateStat(level3, baseStat);

        // Assert - Higher level should give higher stats
        Assert.True(stat1 < stat2);
        Assert.True(stat2 < stat3);
    }

    #endregion

    #region GetPokemonMaxHp Tests

    [Fact]
    public void GetPokemonMaxHp_Should_Calculate_Correct_Hp()
    {
        // Arrange
        var stats = new List<PokemonStat>
        {
            new PokemonStat
            {
                BaseStat = 45,
                Effort = 0,
                Stat = new NamedApiResource<Stat> { Name = "hp", Url = "" }
            }
        };
        var pokemon = CreateTestPokemon(stats: stats);
        var level = 50;

        // Act
        var result = _service.GetPokemonMaxHp(pokemon, level);

        // Expected: ((2 * 45 + 15 + 0) * 50 / 100) + 50 + 10 = (105 * 50 / 100) + 60 = 52.5 + 60 = 112
        // Assert
        Assert.Equal(112, result);
    }

    [Fact]
    public void GetPokemonMaxHp_Should_Scale_With_Level()
    {
        // Arrange
        var stats = new List<PokemonStat>
        {
            new PokemonStat
            {
                BaseStat = 50,
                Effort = 0,
                Stat = new NamedApiResource<Stat> { Name = "hp", Url = "" }
            }
        };
        var pokemon = CreateTestPokemon(stats: stats);

        // Act
        var hp1 = _service.GetPokemonMaxHp(pokemon, 10);
        var hp2 = _service.GetPokemonMaxHp(pokemon, 50);
        var hp3 = _service.GetPokemonMaxHp(pokemon, 100);

        // Assert
        Assert.True(hp1 < hp2);
        Assert.True(hp2 < hp3);
    }

    [Fact]
    public void GetPokemonMaxHp_Should_Use_Hp_Stat()
    {
        // Arrange
        var stats = new List<PokemonStat>
        {
            new PokemonStat
            {
                BaseStat = 45,
                Effort = 0,
                Stat = new NamedApiResource<Stat> { Name = "hp", Url = "" }
            },
            new PokemonStat
            {
                BaseStat = 100,
                Effort = 0,
                Stat = new NamedApiResource<Stat> { Name = "attack", Url = "" }
            }
        };
        var pokemon = CreateTestPokemon(stats: stats);
        var level = 50;

        // Act
        var result = _service.GetPokemonMaxHp(pokemon, level);

        // Assert - Should use HP stat (45), not attack stat (100)
        Assert.Equal(112, result);
    }

    #endregion

    #region RefillOwnedPokemonHp Tests

    [Fact]
    public void RefillOwnedPokemonHp_Should_Restore_Hp_To_Max()
    {
        // Arrange
        var stats = new List<PokemonStat>
        {
            new PokemonStat
            {
                BaseStat = 45,
                Effort = 0,
                Stat = new NamedApiResource<Stat> { Name = "hp", Url = "" }
            }
        };
        var ownedPokemon = CreateTestOwnedPokemon();
        ownedPokemon.PokemonLevel = 50;
        ownedPokemon.CurrentHp = 10;
        ownedPokemon.Pokemon = CreateTestPokemon(stats: stats);

        // Act
        var result = _service.RefillOwnedPokemonHp(ownedPokemon);

        // Assert
        Assert.Equal(112, result.CurrentHp);
        Assert.True(result.CurrentHp > 10);
    }

    [Fact]
    public void RefillOwnedPokemonHp_Should_Throw_When_Pokemon_Not_Attached()
    {
        // Arrange
        var ownedPokemon = CreateTestOwnedPokemon();
        ownedPokemon.Pokemon = null;

        // Act & Assert
        var exception = Assert.Throws<PokeGameApiServerException>(
            () => _service.RefillOwnedPokemonHp(ownedPokemon)
        );

        Assert.Equal("Pokedex pokemon not attached to owned pokemon", exception.Message);
    }

    #endregion

    #region AddXpToOwnedPokemon Tests

    [Fact]
    public void AddXpToOwnedPokemon_Should_Add_Experience_Without_Level_Up()
    {
        // Arrange
        var stats = new List<PokemonStat>
        {
            new PokemonStat
            {
                BaseStat = 45,
                Effort = 0,
                Stat = new NamedApiResource<Stat> { Name = "hp", Url = "" }
            }
        };
        var ownedPokemon = CreateTestOwnedPokemon();
        ownedPokemon.PokemonLevel = 5;
        ownedPokemon.CurrentExperience = 50;
        ownedPokemon.Pokemon = CreateTestPokemon(stats: stats);
        ownedPokemon.PokemonSpecies = CreateTestPokemonSpecies(isLegendary: false);

        // Act
        var result = _service.AddXpToOwnedPokemon(ownedPokemon, 30);

        // Assert
        Assert.Equal(5, result.PokemonLevel); // Should stay at level 5
        Assert.Equal(80, result.CurrentExperience); // 50 + 30
    }

    [Fact]
    public void AddXpToOwnedPokemon_Should_Level_Up_When_Enough_Experience()
    {
        // Arrange
        var stats = new List<PokemonStat>
        {
            new PokemonStat
            {
                BaseStat = 45,
                Effort = 0,
                Stat = new NamedApiResource<Stat> { Name = "hp", Url = "" }
            }
        };
        var ownedPokemon = CreateTestOwnedPokemon();
        ownedPokemon.PokemonLevel = 1;
        ownedPokemon.CurrentExperience = 0;
        ownedPokemon.Pokemon = CreateTestPokemon(stats: stats);
        ownedPokemon.PokemonSpecies = CreateTestPokemonSpecies(isLegendary: false);

        // Act - Add enough XP to level up (BaseXpCeiling is 100, multiplier is 1.5)
        var result = _service.AddXpToOwnedPokemon(ownedPokemon, 200);

        // Assert
        Assert.True(result.PokemonLevel > 1);
    }

    [Fact]
    public void AddXpToOwnedPokemon_Should_Use_Different_Multiplier_For_Legendary()
    {
        // Arrange - Regular pokemon
        var stats = new List<PokemonStat>
        {
            new PokemonStat
            {
                BaseStat = 45,
                Effort = 0,
                Stat = new NamedApiResource<Stat> { Name = "hp", Url = "" }
            }
        };
        var regularPokemon = CreateTestOwnedPokemon();
        regularPokemon.PokemonLevel = 1;
        regularPokemon.CurrentExperience = 0;
        regularPokemon.Pokemon = CreateTestPokemon(stats: stats);
        regularPokemon.PokemonSpecies = CreateTestPokemonSpecies(isLegendary: false);

        // Arrange - Legendary pokemon
        var legendaryPokemon = CreateTestOwnedPokemon();
        legendaryPokemon.PokemonLevel = 1;
        legendaryPokemon.CurrentExperience = 0;
        legendaryPokemon.Pokemon = CreateTestPokemon(stats: stats);
        legendaryPokemon.PokemonSpecies = CreateTestPokemonSpecies(isLegendary: true);

        // Act - Add same XP to both
        var regularResult = _service.AddXpToOwnedPokemon(regularPokemon, 500);
        var legendaryResult = _service.AddXpToOwnedPokemon(legendaryPokemon, 500);

        // Assert - Regular should level up faster than legendary (legendary needs more XP)
        Assert.True(regularResult.PokemonLevel >= legendaryResult.PokemonLevel);
    }

    [Fact]
    public void AddXpToOwnedPokemon_Should_Throw_When_PokemonSpecies_Not_Attached()
    {
        // Arrange
        var ownedPokemon = CreateTestOwnedPokemon();
        ownedPokemon.PokemonSpecies = null;

        // Act & Assert
        var exception = Assert.Throws<PokeGameApiServerException>(
            () => _service.AddXpToOwnedPokemon(ownedPokemon, 100)
        );

        Assert.Equal("Pokemon species not attached to owned pokemon", exception.Message);
    }

    [Fact]
    public void AddXpToOwnedPokemon_Should_Throw_When_Pokemon_Not_Attached()
    {
        // Arrange
        var ownedPokemon = CreateTestOwnedPokemon();
        ownedPokemon.PokemonSpecies = CreateTestPokemonSpecies(isLegendary: false);
        ownedPokemon.Pokemon = null;

        // Act & Assert
        var exception = Assert.Throws<PokeGameApiServerException>(
            () => _service.AddXpToOwnedPokemon(ownedPokemon, 100)
        );

        Assert.Equal("Pokedex pokemon not attached to owned pokemon", exception.Message);
    }

    [Fact]
    public void AddXpToOwnedPokemon_Should_Handle_Multiple_Level_Ups()
    {
        // Arrange
        var stats = new List<PokemonStat>
        {
            new PokemonStat
            {
                BaseStat = 45,
                Effort = 0,
                Stat = new NamedApiResource<Stat> { Name = "hp", Url = "" }
            }
        };
        var ownedPokemon = CreateTestOwnedPokemon();
        ownedPokemon.PokemonLevel = 1;
        ownedPokemon.CurrentExperience = 0;
        ownedPokemon.Pokemon = CreateTestPokemon(stats: stats);
        ownedPokemon.PokemonSpecies = CreateTestPokemonSpecies(isLegendary: false);

        // Act - Add massive XP to trigger multiple level ups
        var result = _service.AddXpToOwnedPokemon(ownedPokemon, 5000);

        // Assert - Should have leveled up multiple times
        Assert.True(result.PokemonLevel > 3);
    }

    #endregion

    #region Helper Methods

    private static Pokemon CreateTestPokemon(List<PokemonMove>? moves = null, List<PokemonStat>? stats = null)
    {
        return new Pokemon
        {
            Id = 1,
            Name = "bulbasaur",
            BaseExperienceFromDefeating = 64,
            Height = 7,
            Weight = 69,
            IsDefault = true,
            Order = 1,
            Abilities = new List<PokemonAbility>(),
            Forms = new List<NamedApiResource<PokemonForm>>(),
            GameIndicies = new List<VersionGameIndex>(),
            HeldItems = new List<PokemonHeldItem>(),
            LocationAreaEncounters = "",
            Moves = moves ?? new List<PokemonMove>(),
            PastTypes = new List<PokemonPastTypes>(),
            Sprites = new PokemonSprites
            {
                FrontDefault = "front",
                FrontShiny = "front-shiny",
                BackDefault = "back",
                BackShiny = "back-shiny"
            },
            Species = new NamedApiResource<PokemonSpecies> { Name = "bulbasaur", Url = "" },
            Stats = stats ?? new List<PokemonStat>(),
            Types = new List<PokemonType>()
        };
    }

    private static PokemonMove CreatePokemonMove(string moveName, int levelLearnedAt)
    {
        return new PokemonMove
        {
            Move = new NamedApiResource<Move> { Name = moveName, Url = "" },
            VersionGroupDetails = new List<PokemonMoveVersion>
            {
                new PokemonMoveVersion
                {
                    LevelLearnedAt = levelLearnedAt,
                    MoveLearnMethod = new NamedApiResource<MoveLearnMethod>
                    {
                        Name = "level-up",
                        Url = ""
                    },
                    VersionGroup = new NamedApiResource<VersionGroup> { Name = "", Url = "" }
                }
            }
        };
    }

    private static OwnedPokemon CreateTestOwnedPokemon()
    {
        return new OwnedPokemon
        {
            Id = Guid.NewGuid(),
            GameSaveId = Guid.NewGuid(),
            PokemonResourceName = "bulbasaur",
            PokemonLevel = 1,
            CurrentExperience = 0,
            CurrentHp = 20,
            MoveOneResourceName = "tackle"
        };
    }

    private static PokemonSpecies CreateTestPokemonSpecies(bool isLegendary)
    {
        return new PokemonSpecies
        {
            Id = 1,
            Name = "bulbasaur",
            Order = 1,
            GenderRate = 1,
            CaptureRate = 45,
            BaseHappiness = 50,
            IsBaby = false,
            IsLegendary = isLegendary,
            IsMythical = false,
            HatchCounter = 20,
            HasGenderDifferences = false,
            FormsSwitchable = false,
            GrowthRate = new NamedApiResource<GrowthRate> { Name = "medium-slow", Url = "" },
            PokedexNumbers = new List<PokemonSpeciesDexEntry>(),
            EggGroups = new List<NamedApiResource<EggGroup>>(),
            Color = new NamedApiResource<PokemonColor> { Name = "green", Url = "" },
            Shape = new NamedApiResource<PokemonShape> { Name = "quadruped", Url = "" },
            EvolvesFromSpecies = new NamedApiResource<PokemonSpecies> { Name = "", Url = "" },
            EvolutionChain = new ApiResource<EvolutionChain> { Url = "" },
            Habitat = new NamedApiResource<PokemonHabitat> { Name = "grassland", Url = "" },
            Generation = new NamedApiResource<Generation> { Name = "generation-i", Url = "" },
            Names = new List<Names>(),
            PalParkEncounters = new List<PalParkEncounterArea>(),
            FlavorTextEntries = new List<PokemonSpeciesFlavorTexts>(),
            FormDescriptions = new List<Descriptions>(),
            Genera = new List<Genuses>(),
            Varieties = new List<PokemonSpeciesVariety>()
        };
    }

    #endregion
}
