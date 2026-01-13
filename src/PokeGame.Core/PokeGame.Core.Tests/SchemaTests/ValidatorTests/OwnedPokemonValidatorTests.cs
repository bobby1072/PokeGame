using AutoFixture;
using AutoFixture.Dsl;
using FluentValidation.TestHelper;
using PokeGame.Core.Schemas.Game;
using PokeGame.Core.Schemas.Game.PokemonRelated;
using PokeGame.Core.Schemas.Validators;

namespace PokeGame.Core.Tests.SchemaTests.ValidatorTests;

public sealed class OwnedPokemonValidatorTests
{
    private readonly Fixture _fixture;
    private readonly OwnedPokemonValidator _validator;

    public OwnedPokemonValidatorTests()
    {
        _fixture = new Fixture();
        _validator = new OwnedPokemonValidator();
    }

    #region PokemonLevel Validation Tests

    [Theory]
    [InlineData(0)]
    [InlineData(-1)]
    [InlineData(-50)]
    public void Should_Have_Error_When_PokemonLevel_Is_Less_Than_One(int invalidLevel)
    {
        // Arrange
        var ownedPokemon = CreateValidOwnedPokemon()
            .With(x => x.PokemonLevel, invalidLevel)
            .Create();

        // Act & Assert
        var result = _validator.TestValidate(ownedPokemon);
        result
            .ShouldHaveValidationErrorFor(x => x.PokemonLevel)
            .WithErrorMessage("Pokemon level must be between 1 - 99");
    }

    [Theory]
    [InlineData(100)]
    [InlineData(150)]
    [InlineData(int.MaxValue)]
    public void Should_Have_Error_When_PokemonLevel_Is_Greater_Than_99(int invalidLevel)
    {
        // Arrange
        var ownedPokemon = CreateValidOwnedPokemon()
            .With(x => x.PokemonLevel, invalidLevel)
            .Create();

        // Act & Assert
        var result = _validator.TestValidate(ownedPokemon);
        result
            .ShouldHaveValidationErrorFor(x => x.PokemonLevel)
            .WithErrorMessage("Pokemon level must be between 1 - 99");
    }

    [Theory]
    [InlineData(1)]
    [InlineData(50)]
    [InlineData(99)]
    public void Should_Not_Have_Error_When_PokemonLevel_Is_Valid(int validLevel)
    {
        // Arrange
        var ownedPokemon = CreateValidOwnedPokemon().With(x => x.PokemonLevel, validLevel).Create();

        // Act & Assert
        var result = _validator.TestValidate(ownedPokemon);
        result.ShouldNotHaveValidationErrorFor(x => x.PokemonLevel);
    }

    #endregion

    #region CurrentExperience Validation Tests

    [Theory]
    [InlineData(0)]
    [InlineData(-1)]
    [InlineData(-1000)]
    public void Should_Have_Error_When_CurrentExperience_Is_Not_Greater_Than_Zero(
        int invalidExperience
    )
    {
        // Arrange
        var ownedPokemon = CreateValidOwnedPokemon()
            .With(x => x.CurrentExperience, invalidExperience)
            .Create();

        // Act & Assert
        var result = _validator.TestValidate(ownedPokemon);
        result
            .ShouldHaveValidationErrorFor(x => x.CurrentExperience)
            .WithErrorMessage("Current experience must be greater than 0");
    }

    [Theory]
    [InlineData(1)]
    [InlineData(1000)]
    [InlineData(int.MaxValue)]
    public void Should_Not_Have_Error_When_CurrentExperience_Is_Greater_Than_Zero(
        int validExperience
    )
    {
        // Arrange
        var ownedPokemon = CreateValidOwnedPokemon()
            .With(x => x.CurrentExperience, validExperience)
            .Create();

        // Act & Assert
        var result = _validator.TestValidate(ownedPokemon);
        result.ShouldNotHaveValidationErrorFor(x => x.CurrentExperience);
    }

    #endregion

    #region CurrentHp Validation Tests

    [Theory]
    [InlineData(-1)]
    [InlineData(-100)]
    [InlineData(int.MinValue)]
    public void Should_Have_Error_When_CurrentHp_Is_Less_Than_Zero(int invalidHP)
    {
        // Arrange
        var ownedPokemon = CreateValidOwnedPokemon().With(x => x.CurrentHp, invalidHP).Create();

        // Act & Assert
        var result = _validator.TestValidate(ownedPokemon);
        result
            .ShouldHaveValidationErrorFor(x => x.CurrentHp)
            .WithErrorMessage("Current hp cannot be negative");
    }

    [Theory]
    [InlineData(0)]
    [InlineData(1)]
    [InlineData(100)]
    [InlineData(int.MaxValue)]
    public void Should_Not_Have_Error_When_CurrentHp_Is_Valid(int validHP)
    {
        // Arrange
        var ownedPokemon = CreateValidOwnedPokemon().With(x => x.CurrentHp, validHP).Create();

        // Act & Assert
        var result = _validator.TestValidate(ownedPokemon);
        result.ShouldNotHaveValidationErrorFor(x => x.CurrentHp);
    }

    #endregion

    #region Move Resource Name Validation Tests

    [Fact]
    public void Should_Not_Have_Error_When_MoveOneResourceName_Is_Null()
    {
        // Arrange
        var ownedPokemon = CreateValidOwnedPokemon()
            .With(x => x.MoveOneResourceName, (string?)null)
            .Create();

        // Act & Assert
        var result = _validator.TestValidate(ownedPokemon);
        result.ShouldNotHaveValidationErrorFor(x => x.MoveOneResourceName);
    }

    [Fact]
    public void Should_Not_Have_Error_When_MoveOneResourceName_Is_Empty()
    {
        // Arrange
        var ownedPokemon = CreateValidOwnedPokemon()
            .With(x => x.MoveOneResourceName, string.Empty)
            .Create();

        // Act & Assert
        var result = _validator.TestValidate(ownedPokemon);
        result.ShouldNotHaveValidationErrorFor(x => x.MoveOneResourceName);
    }

    [Theory]
    [InlineData("invalid-uri")]
    [InlineData("not a uri")]
    [InlineData("http://")]
    public void Should_Have_Error_When_MoveOneResourceName_Is_Invalid_Uri(string invalidUri)
    {
        // Arrange
        var ownedPokemon = CreateValidOwnedPokemon()
            .With(x => x.MoveOneResourceName, invalidUri)
            .Create();

        // Act & Assert
        var result = _validator.TestValidate(ownedPokemon);
        result
            .ShouldHaveValidationErrorFor(x => x.MoveOneResourceName)
            .WithErrorMessage("Resource names must be valid Uris");
    }

    [Theory]
    [InlineData("invalid-uri")]
    [InlineData("not a uri")]
    [InlineData("http://")]
    public void Should_Have_Error_When_MoveTwoResourceName_Is_Invalid_Uri(string invalidUri)
    {
        // Arrange
        var ownedPokemon = CreateValidOwnedPokemon()
            .With(x => x.MoveTwoResourceName, invalidUri)
            .Create();

        // Act & Assert
        var result = _validator.TestValidate(ownedPokemon);
        result
            .ShouldHaveValidationErrorFor(x => x.MoveTwoResourceName)
            .WithErrorMessage("Resource names must be valid Uris");
    }

    [Theory]
    [InlineData("invalid-uri")]
    [InlineData("not a uri")]
    [InlineData("http://")]
    public void Should_Have_Error_When_MoveThreeResourceName_Is_Invalid_Uri(string invalidUri)
    {
        // Arrange
        var ownedPokemon = CreateValidOwnedPokemon()
            .With(x => x.MoveThreeResourceName, invalidUri)
            .Create();

        // Act & Assert
        var result = _validator.TestValidate(ownedPokemon);
        result
            .ShouldHaveValidationErrorFor(x => x.MoveThreeResourceName)
            .WithErrorMessage("Resource names must be valid Uris");
    }

    [Theory]
    [InlineData("invalid-uri")]
    [InlineData("not a uri")]
    [InlineData("http://")]
    public void Should_Have_Error_When_MoveFourResourceName_Is_Invalid_Uri(string invalidUri)
    {
        // Arrange
        var ownedPokemon = CreateValidOwnedPokemon()
            .With(x => x.MoveFourResourceName, invalidUri)
            .Create();

        // Act & Assert
        var result = _validator.TestValidate(ownedPokemon);
        result
            .ShouldHaveValidationErrorFor(x => x.MoveFourResourceName)
            .WithErrorMessage("Resource names must be valid Uris");
    }

    [Theory]
    [InlineData("https://pokemon.com/moves/tackle")]
    [InlineData("http://moves.com/thunderbolt")]
    [InlineData("ftp://server.com/moves/surf")]
    public void Should_Not_Have_Error_When_Move_Resource_Names_Are_Valid_Uris(string validUri)
    {
        // Arrange
        var ownedPokemon = CreateValidOwnedPokemon()
            .With(x => x.MoveOneResourceName, validUri)
            .With(x => x.MoveTwoResourceName, validUri)
            .With(x => x.MoveThreeResourceName, validUri)
            .With(x => x.MoveFourResourceName, validUri)
            .Create();

        // Act & Assert
        var result = _validator.TestValidate(ownedPokemon);
        result.ShouldNotHaveValidationErrorFor(x => x.MoveOneResourceName);
        result.ShouldNotHaveValidationErrorFor(x => x.MoveTwoResourceName);
        result.ShouldNotHaveValidationErrorFor(x => x.MoveThreeResourceName);
        result.ShouldNotHaveValidationErrorFor(x => x.MoveFourResourceName);
    }

    [Fact]
    public void Should_Allow_Null_Optional_Move_Resource_Names()
    {
        // Arrange
        var ownedPokemon = CreateValidOwnedPokemon()
            .With(x => x.MoveTwoResourceName, (string?)null)
            .With(x => x.MoveThreeResourceName, (string?)null)
            .With(x => x.MoveFourResourceName, (string?)null)
            .Create();

        // Act & Assert
        var result = _validator.TestValidate(ownedPokemon);
        result.ShouldNotHaveAnyValidationErrors();
    }

    #endregion

    #region ResourceName Validation Tests

    [Fact]
    public void Should_Have_Error_When_ResourceName_Is_Empty()
    {
        // Arrange
        var ownedPokemon = CreateValidOwnedPokemon()
            .With(x => x.PokemonResourceName, string.Empty)
            .Create();

        // Act & Assert
        var result = _validator.TestValidate(ownedPokemon);
        result
            .ShouldHaveValidationErrorFor(x => x.PokemonResourceName)
            .WithErrorMessage("Resource name cannot be empty");
    }

    [Theory]
    [InlineData("invalid-uri")]
    [InlineData("not a uri")]
    [InlineData("http://")]
    public void Should_Have_Error_When_ResourceName_Is_Invalid_Uri(string invalidUri)
    {
        // Arrange
        var ownedPokemon = CreateValidOwnedPokemon()
            .With(x => x.PokemonResourceName, invalidUri)
            .Create();

        // Act & Assert
        var result = _validator.TestValidate(ownedPokemon);
        result
            .ShouldHaveValidationErrorFor(x => x.PokemonResourceName)
            .WithErrorMessage("Resource names must be valid Uris");
    }

    #endregion

    #region Integration Tests

    [Fact]
    public void Should_Pass_Validation_When_All_Fields_Are_Valid()
    {
        // Arrange
        var ownedPokemon = CreateValidOwnedPokemon().Create();

        // Act & Assert
        var result = _validator.TestValidate(ownedPokemon);
        result.ShouldNotHaveAnyValidationErrors();
    }

    [Fact]
    public void Should_Have_Multiple_Errors_When_Multiple_Fields_Are_Invalid()
    {
        // Arrange
        var ownedPokemon = _fixture
            .Build<OwnedPokemon>()
            .With(x => x.PokemonLevel, 0) // Invalid
            .With(x => x.CurrentExperience, 0) // Invalid
            .With(x => x.CurrentHp, -1) // Invalid
            .With(x => x.MoveOneResourceName, "invalid-uri") // Invalid
            .With(x => x.PokemonResourceName, "invalid-uri") // Invalid
            .Create();

        // Act & Assert
        var result = _validator.TestValidate(ownedPokemon);
        result.ShouldHaveValidationErrorFor(x => x.PokemonLevel);
        result.ShouldHaveValidationErrorFor(x => x.CurrentExperience);
        result.ShouldHaveValidationErrorFor(x => x.CurrentHp);
        result.ShouldHaveValidationErrorFor(x => x.MoveOneResourceName);
        result.ShouldHaveValidationErrorFor(x => x.PokemonResourceName);
    }

    [Fact]
    public void Should_Not_Validate_Other_Properties()
    {
        // Arrange
        var ownedPokemon = CreateValidOwnedPokemon()
            .With(x => x.Id, (Guid?)null) // Invalid ID shouldn't matter
            .With(x => x.GameSaveId, Guid.Empty) // Should not be validated
            .With(x => x.CaughtAt, DateTime.MinValue) // Should not be validated
            .Create();

        // Act & Assert
        var result = _validator.TestValidate(ownedPokemon);
        result.ShouldNotHaveAnyValidationErrors();
    }

    #endregion

    #region Helper Methods

    private IPostprocessComposer<OwnedPokemon> CreateValidOwnedPokemon()
    {
        return _fixture
            .Build<OwnedPokemon>()
            .With(x => x.PokemonLevel, _fixture.Create<int>() % 99 + 1) // 1-99
            .With(x => x.CurrentExperience, _fixture.Create<int>() % 10000 + 1) // > 0
            .With(x => x.CurrentHp, Math.Abs(_fixture.Create<int>()) % 1000) // >= 0
            .With(x => x.PokemonResourceName, "https://pokemon.com/pokemon/pikachu")
            .With(x => x.MoveOneResourceName, "https://pokemon.com/moves/tackle")
            .With(x => x.MoveTwoResourceName, "https://pokemon.com/moves/thunderbolt")
            .With(x => x.MoveThreeResourceName, "https://pokemon.com/moves/surf")
            .With(x => x.MoveFourResourceName, "https://pokemon.com/moves/fly");
    }

    #endregion
}
