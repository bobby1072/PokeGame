using AutoFixture;
using FluentValidation;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using Moq;
using PokeGame.Core.Common.Exceptions;
using PokeGame.Core.Domain.Services.Abstract;
using PokeGame.Core.Domain.Services.Concrete;
using PokeGame.Core.Schemas.Game;
using System.Net;

namespace PokeGame.Core.Tests.DomainServiceTests.ConcreteTests;

public sealed class ValidatorServiceTests
{
    private readonly Fixture _fixture;
    private readonly Mock<IServiceProvider> _mockServiceProvider;
    private readonly Mock<ILogger<ValidatorService>> _mockLogger;
    private readonly ValidatorService _validatorService;

    public ValidatorServiceTests()
    {
        _fixture = new Fixture();
        _mockServiceProvider = new Mock<IServiceProvider>();
        _mockLogger = new Mock<ILogger<ValidatorService>>();
        _validatorService = new ValidatorService(_mockServiceProvider.Object, _mockLogger.Object);
    }

    [Fact]
    public async Task ValidateAndThrowAsync_Should_Log_Warning_When_No_Validator_Found()
    {
        // Arrange
        var testModel = _fixture.Create<User>();
        _mockServiceProvider.Setup(x => x.GetService(typeof(IValidator<User>))).Returns(null as IValidator<User>);

        // Act
        await _validatorService.ValidateAndThrowAsync(testModel);

        // Assert
        _mockLogger.Verify(
            x => x.Log(
                LogLevel.Warning,
                It.IsAny<EventId>(),
                It.Is<It.IsAnyType>((v, t) => v.ToString()!.Contains("No validator found for type: User")),
                It.IsAny<Exception>(),
                It.IsAny<Func<It.IsAnyType, Exception?, string>>()),
            Times.Once);
    }

    [Fact]
    public async Task ValidateAndThrowAsync_Should_Log_Success_When_Validation_Passes()
    {
        // Arrange
        var testModel = _fixture.Create<User>();
        var mockValidator = new Mock<IValidator<User>>();
        var validationResult = new FluentValidation.Results.ValidationResult();
        
        mockValidator.Setup(x => x.ValidateAsync(testModel, It.IsAny<CancellationToken>()))
                    .ReturnsAsync(validationResult);
        
        _mockServiceProvider.Setup(x => x.GetService(typeof(IValidator<User>)))
                           .Returns(mockValidator.Object);

        // Act
        await _validatorService.ValidateAndThrowAsync(testModel);

        // Assert
        _mockLogger.Verify(
            x => x.Log(
                LogLevel.Information,
                It.IsAny<EventId>(),
                It.Is<It.IsAnyType>((v, t) => v.ToString()!.Contains("Validation succeeded for model of type: User")),
                It.IsAny<Exception>(),
                It.IsAny<Func<It.IsAnyType, Exception?, string>>()),
            Times.Once);
    }

    [Fact]
    public async Task ValidateAndThrowAsync_Should_Throw_PokeGameApiUserException_When_Validation_Fails()
    {
        // Arrange
        var testModel = _fixture.Create<User>();
        var mockValidator = new Mock<IValidator<User>>();
        var validationResult = new FluentValidation.Results.ValidationResult();
        validationResult.Errors.Add(new FluentValidation.Results.ValidationFailure("Email", "Email is required"));
        validationResult.Errors.Add(new FluentValidation.Results.ValidationFailure("Name", "Name is required"));
        
        mockValidator.Setup(x => x.ValidateAsync(testModel, It.IsAny<CancellationToken>()))
                    .ReturnsAsync(validationResult);
        
        _mockServiceProvider.Setup(x => x.GetService(typeof(IValidator<User>)))
                           .Returns(mockValidator.Object);

        // Act & Assert
        var exception = await Assert.ThrowsAsync<PokeGameApiUserException>(
            () => _validatorService.ValidateAndThrowAsync(testModel));
        
        Assert.Equal(HttpStatusCode.BadRequest, exception.StatusCode);
        Assert.Contains("Email is required", exception.Message);
        Assert.Contains("Name is required", exception.Message);
    }

    [Fact]
    public async Task ValidateAndThrowAsync_Should_Log_Validation_Failure()
    {
        // Arrange
        var testModel = _fixture.Create<User>();
        var mockValidator = new Mock<IValidator<User>>();
        var validationResult = new FluentValidation.Results.ValidationResult();
        validationResult.Errors.Add(new FluentValidation.Results.ValidationFailure("Email", "Email is required"));
        
        mockValidator.Setup(x => x.ValidateAsync(testModel, It.IsAny<CancellationToken>()))
                    .ReturnsAsync(validationResult);
        
        _mockServiceProvider.Setup(x => x.GetService(typeof(IValidator<User>)))
                           .Returns(mockValidator.Object);

        // Act & Assert
        await Assert.ThrowsAsync<PokeGameApiUserException>(
            () => _validatorService.ValidateAndThrowAsync(testModel));

        // Assert
        _mockLogger.Verify(
            x => x.Log(
                LogLevel.Information,
                It.IsAny<EventId>(),
                It.Is<It.IsAnyType>((v, t) => v.ToString()!.Contains("Model of type: User validation failed")),
                It.IsAny<Exception>(),
                It.IsAny<Func<It.IsAnyType, Exception?, string>>()),
            Times.Once);
    }

    [Fact]
    public async Task ValidateAndThrowAsync_Should_Pass_CancellationToken_To_Validator()
    {
        // Arrange
        var testModel = _fixture.Create<User>();
        var mockValidator = new Mock<IValidator<User>>();
        var validationResult = new FluentValidation.Results.ValidationResult();
        var cancellationToken = new CancellationToken();
        
        mockValidator.Setup(x => x.ValidateAsync(testModel, cancellationToken))
                    .ReturnsAsync(validationResult);
        
        _mockServiceProvider.Setup(x => x.GetService(typeof(IValidator<User>)))
                           .Returns(mockValidator.Object);

        // Act
        await _validatorService.ValidateAndThrowAsync(testModel, cancellationToken);

        // Assert
        mockValidator.Verify(x => x.ValidateAsync(testModel, cancellationToken), Times.Once);
    }

    [Fact]
    public async Task ValidateAndThrowAsync_Should_Handle_Different_Model_Types()
    {
        // Arrange
        var gameModel = _fixture.Create<GameSave>();
        var mockGameValidator = new Mock<IValidator<GameSave>>();
        var validationResult = new FluentValidation.Results.ValidationResult();
        
        mockGameValidator.Setup(x => x.ValidateAsync(gameModel, It.IsAny<CancellationToken>()))
                        .ReturnsAsync(validationResult);
        
        _mockServiceProvider.Setup(x => x.GetService(typeof(IValidator<GameSave>)))
                           .Returns(mockGameValidator.Object);

        // Act
        await _validatorService.ValidateAndThrowAsync(gameModel);

        // Assert
        _mockLogger.Verify(
            x => x.Log(
                LogLevel.Information,
                It.IsAny<EventId>(),
                It.Is<It.IsAnyType>((v, t) => v.ToString()!.Contains("Validation succeeded for model of type: GameSave")),
                It.IsAny<Exception>(),
                It.IsAny<Func<It.IsAnyType, Exception?, string>>()),
            Times.Once);
    }

    [Fact]
    public async Task ValidateAndThrowAsync_Should_Not_Throw_When_No_Validator_Found()
    {
        // Arrange
        var testModel = _fixture.Create<User>();
        _mockServiceProvider.Setup(x => x.GetService(typeof(IValidator<User>))).Returns(null as IValidator<User>);

        // Act & Assert - Should not throw
        await _validatorService.ValidateAndThrowAsync(testModel);
    }
}