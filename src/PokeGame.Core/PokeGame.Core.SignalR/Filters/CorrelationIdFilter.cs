using BT.Common.Api.Helpers;
using Microsoft.AspNetCore.SignalR;

namespace PokeGame.Core.SignalR.Filters;

internal sealed class CorrelationIdFilter: IHubFilter
{
    private readonly ILogger<CorrelationIdFilter> _logger;

    public CorrelationIdFilter(ILogger<CorrelationIdFilter> logger)
    {
        _logger = logger;
    }
    
    public async ValueTask<object?> InvokeMethodAsync(HubInvocationContext context,
        Func<HubInvocationContext, ValueTask<object?>> next)
    {
        var httpContext = context.Context.GetHttpContext();
        
        var correlationId = Guid.NewGuid().ToString();

        if (httpContext?.Response.Headers.TryAdd(ApiConstants.CorrelationIdHeader, correlationId) != true)
        {
            _logger.LogWarning("Signal R filter failed to add correlationId to response headers...");
        }
        else
        {
            _logger.LogInformation("Signal R filter successfully added correlationId: {CorrelationId} to response headers...", correlationId);
        }

        using (_logger.BeginScope(new { CorrelationId = correlationId }))
        {
            return await next.Invoke(context);
        }
    }
}