using BT.Common.Api.Helpers;
using BT.Common.Helpers.Models;
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
        var addedCorrelationId = AddCorrelationIdToHttpContext(context.Context.GetHttpContext());

        using (_logger.BeginScope(new LoggingScopeVariableDictionary { ["CorrelationId"] = addedCorrelationId }))
        {
            return await next.Invoke(context);
        }
    }
    public Task OnConnectedAsync(HubLifetimeContext context, Func<HubLifetimeContext, Task> next)
    {
        var addedCorrelationId = AddCorrelationIdToHttpContext(context.Context.GetHttpContext());

        using (_logger.BeginScope(new LoggingScopeVariableDictionary { ["CorrelationId"] = addedCorrelationId }))
        {
            return next.Invoke(context);
        }
    }
    public Task OnDisconnectedAsync(HubLifetimeContext context, Exception? exception,
        Func<HubLifetimeContext, Exception?, Task> next)
    {
        var addedCorrelationId = AddCorrelationIdToHttpContext(context.Context.GetHttpContext());

        using (_logger.BeginScope(new LoggingScopeVariableDictionary { ["CorrelationId"] = addedCorrelationId }))
        {
            return next.Invoke(context, exception);
        }
    }
    private string AddCorrelationIdToHttpContext(HttpContext? context)
    {
        var correlationId = Guid.NewGuid().ToString();

        if (context?.Response.Headers.TryAdd(ApiConstants.CorrelationIdHeader, correlationId) != true)
        {
            _logger.LogWarning("Signal R filter failed to add correlationId to response headers...");
        }
        else
        {
            _logger.LogInformation("Signal R filter successfully added correlationId: {CorrelationId} to response headers...", correlationId);
        }

        return correlationId;
    }
}