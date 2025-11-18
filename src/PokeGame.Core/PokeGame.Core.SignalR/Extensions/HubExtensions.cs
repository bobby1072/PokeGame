using Microsoft.AspNetCore.SignalR;
using System.Reflection;

namespace PokeGame.Core.SignalR.Extensions;

internal static class HubExtensions
{
    public static T? GetMetadata<T>(this HubInvocationContext invocationContext) where T : Attribute
     => invocationContext.HubMethod.GetCustomAttribute<T>() ?? invocationContext.Hub.GetType().GetCustomAttribute<T>();
}