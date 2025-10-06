using System.Security.Claims;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Http.Features;
using Microsoft.AspNetCore.SignalR;

namespace PokeGame.Core.Tests.SignalRTests.Helpers;

internal sealed class TestHubCallerContext : HubCallerContext
{
    private readonly TestHttpContext _httpContext;
    private readonly Dictionary<object, object?> _items;
    private readonly IFeatureCollection _features;

    public TestHubCallerContext(TestHttpContext httpContext)
    {
        _httpContext = httpContext;
        _items = new Dictionary<object, object?>();
        _features = new FeatureCollection();
        
        // The SignalR GetHttpContext() extension method looks in the Features collection
        // Store HttpContext directly as a feature
        _features.Set(_httpContext);
        
        // Also try storing with a specific key that might be expected
        _features[typeof(HttpContext)] = _httpContext;
    }

    public override string ConnectionId { get; } = "test-connection-id";

    public override string? UserIdentifier => null;

    public override ClaimsPrincipal User { get; } = new ClaimsPrincipal();

    public override IDictionary<object, object?> Items => _items;

    public override IFeatureCollection Features => _features;

    public override CancellationToken ConnectionAborted { get; } = CancellationToken.None;

    public override void Abort() { }
}