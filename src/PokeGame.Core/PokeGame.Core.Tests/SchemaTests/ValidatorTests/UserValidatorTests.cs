using AutoFixture;
using FluentValidation.TestHelper;
using PokeGame.Core.Schemas.Game;
using PokeGame.Core.Schemas.Validators;

namespace PokeGame.Core.Tests.SchemaTests.ValidatorTests;

public sealed class UserValidatorTests
{
    private readonly Fixture _fixture;
    private readonly UserValidator _validator;

    public UserValidatorTests()
    {
        _fixture = new Fixture();
        _validator = new UserValidator();
    }

    [Fact]
    public void Should_Have_Error_When_Name_Is_Empty()
    {
        // Arrange
        var user = _fixture.Build<User>()
            .With(x => x.Name, string.Empty)
            .Create();

        // Act & Assert
        var result = _validator.TestValidate(user);
        result.ShouldHaveValidationErrorFor(x => x.Name)
              .WithErrorMessage("Name is required");
    }

    [Fact]
    public void Should_Have_Error_When_Name_Is_Null()
    {
        // Arrange
        var user = _fixture.Build<User>()
            .With(x => x.Name, (string)null!)
            .Create();

        // Act & Assert
        var result = _validator.TestValidate(user);
        result.ShouldHaveValidationErrorFor(x => x.Name)
              .WithErrorMessage("Name is required");
    }

    [Fact]
    public void Should_Have_Error_When_Email_Is_Empty()
    {
        // Arrange
        var user = _fixture.Build<User>()
            .With(x => x.Email, string.Empty)
            .Create();

        // Act & Assert
        var result = _validator.TestValidate(user);
        result.ShouldHaveValidationErrorFor(x => x.Email)
              .WithErrorMessage("Email is required");
    }

    [Fact]
    public void Should_Have_Error_When_Email_Is_Null()
    {
        // Arrange
        var user = _fixture.Build<User>()
            .With(x => x.Email, (string)null!)
            .Create();

        // Act & Assert
        var result = _validator.TestValidate(user);
        result.ShouldHaveValidationErrorFor(x => x.Email)
              .WithErrorMessage("Email is required");
    }

    [Theory]
    [InlineData("invalid-email")]
    [InlineData("@domain.com")]
    [InlineData("user@")]
    public void Should_Have_Error_When_Email_Is_Invalid(string invalidEmail)
    {
        // Arrange
        var user = _fixture.Build<User>()
            .With(x => x.Email, invalidEmail)
            .Create();

        // Act & Assert
        var result = _validator.TestValidate(user);
        result.ShouldHaveValidationErrorFor(x => x.Email)
              .WithErrorMessage("Invalid email address");
    }

    [Theory]
    [InlineData("test@example.com")]
    [InlineData("user.name@domain.co.uk")]
    [InlineData("user+tag@example.org")]
    [InlineData("test@sub.domain.com")]
    public void Should_Not_Have_Error_When_Email_Is_Valid(string validEmail)
    {
        // Arrange
        var user = _fixture.Build<User>()
            .With(x => x.Email, validEmail)
            .With(x => x.Name, "Valid Name")
            .Create();

        // Act & Assert
        var result = _validator.TestValidate(user);
        result.ShouldNotHaveValidationErrorFor(x => x.Email);
    }

    [Fact]
    public void Should_Not_Have_Error_When_Name_Is_Valid()
    {
        // Arrange
        var user = _fixture.Build<User>()
            .With(x => x.Name, "Valid Name")
            .With(x => x.Email, "test@example.com")
            .Create();

        // Act & Assert
        var result = _validator.TestValidate(user);
        result.ShouldNotHaveValidationErrorFor(x => x.Name);
    }

    [Fact]
    public void Should_Pass_Validation_When_All_Fields_Are_Valid()
    {
        // Arrange
        var user = _fixture.Build<User>()
            .With(x => x.Name, "John Doe")
            .With(x => x.Email, "john.doe@example.com")
            .Create();

        // Act & Assert
        var result = _validator.TestValidate(user);
        result.ShouldNotHaveAnyValidationErrors();
    }

    [Fact]
    public void Should_Have_Multiple_Errors_When_Multiple_Fields_Are_Invalid()
    {
        // Arrange
        var user = _fixture.Build<User>()
            .With(x => x.Name, string.Empty)
            .With(x => x.Email, "invalid-email")
            .Create();

        // Act & Assert
        var result = _validator.TestValidate(user);
        result.ShouldHaveValidationErrorFor(x => x.Name);
        result.ShouldHaveValidationErrorFor(x => x.Email);
    }

    [Fact]
    public void Should_Not_Validate_Other_Properties()
    {
        // Arrange
        var user = _fixture.Build<User>()
            .With(x => x.Name, "Valid Name")
            .With(x => x.Email, "test@example.com")
            .With(x => x.Id, (Guid?)null) // Invalid ID shouldn't matter
            .Create();

        // Act & Assert
        var result = _validator.TestValidate(user);
        result.ShouldNotHaveAnyValidationErrors();
    }
}