using System.Security.Claims;
using AutoFixture;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.SignalR;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging.Abstractions;
using Moq;
using PokeGame.Core.Common;
using PokeGame.Core.Common.Exceptions;
using PokeGame.Core.Domain.Services.Game.Abstract;
using PokeGame.Core.Domain.Services.User.Abstract;
using PokeGame.Core.Schemas.Game;
using PokeGame.Core.SignalR.Hubs;
using PokeGame.Core.SignalR.Models;
using PokeGame.Core.Tests.SignalRTests.Helpers;

namespace PokeGame.Core.Tests.SignalRTests.Hubs;

public sealed class PokeGameSessionHubTests
{
    private static readonly Fixture _fixture = new();
    private readonly TestClientProxy _clientProxy = new();
    private readonly TestHubCallerClients _clients;
    private readonly Mock<HubCallerContext> _mockHubContext = new();
    private readonly Mock<IServiceProvider> _mockServiceProvider = new();
    private readonly Mock<IGameSessionProcessingManager> _mockGameSessionManager = new();
    private readonly Mock<IUserProcessingManager> _mockUserManager = new();
    private readonly PokeGameSessionHub _hub;
    private readonly TestHttpContext _testHttpContext = new();

    public PokeGameSessionHubTests()
    {
        _clients = new TestHubCallerClients(_clientProxy);

        _mockHubContext.Setup(context => context.ConnectionId).Returns("test-connection-id");

        var items = new Dictionary<object, object?>();
        items["HttpContext"] = _testHttpContext;
        _mockHubContext.Setup(c => c.Items).Returns(items);
        _mockHubContext.Setup(c => c.User).Returns(new ClaimsPrincipal());

        _mockServiceProvider
            .Setup(sp => sp.GetService(typeof(IGameSessionProcessingManager)))
            .Returns(_mockGameSessionManager.Object);

        _mockServiceProvider
            .Setup(sp => sp.GetService(typeof(IUserProcessingManager)))
            .Returns(_mockUserManager.Object);

        _hub = new PokeGameSessionHub(
            _mockServiceProvider.Object,
            new NullLogger<PokeGameSessionHub>()
        )
        {
            Clients = (IHubCallerClients)_clients,
            Context = _mockHubContext.Object,
        };
    }

    [Fact]
    public async Task OnConnectedAsync_Should_Start_Game_Session_And_Send_Success_Event_When_Valid_Input()
    {
        // Arrange
        var userId = Guid.NewGuid();
        var gameSaveId = Guid.NewGuid();
        var user = _fixture.Create<User>();
        var gameSession = _fixture.Create<GameSession>();

        _testHttpContext.SetQueryParam(Constants.ApiConstants.UserIdHeaderKey, userId.ToString());
        _testHttpContext.SetQueryParam(
            Constants.ApiConstants.GameSaveIdHeaderKey,
            gameSaveId.ToString()
        );

        _mockUserManager.Setup(m => m.GetUserAsync(userId)).ReturnsAsync(user);

        _mockGameSessionManager
            .Setup(m => m.StartGameSession(gameSaveId, "test-connection-id", user))
            .ReturnsAsync(gameSession);

        // Act
        await _hub.OnConnectedAsync();

        // Assert
        var invocation = Assert.Single(_clientProxy.Invocations);
        Assert.Equal("GameSessionStarted", invocation.MethodName);
        var eventArg = Assert.Single(invocation.Arguments);
        var evt = Assert.IsType<SignalRServerEvent<GameSession>>(eventArg);
        Assert.Equal(gameSession, evt.Data);
        Assert.Equal("GameSessionStarted", evt.ExtraData["EventKey"]);

        _mockHubContext.Verify(c => c.ConnectionId, Times.AtLeastOnce);
        _mockUserManager.Verify(m => m.GetUserAsync(userId), Times.Once);
        _mockGameSessionManager.Verify(
            m => m.StartGameSession(gameSaveId, "test-connection-id", user),
            Times.Once
        );
    }

    [Fact]
    public async Task OnConnectedAsync_Should_Send_Error_And_Abort_When_No_GameSaveId()
    {
        // Arrange
        var userId = Guid.NewGuid();
        _testHttpContext.SetQueryParam(Constants.ApiConstants.UserIdHeaderKey, userId.ToString());

        // Act
        await _hub.OnConnectedAsync();

        // Assert
        var invocation = Assert.Single(_clientProxy.Invocations);
        Assert.Equal("GameSessionConnectionFailed", invocation.MethodName);
        var eventArg = Assert.Single(invocation.Arguments);
        var evt = Assert.IsType<SignalRClientEvent>(eventArg);
        Assert.Equal("No game save id attached to request query", evt.ExceptionMessage);
        Assert.Equal("GameSessionConnectionFailed", evt.ExtraData["EventKey"]);

        _mockHubContext.Verify(c => c.Abort(), Times.Once);
    }

    [Fact]
    public async Task OnConnectedAsync_Should_Send_Error_And_Abort_When_No_UserId()
    {
        // Arrange
        var gameSaveId = Guid.NewGuid();
        _testHttpContext.SetQueryParam(
            Constants.ApiConstants.GameSaveIdHeaderKey,
            gameSaveId.ToString()
        );

        // Act
        await _hub.OnConnectedAsync();

        // Assert
        var invocation = Assert.Single(_clientProxy.Invocations);
        Assert.Equal("GameSessionConnectionFailed", invocation.MethodName);
        var eventArg = Assert.Single(invocation.Arguments);
        var evt = Assert.IsType<SignalRClientEvent>(eventArg);
        Assert.Equal("No user id attached to request query", evt.ExceptionMessage);
        Assert.Equal("GameSessionConnectionFailed", evt.ExtraData["EventKey"]);

        _mockHubContext.Verify(c => c.Abort(), Times.Once);
    }

    [Fact]
    public async Task OnConnectedAsync_Should_Handle_User_Exception()
    {
        // Arrange
        var userId = Guid.NewGuid();
        var gameSaveId = Guid.NewGuid();
        var expectedMessage = "User not found";

        _testHttpContext.SetQueryParam(Constants.ApiConstants.UserIdHeaderKey, userId.ToString());
        _testHttpContext.SetQueryParam(
            Constants.ApiConstants.GameSaveIdHeaderKey,
            gameSaveId.ToString()
        );

        _mockUserManager
            .Setup(m => m.GetUserAsync(userId))
            .ThrowsAsync(
                new PokeGameApiUserException(System.Net.HttpStatusCode.BadRequest, expectedMessage)
            );

        // Act
        await _hub.OnConnectedAsync();

        // Assert
        var invocation = Assert.Single(_clientProxy.Invocations);
        Assert.Equal("GameSessionConnectionFailed", invocation.MethodName);
        var eventArg = Assert.Single(invocation.Arguments);
        var evt = Assert.IsType<SignalRClientEvent>(eventArg);
        Assert.Equal(expectedMessage, evt.ExceptionMessage);

        _mockHubContext.Verify(c => c.Abort(), Times.Once);
    }

    [Fact]
    public async Task OnConnectedAsync_Should_Handle_Server_Exception()
    {
        // Arrange
        var userId = Guid.NewGuid();
        var gameSaveId = Guid.NewGuid();

        _testHttpContext.SetQueryParam(Constants.ApiConstants.UserIdHeaderKey, userId.ToString());
        _testHttpContext.SetQueryParam(
            Constants.ApiConstants.GameSaveIdHeaderKey,
            gameSaveId.ToString()
        );

        _mockUserManager
            .Setup(m => m.GetUserAsync(userId))
            .ThrowsAsync(new PokeGameApiServerException("Internal error"));

        // Act
        await _hub.OnConnectedAsync();

        // Assert
        var invocation = Assert.Single(_clientProxy.Invocations);
        Assert.Equal("GameSessionConnectionFailed", invocation.MethodName);
        var eventArg = Assert.Single(invocation.Arguments);
        var evt = Assert.IsType<SignalRClientEvent>(eventArg);
        Assert.Equal(Constants.ExceptionConstants.InternalError, evt.ExceptionMessage);

        _mockHubContext.Verify(c => c.Abort(), Times.Once);
    }

    [Fact]
    public async Task OnDisconnectedAsync_Should_Delete_All_Sessions()
    {
        // Arrange
        _mockGameSessionManager
            .Setup(m => m.DeleteAllGameSessionsByConnectionId("test-connection-id"))
            .Returns(Task.CompletedTask);

        // Act
        await _hub.OnDisconnectedAsync(null);

        // Assert
        _mockGameSessionManager.Verify(
            m => m.DeleteAllGameSessionsByConnectionId("test-connection-id"),
            Times.Once
        );
    }

    [Fact]
    public async Task OnDisconnectedAsync_Should_Handle_Exception()
    {
        // Arrange
        _mockGameSessionManager
            .Setup(m => m.DeleteAllGameSessionsByConnectionId("test-connection-id"))
            .ThrowsAsync(new Exception("Test exception"));

        // Act & Assert
        // Should not throw
        await _hub.OnDisconnectedAsync(null);

        _mockGameSessionManager.Verify(
            m => m.DeleteAllGameSessionsByConnectionId("test-connection-id"),
            Times.Once
        );
    }
}
