using AutoFixture;
using Microsoft.AspNetCore.SignalR;
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
    private readonly TestServiceProvider _testServiceProvider = new();
    private readonly Mock<IGameSessionProcessingManager> _mockGameSessionManager = new();
    private readonly Mock<IUserProcessingManager> _mockUserManager = new();
    private readonly PokeGameSessionHub _hub;
    private readonly TestHttpContext _testHttpContext = new();
    private readonly TestHubCallerContext _testHubContext;

    public PokeGameSessionHubTests()
    {
        _clients = new TestHubCallerClients(_clientProxy);
        _testHttpContext = new TestHttpContext();
        _testHubContext = new TestHubCallerContext(_testHttpContext);

        _testServiceProvider.RegisterService(_mockGameSessionManager.Object);
        _testServiceProvider.RegisterService(_mockUserManager.Object);

        _hub = new PokeGameSessionHub(
            _testServiceProvider,
            new NullLogger<PokeGameSessionHub>()
        )
        {
            Clients = (IHubCallerClients)_clients,
            Context = _testHubContext,
        };
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

        // Note: Abort() is called on the context but we can't verify it with our test implementation
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
