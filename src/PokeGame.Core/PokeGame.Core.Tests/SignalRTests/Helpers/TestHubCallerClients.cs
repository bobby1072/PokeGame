using Microsoft.AspNetCore.SignalR;

namespace PokeGame.Core.Tests.SignalRTests.Helpers;

internal sealed class TestHubCallerClients : IHubCallerClients, IHubCallerClients<IClientProxy>
{
    public TestHubCallerClients(TestClientProxy caller)
    {
        Caller = caller;
    }

    public IClientProxy Caller { get; }

    public IClientProxy All => throw new NotImplementedException();

    public IClientProxy AllExcept(IReadOnlyList<string> excludedConnectionIds) =>
        throw new NotImplementedException();

    public IClientProxy Client(string connectionId) => throw new NotImplementedException();

    public IClientProxy Clients(IReadOnlyList<string> connectionIds) =>
        throw new NotImplementedException();

    public IClientProxy Group(string groupName) => throw new NotImplementedException();

    public IClientProxy GroupExcept(
        string groupName,
        IReadOnlyList<string> excludedConnectionIds
    ) => throw new NotImplementedException();

    public IClientProxy Groups(IReadOnlyList<string> groupNames) =>
        throw new NotImplementedException();

    public IClientProxy Others => throw new NotImplementedException();

    public IClientProxy OthersInGroup(string groupName) => throw new NotImplementedException();

    public IClientProxy User(string userId) => throw new NotImplementedException();

    public IClientProxy Users(IReadOnlyList<string> userIds) => throw new NotImplementedException();
}
