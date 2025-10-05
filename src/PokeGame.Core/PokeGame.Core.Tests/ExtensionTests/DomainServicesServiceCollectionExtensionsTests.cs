using System.Text.Json;
using AutoFixture;
using BT.Common.Api.Helpers.Models;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Diagnostics.HealthChecks;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using Moq;
using PokeGame.Core.Common;
using PokeGame.Core.Domain.Services.Abstract;
using PokeGame.Core.Domain.Services.Concrete;
using PokeGame.Core.Domain.Services.Extensions;
using PokeGame.Core.Domain.Services.Game.Abstract;
using PokeGame.Core.Domain.Services.Game.Commands;
using PokeGame.Core.Domain.Services.Game.Concrete;
using PokeGame.Core.Domain.Services.Pokedex.Abstract;
using PokeGame.Core.Domain.Services.Pokedex.Commands;
using PokeGame.Core.Domain.Services.Pokedex.Concrete;
using PokeGame.Core.Domain.Services.User.Abstract;
using PokeGame.Core.Domain.Services.User.Commands;
using PokeGame.Core.Domain.Services.User.Concrete;
using PokeGame.Core.Persistence.Repositories.Abstract;
using PokeGame.Core.Persistence.Repositories.Concrete;

namespace PokeGame.Core.Tests.ExtensionTests;

public sealed class DomainServicesServiceCollectionExtensionsTests
{
    private static readonly Fixture _fixture = new();
    private readonly IServiceCollection _services = new ServiceCollection();
    private readonly IConfiguration _configuration;
    private readonly IHostEnvironment _hostEnvironment;

    public DomainServicesServiceCollectionExtensionsTests()
    {
        // Create test configuration with required ServiceInfo section
        var serviceInfo = new ServiceInfo { ReleaseName = "TestService", ReleaseVersion = "1.0.0" };

        var configurationData = new Dictionary<string, string>
        {
            [$"{ServiceInfo.Key}:{nameof(ServiceInfo.ReleaseName)}"] = serviceInfo.ReleaseName,
            [$"{ServiceInfo.Key}:{nameof(ServiceInfo.ReleaseVersion)}"] =
                serviceInfo.ReleaseVersion,
            ["ConnectionStrings:PostgresConnection"] =
                "Server=localhost;Port=5432;Database=test;User ID=test;Password=test;",
            ["DbMigrationSettings:StartVersion"] = "1",
            ["DbMigrationSettings:TotalAttempts"] = "2",
            ["DbMigrationSettings:DelayBetweenAttemptsInSeconds"] = "1",
        };

        _configuration = new ConfigurationBuilder()
            .AddInMemoryCollection(configurationData!)
            .Build();

        // Mock host environment
        var hostEnvironment = new Mock<IHostEnvironment>();
        hostEnvironment.Setup(x => x.EnvironmentName).Returns("Development");
        _hostEnvironment = hostEnvironment.Object;
    }

    [Fact]
    public void AddPokeGameApplicationServices_Should_Register_All_Core_Services()
    {
        // Act
        _services.AddPokeGameApplicationServices(_configuration, _hostEnvironment);
        var serviceProvider = _services.BuildServiceProvider();

        // Assert - Repositories are registered with correct implementations
        AssertServiceRegistration<IUserRepository, UserRepository>(ServiceLifetime.Scoped);
        AssertServiceRegistration<IPokedexPokemonRepository, PokedexPokemonRepository>(
            ServiceLifetime.Scoped
        );
        AssertServiceRegistration<IGameSaveRepository, GameSaveRepository>(ServiceLifetime.Scoped);
        AssertServiceRegistration<IOwnedPokemonRepository, OwnedPokemonRepository>(
            ServiceLifetime.Scoped
        );
        AssertServiceRegistration<IItemStackRepository, ItemStackRepository>(
            ServiceLifetime.Scoped
        );
        AssertServiceRegistration<IGameSessionRepository, GameSessionRepository>(
            ServiceLifetime.Scoped
        );

        // Assert - Core services
        AssertServiceRegistration<IDomainServiceCommandExecutor, DomainServiceCommandExecutor>(
            ServiceLifetime.Scoped
        );
        AssertServiceRegistration<IValidatorService, ValidatorService>(ServiceLifetime.Scoped);

        // Assert - HttpClient is registered
        var httpClientDescriptor = _services.FirstOrDefault(x =>
            x.ServiceType.Name.Contains("HttpClient")
        );
        Assert.NotNull(httpClientDescriptor);

        // Assert - Logging is registered
        var loggingDescriptor = _services.FirstOrDefault(x =>
            x.ServiceType == typeof(ILoggerFactory)
        );
        Assert.NotNull(loggingDescriptor);

        // Assert - Health checks are registered
        var healthCheckDescriptor = _services.FirstOrDefault(x =>
            x.ServiceType == typeof(HealthCheckService)
        );
        Assert.NotNull(healthCheckDescriptor);

        // Assert - ServiceInfo configuration is registered
        var serviceInfoDescriptor = _services.FirstOrDefault(x =>
            x.ServiceType == typeof(ServiceInfo)
        );
        Assert.NotNull(serviceInfoDescriptor);
        Assert.Equal(ServiceLifetime.Singleton, serviceInfoDescriptor.Lifetime);
    }

    [Fact]
    public void AddPokeGameApplicationServices_Should_Throw_When_ServiceInfo_Section_Missing()
    {
        // Arrange
        var emptyConfiguration = new ConfigurationBuilder().Build();

        // Act & Assert
        var exception = Assert.Throws<ArgumentNullException>(
            () => _services.AddPokeGameApplicationServices(emptyConfiguration, _hostEnvironment)
        );

        Assert.Equal(ServiceInfo.Key, exception.ParamName);
    }

    [Fact]
    public void AddGameServices_Should_Register_All_Game_Services()
    {
        // Act
        _services.AddPokeGameApplicationServices(_configuration, _hostEnvironment);

        // Assert - Game Commands
        AssertServiceRegistration<CreateNewGameCommand>(ServiceLifetime.Scoped);
        AssertServiceRegistration<GetGameSavesByUserCommand>(ServiceLifetime.Scoped);
        AssertServiceRegistration<StartGameSessionCommand>(ServiceLifetime.Scoped);
        AssertServiceRegistration<RemoveGameSessionCommand>(ServiceLifetime.Scoped);

        // Assert - Game Processing Managers
        AssertServiceRegistration<IGameSaveProcessingManager, GameSaveProcessingManager>(
            ServiceLifetime.Scoped
        );
        AssertServiceRegistration<IGameSessionProcessingManager, GameSessionProcessingManager>(
            ServiceLifetime.Scoped
        );
    }

    [Fact]
    public void AddUserServices_Should_Register_All_User_Services()
    {
        // Act
        _services.AddPokeGameApplicationServices(_configuration, _hostEnvironment);

        // Assert - User Commands
        AssertServiceRegistration<SaveUserCommand>(ServiceLifetime.Scoped);
        AssertServiceRegistration<GetUserByEmailCommand>(ServiceLifetime.Scoped);
        AssertServiceRegistration<GetUserByIdCommand>(ServiceLifetime.Scoped);

        // Assert - User Processing Manager
        AssertServiceRegistration<IUserProcessingManager, UserProcessingManager>(
            ServiceLifetime.Scoped
        );
    }

    [Fact]
    public void AddPokemonServices_Should_Register_All_Pokemon_Services()
    {
        // Act
        _services.AddPokeGameApplicationServices(_configuration, _hostEnvironment);

        // Assert - Pokemon Commands
        AssertServiceRegistration<CreateDbPokedexPokemonCommand>(ServiceLifetime.Scoped);
        AssertServiceRegistration<GetDbPokedexPokemonCommand>(ServiceLifetime.Scoped);

        // Assert - Health Check
        AssertServiceRegistration<IPokedexDataMigratorHealthCheck, PokedexDataMigratorHealthCheck>(
            ServiceLifetime.Singleton
        );

        // Assert - Hosted Service
        var hostedServiceDescriptor = _services.FirstOrDefault(x =>
            x.ServiceType == typeof(Microsoft.Extensions.Hosting.IHostedService)
            && x.ImplementationType == typeof(PokedexDataMigratorHostedService)
        );
        Assert.NotNull(hostedServiceDescriptor);
        Assert.Equal(ServiceLifetime.Singleton, hostedServiceDescriptor.Lifetime);

        // Assert - HTTP Client for PokeAPI
        var httpClientDescriptor = _services.FirstOrDefault(x =>
            x.ServiceType == typeof(IPokeApiClient)
        );
        Assert.NotNull(httpClientDescriptor);

        // Assert - Health check is added to builder (verify registration exists)
        var healthCheckServiceDescriptor = _services.FirstOrDefault(x =>
            x.ServiceType == typeof(HealthCheckService)
        );
        Assert.NotNull(healthCheckServiceDescriptor);
    }

    [Fact]
    public void AddPokedexJsonDoc_Should_Register_Pokedex_Json_Services()
    {
        // Act
        _services.AddPokeGameApplicationServices(_configuration, _hostEnvironment);

        // Assert - Pokedex JSON Factory is registered with a factory function
        var pokedexFactoryDescriptor = _services.FirstOrDefault(x =>
            x.ServiceType == typeof(IPokedexJsonFactory)
        );
        Assert.NotNull(pokedexFactoryDescriptor);
        Assert.Equal(ServiceLifetime.Singleton, pokedexFactoryDescriptor.Lifetime);
        Assert.NotNull(pokedexFactoryDescriptor.ImplementationFactory);
        Assert.Null(pokedexFactoryDescriptor.ImplementationType);

        // Assert - Keyed singleton for Pokedex JSON file
        var keyedServiceDescriptor = _services.FirstOrDefault(x =>
            x.IsKeyedService && x.ServiceKey?.ToString() == Constants.ServiceKeys.PokedexJsonFile
        );
        Assert.NotNull(keyedServiceDescriptor);
        Assert.Equal(ServiceLifetime.Singleton, keyedServiceDescriptor.Lifetime);
    }

    [Fact]
    public void Services_Should_Be_Resolvable_From_ServiceProvider()
    {
        // Act
        _services.AddPokeGameApplicationServices(_configuration, _hostEnvironment);
        var serviceProvider = _services.BuildServiceProvider();

        // Assert - All major services can be resolved
        Assert.NotNull(serviceProvider.GetService<IDomainServiceCommandExecutor>());
        Assert.NotNull(serviceProvider.GetService<IValidatorService>());
        Assert.NotNull(serviceProvider.GetService<IGameSaveProcessingManager>());
        Assert.NotNull(serviceProvider.GetService<IGameSessionProcessingManager>());
        Assert.NotNull(serviceProvider.GetService<IUserProcessingManager>());
        Assert.NotNull(serviceProvider.GetService<IPokedexDataMigratorHealthCheck>());
        Assert.NotNull(serviceProvider.GetService<IPokedexJsonFactory>());

        // Assert - Commands can be resolved
        Assert.NotNull(serviceProvider.GetService<CreateNewGameCommand>());
        Assert.NotNull(serviceProvider.GetService<GetGameSavesByUserCommand>());
        Assert.NotNull(serviceProvider.GetService<StartGameSessionCommand>());
        Assert.NotNull(serviceProvider.GetService<RemoveGameSessionCommand>());
        Assert.NotNull(serviceProvider.GetService<SaveUserCommand>());
        Assert.NotNull(serviceProvider.GetService<GetUserByEmailCommand>());
        Assert.NotNull(serviceProvider.GetService<GetUserByIdCommand>());
        Assert.NotNull(serviceProvider.GetService<CreateDbPokedexPokemonCommand>());
        Assert.NotNull(serviceProvider.GetService<GetDbPokedexPokemonCommand>());

        // Assert - Keyed service can be resolved
        var keyedService = serviceProvider.GetKeyedService<JsonDocument>(
            Constants.ServiceKeys.PokedexJsonFile
        );
        Assert.NotNull(keyedService);
    }

    [Fact]
    public void Service_Lifetimes_Should_Be_Correctly_Configured()
    {
        // Act
        _services.AddPokeGameApplicationServices(_configuration, _hostEnvironment);

        // Assert singleton services
        var singletonServices = new[]
        {
            typeof(IPokedexDataMigratorHealthCheck),
            typeof(ServiceInfo),
        };

        foreach (var serviceType in singletonServices)
        {
            var descriptor = _services.FirstOrDefault(x => x.ServiceType == serviceType);
            Assert.NotNull(descriptor);
            Assert.Equal(ServiceLifetime.Singleton, descriptor.Lifetime);
        }

        // Assert factory-registered singleton services
        var pokedexFactoryDescriptor = _services.FirstOrDefault(x =>
            x.ServiceType == typeof(IPokedexJsonFactory)
        );
        Assert.NotNull(pokedexFactoryDescriptor);
        Assert.Equal(ServiceLifetime.Singleton, pokedexFactoryDescriptor.Lifetime);

        // Assert scoped services
        var scopedServices = new[]
        {
            typeof(IDomainServiceCommandExecutor),
            typeof(IValidatorService),
            typeof(IGameSaveProcessingManager),
            typeof(IGameSessionProcessingManager),
            typeof(IUserProcessingManager),
            typeof(CreateNewGameCommand),
            typeof(GetGameSavesByUserCommand),
            typeof(StartGameSessionCommand),
            typeof(RemoveGameSessionCommand),
            typeof(SaveUserCommand),
            typeof(GetUserByEmailCommand),
            typeof(GetUserByIdCommand),
            typeof(CreateDbPokedexPokemonCommand),
            typeof(GetDbPokedexPokemonCommand),
        };

        foreach (var serviceType in scopedServices)
        {
            var descriptor = _services.FirstOrDefault(x => x.ServiceType == serviceType);
            Assert.NotNull(descriptor);
            Assert.Equal(ServiceLifetime.Scoped, descriptor.Lifetime);
        }
    }

    [Fact]
    public void Implementation_Types_Should_Match_Interface_Contracts()
    {
        // Act
        _services.AddPokeGameApplicationServices(_configuration, _hostEnvironment);

        // Assert interface-to-implementation mappings
        var interfaceImplementationMappings = new Dictionary<Type, Type>
        {
            { typeof(IDomainServiceCommandExecutor), typeof(DomainServiceCommandExecutor) },
            { typeof(IValidatorService), typeof(ValidatorService) },
            { typeof(IGameSaveProcessingManager), typeof(GameSaveProcessingManager) },
            { typeof(IGameSessionProcessingManager), typeof(GameSessionProcessingManager) },
            { typeof(IUserProcessingManager), typeof(UserProcessingManager) },
            { typeof(IPokedexDataMigratorHealthCheck), typeof(PokedexDataMigratorHealthCheck) },
        };

        foreach (var mapping in interfaceImplementationMappings)
        {
            var descriptor = _services.FirstOrDefault(x => x.ServiceType == mapping.Key);
            Assert.NotNull(descriptor);
            Assert.Equal(mapping.Value, descriptor.ImplementationType);
        }

        // Assert factory-registered services (these use ImplementationFactory instead of ImplementationType)
        var pokedexFactoryDescriptor = _services.FirstOrDefault(x =>
            x.ServiceType == typeof(IPokedexJsonFactory)
        );
        Assert.NotNull(pokedexFactoryDescriptor);
        Assert.NotNull(pokedexFactoryDescriptor.ImplementationFactory);
        Assert.Null(pokedexFactoryDescriptor.ImplementationType);
    }

    [Fact]
    public void Command_Services_Should_Be_Registered_As_Concrete_Types()
    {
        // Act
        _services.AddPokeGameApplicationServices(_configuration, _hostEnvironment);

        // Assert command services are registered as their concrete types (not interfaces)
        var commandTypes = new[]
        {
            typeof(CreateNewGameCommand),
            typeof(GetGameSavesByUserCommand),
            typeof(StartGameSessionCommand),
            typeof(RemoveGameSessionCommand),
            typeof(SaveUserCommand),
            typeof(GetUserByEmailCommand),
            typeof(GetUserByIdCommand),
            typeof(CreateDbPokedexPokemonCommand),
            typeof(GetDbPokedexPokemonCommand),
        };

        foreach (var commandType in commandTypes)
        {
            var descriptor = _services.FirstOrDefault(x => x.ServiceType == commandType);
            Assert.NotNull(descriptor);
            Assert.Equal(commandType, descriptor.ServiceType);
            Assert.Equal(commandType, descriptor.ImplementationType);
            Assert.Equal(ServiceLifetime.Scoped, descriptor.Lifetime);
        }
    }

    private void AssertServiceRegistration<TService>(ServiceLifetime expectedLifetime)
    {
        var descriptor = _services.FirstOrDefault(x => x.ServiceType == typeof(TService));
        Assert.NotNull(descriptor);
        Assert.Equal(expectedLifetime, descriptor.Lifetime);
        Assert.Equal(typeof(TService), descriptor.ImplementationType);
    }

    private void AssertServiceRegistration<TInterface, TImplementation>(
        ServiceLifetime expectedLifetime
    )
    {
        var descriptor = _services.FirstOrDefault(x => x.ServiceType == typeof(TInterface));
        Assert.NotNull(descriptor);
        Assert.Equal(expectedLifetime, descriptor.Lifetime);
        Assert.Equal(typeof(TImplementation), descriptor.ImplementationType);
    }
}
