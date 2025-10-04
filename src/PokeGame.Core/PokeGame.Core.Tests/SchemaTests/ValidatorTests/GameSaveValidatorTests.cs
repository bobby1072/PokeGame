using AutoFixture;
using FluentValidation.TestHelper;
using PokeGame.Core.Schemas.Game;
using PokeGame.Core.Schemas.Validators;

namespace PokeGame.Core.Tests.SchemaTests.ValidatorTests;

public sealed class GameSaveValidatorTests
{
    private readonly Fixture _fixture;
    private readonly GameSaveValidator _validator;

    public GameSaveValidatorTests()
    {
        _fixture = new Fixture();
        _validator = new GameSaveValidator();
    }

    [Fact]
    public void Should_Have_Error_When_CharacterName_Is_Empty()
    {
        // Arrange
        var gameSave = _fixture.Build<GameSave>()
            .With(x => x.CharacterName, string.Empty)
            .Create();

        // Act & Assert
        var result = _validator.TestValidate(gameSave);
        result.ShouldHaveValidationErrorFor(x => x.CharacterName)
              .WithErrorMessage("Character name cannot be empty");
    }

    [Fact]
    public void Should_Have_Error_When_CharacterName_Exceeds_Maximum_Length()
    {
        // Arrange
        var longCharacterName = new string('A', 51); // 51 characters
        var gameSave = _fixture.Build<GameSave>()
            .With(x => x.CharacterName, longCharacterName)
            .Create();

        // Act & Assert
        var result = _validator.TestValidate(gameSave);
        result.ShouldHaveValidationErrorFor(x => x.CharacterName)
              .WithErrorMessage("Character name must be less than 50 characters");
    }

    [Theory]
    [InlineData("A")]                                          // 1 character
    [InlineData("Ash")]                                        // 3 characters
    [InlineData("Professor Oak")]                              // 13 characters
    [InlineData("Pokemon Trainer Champion")]                   // 25 characters
    [InlineData("SuperLongButValidCharacterName")]             // 31 characters
    [InlineData("12345678901234567890123456789012345678901234567890")] // 50 characters (max)
    public void Should_Not_Have_Error_When_CharacterName_Is_Valid_Length(string validCharacterName)
    {
        // Arrange
        var gameSave = _fixture.Build<GameSave>()
            .With(x => x.CharacterName, validCharacterName)
            .Create();

        // Act & Assert
        var result = _validator.TestValidate(gameSave);
        result.ShouldNotHaveValidationErrorFor(x => x.CharacterName);
    }

    [Fact]
    public void Should_Pass_Validation_When_All_Fields_Are_Valid()
    {
        // Arrange
        var gameSave = _fixture.Build<GameSave>()
            .With(x => x.CharacterName, "Ash Ketchum")
            .Create();

        // Act & Assert
        var result = _validator.TestValidate(gameSave);
        result.ShouldNotHaveAnyValidationErrors();
    }

    [Fact]
    public void Should_Allow_Special_Characters_In_CharacterName()
    {
        // Arrange
        var characterNameWithSpecialChars = "Ash-Ketchum_123!@#";
        var gameSave = _fixture.Build<GameSave>()
            .With(x => x.CharacterName, characterNameWithSpecialChars)
            .Create();

        // Act & Assert
        var result = _validator.TestValidate(gameSave);
        result.ShouldNotHaveValidationErrorFor(x => x.CharacterName);
    }

    [Fact]
    public void Should_Allow_Unicode_Characters_In_CharacterName()
    {
        // Arrange
        var unicodeCharacterName = "Pokémon Trainer 🔥";
        var gameSave = _fixture.Build<GameSave>()
            .With(x => x.CharacterName, unicodeCharacterName)
            .Create();

        // Act & Assert
        var result = _validator.TestValidate(gameSave);
        result.ShouldNotHaveValidationErrorFor(x => x.CharacterName);
    }

    [Fact]
    public void Should_Not_Validate_Other_Properties()
    {
        // Arrange
        var gameSave = _fixture.Build<GameSave>()
            .With(x => x.CharacterName, "Valid Name")
            .With(x => x.Id, (Guid?)null) // Invalid ID shouldn't matter
            .With(x => x.UserId, (Guid?)null) // Invalid UserId shouldn't matter
            .Create();

        // Act & Assert
        var result = _validator.TestValidate(gameSave);
        result.ShouldNotHaveAnyValidationErrors();
    }

    [Theory]
    [InlineData("")]     // Empty string
    [InlineData("12345678901234567890123456789012345678901234567890A")] // 51 characters
    public void Should_Have_Error_When_CharacterName_Is_Invalid(string invalidCharacterName)
    {
        // Arrange
        var gameSave = _fixture.Build<GameSave>()
            .With(x => x.CharacterName, invalidCharacterName)
            .Create();

        // Act & Assert
        var result = _validator.TestValidate(gameSave);
        result.ShouldHaveValidationErrorFor(x => x.CharacterName);
    }
}