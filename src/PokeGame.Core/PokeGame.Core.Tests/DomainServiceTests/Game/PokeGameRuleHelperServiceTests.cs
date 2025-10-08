using Microsoft.Extensions.Logging;
using Moq;
using PokeGame.Core.Common.Configurations;
using PokeGame.Core.Common.Exceptions;
using PokeGame.Core.Domain.Services.Game.Concrete;
using PokeGame.Core.Schemas.Game;
using PokeGame.Core.Schemas.PokeApi;
using PokeGame.Core.Schemas.Pokedex;
using Xunit;
using PokemonType = PokeGame.Core.Schemas.Pokedex.PokemonType;

namespace PokeGame.Core.Tests.DomainServiceTests.Game;

public class PokeGameRuleHelperServiceTests
{
    private readonly Mock<ILogger<PokeGameRuleHelperService>> _loggerMock;
    private readonly PokeGameRules _pokeGameRules;
    private readonly PokeGameRuleHelperService _service;

    public PokeGameRuleHelperServiceTests()
    {
        _loggerMock = new Mock<ILogger<PokeGameRuleHelperService>>();
        _pokeGameRules = new PokeGameRules
        {
            XpMultiplier = 1.052M,
            BaseXpCeiling = 100,
            LegendaryXpMultiplier = 1.055M,
            HpCalculationStats = new HpCalculationStats { DefaultIV = 31, DefaultEV = 0 },
            StandardPokemonPokedexRange = new PokedexRange
            {
                Min = 1,
                Max = 143,
                Extras = new[] { 147, 148, 149 },
            },
            LegendaryPokemonPokedexRange = new PokedexRange
            {
                Min = 144,
                Max = 151,
                Extras = Array.Empty<int>(),
            },
        };

        _service = new PokeGameRuleHelperService(_pokeGameRules, _loggerMock.Object);
    }

    [Fact]
    public void GetRandomPokemonNumberFromStandardPokedexRange_Should_ReturnValidNumber()
    {
        // Act
        var result = _service.GetRandomPokemonNumberFromStandardPokedexRange();

        // Assert
        var validNumbers = Enumerable
            .Range(
                _pokeGameRules.StandardPokemonPokedexRange.Min,
                _pokeGameRules.StandardPokemonPokedexRange.Max
                    - _pokeGameRules.StandardPokemonPokedexRange.Min
                    + 1
            )
            .Concat(_pokeGameRules.StandardPokemonPokedexRange.Extras);

        Assert.Contains(result, validNumbers);
    }

    [Fact]
    public void GetRandomPokemonNumberFromLegendaryPokedexRange_Should_ReturnValidNumber()
    {
        // Act
        var result = _service.GetRandomPokemonNumberFromLegendaryPokedexRange();

        // Assert
        var validNumbers = Enumerable.Range(
            _pokeGameRules.LegendaryPokemonPokedexRange.Min,
            _pokeGameRules.LegendaryPokemonPokedexRange.Max
                - _pokeGameRules.LegendaryPokemonPokedexRange.Min
                + 1
        );

        Assert.Contains(result, validNumbers);
    }

    [Theory]
    [ClassData(typeof(HpCalculationTestData))]
    public void RefillOwnedPokemonHp_Should_Calculate_Correct_HP(
        int baseHp,
        int level,
        int expectedHp
    )
    {
        // Arrange
        var pokemon = CreateTestPokemon(
            new TestPokemonParams(Level: level, BaseHp: baseHp, CurrentExp: 0, IsLegendary: false)
        );

        // Act
        var result = _service.RefillOwnedPokemonHp(pokemon);

        // Assert
        Assert.Equal(expectedHp, result.CurrentHp);
    }

    [Theory]
    [ClassData(typeof(XpCalculationTestData))]
    public void AddXpToOwnedPokemon_Should_Calculate_Correct_Level_And_Experience(
        int startingLevel,
        int startingExp,
        int xpToAdd,
        bool isLegendary,
        (int expectedLevel, int expectedExp, int expectedHp) expected
    )
    {
        // Arrange
        var pokemon = CreateTestPokemon(
            new TestPokemonParams(
                Level: startingLevel,
                BaseHp: 45, // Using standard base HP
                CurrentExp: startingExp,
                IsLegendary: isLegendary
            )
        );

        // Act
        var result = _service.AddXpToOwnedPokemon(pokemon, xpToAdd);

        // Assert
        Assert.Equal(expected.expectedLevel, result.PokemonLevel);
        Assert.Equal(expected.expectedExp, result.CurrentExperience);
        Assert.Equal(expected.expectedHp, result.CurrentHp);
    }

    private sealed class HpCalculationTestData : TheoryData<int, int, int>
    {
        public HpCalculationTestData()
        {
            // Format: baseHp, level, expectedHp
            Add(45, 1, 12); // Bulbasaur at level 1
            Add(45, 5, 21); // Bulbasaur at level 5
            Add(45, 50, 111); // Bulbasaur at level 50
            Add(45, 100, 211); // Bulbasaur at level 100
            Add(255, 1, 27); // Blissey at level 1
            Add(255, 50, 496); // Blissey at level 50
            Add(255, 100, 946); // Blissey at level 100
            Add(1, 100, 113); // Minimum base HP at max level
        }
    }

    private sealed class XpCalculationTestData : TheoryData<int, int, int, bool, (int, int, int)>
    {
        public XpCalculationTestData()
        {
            // Format: startLevel, startExp, xpToAdd, isLegendary, (expectedLevel, expectedExp, expectedHp)

            // Standard Pokemon Tests
            Add(1, 0, 50, false, (1, 50, 12)); // Half way to level 2
            Add(1, 0, 100, false, (2, 0, 14)); // Exact level up
            Add(1, 0, 150, false, (2, 50, 14)); // Level up with remainder
            Add(5, 0, 1000, false, (8, 50, 23)); // Multiple level ups

            // Legendary Pokemon Tests (requires more XP)
            Add(1, 0, 100, true, (1, 100, 12)); // Not enough for level up
            Add(1, 0, 200, true, (2, 100, 14)); // Level up with remainder
            Add(50, 0, 10000, true, (51, 500, 112)); // High level progression

            // Edge Cases
            Add(99, 0, 100000, false, (100, 0, 211)); // Max level cap
            Add(1, 90, 20, false, (2, 10, 14)); // Almost leveled with small XP gain
        }
    }

    [Fact]
    public void AddXpToOwnedPokemon_Should_ThrowException_WhenPokemonSpeciesIsNull()
    {
        // Arrange
        var pokemon = CreateTestPokemon(
            new TestPokemonParams(Level: 1, BaseHp: 45, CurrentExp: 0, IsLegendary: false)
        );
        pokemon.PokemonSpecies = null;

        // Act & Assert
        Assert.Throws<PokeGameApiServerException>(() => _service.AddXpToOwnedPokemon(pokemon, 100));
    }

    [Fact]
    public void RefillOwnedPokemonHp_Should_ThrowException_WhenPokedexPokemonIsNull()
    {
        // Arrange
        var pokemon = CreateTestPokemon(
            new TestPokemonParams(Level: 1, BaseHp: 45, CurrentExp: 0, IsLegendary: false)
        );
        pokemon.PokedexPokemon = null;

        // Act & Assert
        Assert.Throws<PokeGameApiServerException>(() => _service.RefillOwnedPokemonHp(pokemon));
    }

    private record TestPokemonParams(int Level, int BaseHp, int CurrentExp, bool IsLegendary);

    private OwnedPokemon CreateTestPokemon(TestPokemonParams parameters)
    {
        return new OwnedPokemon
        {
            Id = Guid.NewGuid(),
            GameSaveId = Guid.NewGuid(),
            PokemonLevel = parameters.Level,
            CurrentExperience = parameters.CurrentExp,
            CurrentHp = 100,
            ResourceName = "TestPokemon",
            MoveOneResourceName = "TestMove1",
            MoveTwoResourceName = "TestMove2",
            MoveThreeResourceName = "TestMove3",
            MoveFourResourceName = "TestMove4",
            PokemonSpecies = new PokemonSpecies { IsLegendary = parameters.IsLegendary },
            PokedexPokemon = new PokedexPokemon
            {
                Id = 1,
                EnglishName = "Test Pokemon",
                JapaneseName = "テストポケモン",
                ChineseName = "测试宝可梦",
                FrenchName = "Pokemon Test",
                Type = new PokedexPokemonType { Type1 = PokemonType.Fire },
                Stats = new PokedexPokemonStats
                {
                    Hp = parameters.BaseHp,
                    Attack = 49,
                    Defence = 49,
                    SpecialAttack = 65,
                    SpecialDefence = 65,
                    Speed = 45,
                },
            },
        };
    }
}
