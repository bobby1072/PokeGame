using Microsoft.AspNetCore.SignalR;
using PokeGame.Core.SignalR.Filters;

namespace PokeGame.Core.SignalR.Extensions;

internal static class SignalRServiceCollectionExtensions
{
    public static IServiceCollection AddPokeGameSignalR(this IServiceCollection services, TimeSpan timeout)
    {
        services.AddSignalR(opts =>
        {
            opts.HandshakeTimeout = timeout;
            opts.ClientTimeoutInterval = timeout;
            opts.AddFilter<CorrelationIdFilter>();
        });
        
        return services;
    }
}