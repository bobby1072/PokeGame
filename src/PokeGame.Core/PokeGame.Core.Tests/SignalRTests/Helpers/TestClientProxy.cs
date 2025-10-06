using Microsoft.AspNetCore.SignalR;

namespace PokeGame.Core.Tests.SignalRTests.Helpers;

internal sealed class TestClientProxy : IClientProxy
{
    private readonly List<(string MethodName, object?[] Arguments)> _invocations = new();

    public IReadOnlyList<(string MethodName, object?[] Arguments)> Invocations => _invocations;

    public Task SendCoreAsync(
        string method,
        object?[] args,
        CancellationToken cancellationToken = default
    )
    {
        _invocations.Add((method, args));
        return Task.CompletedTask;
    }

    public Task SendAsync(
        string method,
        object? value,
        CancellationToken cancellationToken = default
    )
    {
        return SendCoreAsync(method, new[] { value }, cancellationToken);
    }
}
