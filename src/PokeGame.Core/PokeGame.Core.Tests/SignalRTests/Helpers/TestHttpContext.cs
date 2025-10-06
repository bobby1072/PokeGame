using System.Security.Claims;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Http.Features;
using Microsoft.Extensions.Primitives;

namespace PokeGame.Core.Tests.SignalRTests.Helpers;

public class TestHttpContext : HttpContext
{
    private readonly Dictionary<string, StringValues> _queryParams = new();
    private readonly Dictionary<object, object?> _items = new();
    private readonly HttpRequest _request;

    public TestHttpContext()
    {
        _request = new TestHttpRequest(this);
    }

    public void SetQueryParam(string key, string value)
    {
        _queryParams[key] = new StringValues(value);
    }

    private IQueryCollection? _query;
    public IQueryCollection Query => _query ??= new QueryCollection(_queryParams);
    public override IDictionary<object, object?> Items
    {
        get => _items;
        set => throw new NotImplementedException();
    }
    public override HttpRequest Request => _request;

    #region Not Implemented
    public override ConnectionInfo Connection => throw new NotImplementedException();
    public override WebSocketManager WebSockets => throw new NotImplementedException();
    public override ClaimsPrincipal User { get; set; } = new ClaimsPrincipal();
    public override IFeatureCollection Features { get; } = new FeatureCollection();
    public override IServiceProvider RequestServices { get; set; } = null!;
    public override CancellationToken RequestAborted { get; set; }
    public override string TraceIdentifier { get; set; } = string.Empty;
    public override ISession Session { get; set; } = null!;
    public override HttpResponse Response => throw new NotImplementedException();

    public override void Abort() { }

    #endregion
}

public class TestHttpRequest : HttpRequest
{
    private readonly TestHttpContext _context;

    public TestHttpRequest(TestHttpContext context)
    {
        _context = context;
    }

    public override HttpContext HttpContext => _context;
    public override IQueryCollection Query
    {
        get => _context.Query;
        set => throw new NotImplementedException();
    }

    #region Not Implemented
    public override Stream Body { get; set; } = null!;
    public override string? ContentType { get; set; } = string.Empty;
    public override long? ContentLength { get; set; }
    public override string Protocol { get; set; } = string.Empty;
    public override string Method { get; set; } = string.Empty;
    public override string Scheme { get; set; } = string.Empty;
    public override bool IsHttps { get; set; }
    public override HostString Host { get; set; }
    public override PathString PathBase { get; set; }
    public override PathString Path { get; set; }
    public override QueryString QueryString { get; set; }
    public override IHeaderDictionary Headers { get; } = new HeaderDictionary();
    public override IRequestCookieCollection Cookies { get; set; } = null!;
    public override bool HasFormContentType => false;
    public override IFormCollection Form { get; set; } = null!;

    public override Task<IFormCollection> ReadFormAsync(
        CancellationToken cancellationToken = default
    ) => throw new NotImplementedException();
    #endregion
}
