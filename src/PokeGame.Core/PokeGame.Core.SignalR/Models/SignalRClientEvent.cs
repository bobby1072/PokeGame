using BT.Common.Api.Helpers.Models;

namespace PokeGame.Core.SignalR.Models;

internal sealed record SignalRClientEvent: WebOutcome
{ }

internal sealed record SignalRServerEvent<T> : WebOutcome<T>
{ }