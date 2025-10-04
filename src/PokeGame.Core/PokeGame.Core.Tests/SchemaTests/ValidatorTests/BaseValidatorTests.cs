using AutoFixture;
using PokeGame.Core.Schemas.Validators;
using PokeGame.Core.Schemas.Game;

namespace PokeGame.Core.Tests.SchemaTests.ValidatorTests;

public sealed class BaseValidatorTests
{
    private readonly Fixture _fixture;

    public BaseValidatorTests()
    {
        _fixture = new Fixture();
    }

    [Theory]
    [InlineData("https://www.google.com", true)]
    [InlineData("http://localhost:3000", true)]
    [InlineData("ftp://files.example.com", true)]
    [InlineData("mailto:test@example.com", true)]
    [InlineData("file:///path/to/file", true)]
    [InlineData("invalid-uri", false)]
    [InlineData("", false)]
    [InlineData("not a uri at all", false)]
    [InlineData("://missing-scheme", false)]
    public void IsValidUri_Should_Validate_Uri_Correctly(string uri, bool expected)
    {
        // Act
        var result = TestableBaseValidator.TestIsValidUri(uri);

        // Assert
        Assert.Equal(expected, result);
    }

    [Theory]
    [InlineData("test@example.com", true)]
    [InlineData("user.name@domain.co.uk", true)]
    [InlineData("user+tag@example.org", true)]
    [InlineData("test@sub.domain.com", true)]
    [InlineData("invalid-email", false)]
    [InlineData("@domain.com", false)]
    [InlineData("user@", false)]
    [InlineData("", false)]
    public void IsValidEmail_Should_Validate_Email_Correctly(string email, bool expected)
    {
        // Act
        var result = TestableBaseValidator.TestIsValidEmail(email);

        // Assert
        Assert.Equal(expected, result);
    }

    [Fact]
    public void BaseValidator_Should_Be_Abstract_And_Inherit_From_AbstractValidator()
    {
        // Assert
        var baseValidatorType = typeof(BaseValidator<>);
        Assert.True(baseValidatorType.IsAbstract);
        Assert.True(baseValidatorType.BaseType?.Name.Contains("AbstractValidator") ?? false);
    }

    [Fact]
    public void BaseValidator_Should_Provide_Protected_Validation_Methods()
    {
        // Arrange
        var baseValidatorType = typeof(BaseValidator<User>);
        
        // Act
        var isValidUriMethod = baseValidatorType.GetMethod("IsValidUri", 
            System.Reflection.BindingFlags.Static | System.Reflection.BindingFlags.NonPublic);
        var isValidEmailMethod = baseValidatorType.GetMethod("IsValidEmail", 
            System.Reflection.BindingFlags.Static | System.Reflection.BindingFlags.NonPublic);

        // Assert
        Assert.NotNull(isValidUriMethod);
        Assert.NotNull(isValidEmailMethod);
        Assert.True(isValidUriMethod.IsStatic);
        Assert.True(isValidEmailMethod.IsStatic);
    }

    // Test helper class to access protected static methods
    private class TestableBaseValidator : BaseValidator<User>
    {
        public static bool TestIsValidUri(string uri) => IsValidUri(uri);
        public static bool TestIsValidEmail(string email) => IsValidEmail(email);
    }
}