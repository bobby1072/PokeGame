using AutoFixture;
using FluentValidation.TestHelper;
using PokeGame.Core.Schemas.Game;
using PokeGame.Core.Schemas.Game.PokemonRelated;
using PokeGame.Core.Schemas.Validators;

namespace PokeGame.Core.Tests.SchemaTests.ValidatorTests;

public sealed class OwnedItemValidatorTests
{
    private readonly Fixture _fixture;
    private readonly OwnedItemValidator _validator;

    public OwnedItemValidatorTests()
    {
        _fixture = new Fixture();
        _validator = new OwnedItemValidator();
    }

    [Fact]
    public void Should_Have_Error_When_ResourceName_Is_Empty()
    {
        // Arrange
        var itemStack = _fixture.Build<OwnedItem>()
            .With(x => x.ResourceName, string.Empty)
            .With(x => x.Quantity, _fixture.Create<int>() % 99 + 1) // Valid quantity
            .Create();

        // Act & Assert
        var result = _validator.TestValidate(itemStack);
        result.ShouldHaveValidationErrorFor(x => x.ResourceName)
              .WithErrorMessage("Resource name cannot be empty");
    }

    [Fact]
    public void Should_Have_Error_When_ResourceName_Is_Null()
    {
        // Arrange
        var itemStack = _fixture.Build<OwnedItem>()
            .With(x => x.ResourceName, (string)null!)
            .With(x => x.Quantity, _fixture.Create<int>() % 99 + 1) // Valid quantity
            .Create();

        // Act & Assert
        var result = _validator.TestValidate(itemStack);
        result.ShouldHaveValidationErrorFor(x => x.ResourceName)
              .WithErrorMessage("Resource name cannot be empty");
    }

    [Theory]
    [InlineData("invalid-uri")]
    [InlineData("not a uri")]
    [InlineData("http://")]
    [InlineData("://invalid")]
    public void Should_Have_Error_When_ResourceName_Is_Invalid_Uri(string invalidUri)
    {
        // Arrange
        var itemStack = _fixture.Build<OwnedItem>()
            .With(x => x.ResourceName, invalidUri)
            .With(x => x.Quantity, _fixture.Create<int>() % 99 + 1) // Valid quantity
            .Create();

        // Act & Assert
        var result = _validator.TestValidate(itemStack);
        result.ShouldHaveValidationErrorFor(x => x.ResourceName)
              .WithErrorMessage("Resource names must be valid Uris");
    }

    [Theory]
    [InlineData("https://example.com/items/potion")]
    [InlineData("http://pokemon.com/items/pokeball")]
    [InlineData("ftp://server.com/resources/item1")]
    [InlineData("file:///items/berry")]
    public void Should_Not_Have_Error_When_ResourceName_Is_Valid_Uri(string validUri)
    {
        // Arrange
        var itemStack = _fixture.Build<OwnedItem>()
            .With(x => x.ResourceName, validUri)
            .With(x => x.Quantity, _fixture.Create<int>() % 99 + 1) // Valid quantity
            .Create();

        // Act & Assert
        var result = _validator.TestValidate(itemStack);
        result.ShouldNotHaveValidationErrorFor(x => x.ResourceName);
    }

    [Theory]
    [InlineData(0)]
    [InlineData(-1)]
    [InlineData(-100)]
    public void Should_Have_Error_When_Quantity_Is_Less_Than_One(int invalidQuantity)
    {
        // Arrange
        var itemStack = _fixture.Build<OwnedItem>()
            .With(x => x.ResourceName, "https://example.com/items/potion")
            .With(x => x.Quantity, invalidQuantity)
            .Create();

        // Act & Assert
        var result = _validator.TestValidate(itemStack);
        result.ShouldHaveValidationErrorFor(x => x.Quantity)
              .WithErrorMessage("Quantity must be between 1 - 99");
    }

    [Theory]
    [InlineData(100)]
    [InlineData(500)]
    [InlineData(int.MaxValue)]
    public void Should_Have_Error_When_Quantity_Is_Greater_Than_99(int invalidQuantity)
    {
        // Arrange
        var itemStack = _fixture.Build<OwnedItem>()
            .With(x => x.ResourceName, "https://example.com/items/potion")
            .With(x => x.Quantity, invalidQuantity)
            .Create();

        // Act & Assert
        var result = _validator.TestValidate(itemStack);
        result.ShouldHaveValidationErrorFor(x => x.Quantity)
              .WithErrorMessage("Quantity must be between 1 - 99");
    }

    [Theory]
    [InlineData(1)]
    [InlineData(50)]
    [InlineData(99)]
    public void Should_Not_Have_Error_When_Quantity_Is_Valid(int validQuantity)
    {
        // Arrange
        var itemStack = _fixture.Build<OwnedItem>()
            .With(x => x.ResourceName, "https://example.com/items/potion")
            .With(x => x.Quantity, validQuantity)
            .Create();

        // Act & Assert
        var result = _validator.TestValidate(itemStack);
        result.ShouldNotHaveValidationErrorFor(x => x.Quantity);
    }

    [Fact]
    public void Should_Pass_Validation_When_All_Fields_Are_Valid()
    {
        // Arrange
        var itemStack = _fixture.Build<OwnedItem>()
            .With(x => x.ResourceName, "https://pokemon.com/items/pokeball")
            .With(x => x.Quantity, 10)
            .Create();

        // Act & Assert
        var result = _validator.TestValidate(itemStack);
        result.ShouldNotHaveAnyValidationErrors();
    }

    [Fact]
    public void Should_Have_Multiple_Errors_When_Multiple_Fields_Are_Invalid()
    {
        // Arrange
        var itemStack = _fixture.Build<OwnedItem>()
            .With(x => x.ResourceName, "invalid-uri")
            .With(x => x.Quantity, 0)
            .Create();

        // Act & Assert
        var result = _validator.TestValidate(itemStack);
        result.ShouldHaveValidationErrorFor(x => x.ResourceName);
        result.ShouldHaveValidationErrorFor(x => x.Quantity);
    }

    [Fact]
    public void Should_Not_Validate_Other_Properties()
    {
        // Arrange
        var itemStack = _fixture.Build<OwnedItem>()
            .With(x => x.ResourceName, "https://example.com/items/potion")
            .With(x => x.Quantity, 25)
            .With(x => x.Id, (Guid?)null) // Invalid ID shouldn't matter
            .Create();

        // Act & Assert
        var result = _validator.TestValidate(itemStack);
        result.ShouldNotHaveAnyValidationErrors();
    }

    [Fact]
    public void Should_Accept_Various_Uri_Schemes_For_ResourceName()
    {
        // Arrange & Act & Assert
        var validUris = new[]
        {
            "https://example.com/items/potion",
            "http://pokemon.com/items/pokeball",
            "ftp://server.com/resources/item1",
            "file:///local/items/berry",
            "custom://scheme/resource/item"
        };

        foreach (var uri in validUris)
        {
            var itemStack = _fixture.Build<OwnedItem>()
                .With(x => x.ResourceName, uri)
                .With(x => x.Quantity, 1)
                .Create();

            var result = _validator.TestValidate(itemStack);
            result.ShouldNotHaveValidationErrorFor(x => x.ResourceName);
        }
    }
}