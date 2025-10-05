using BT.Common.Api.Helpers.Models;

namespace PokeGame.Core.SignalR.Models;

public sealed record SignalRClientEvent: WebOutcome
{ }

public sealed record SignalRServerEvent<T> : WebOutcome<T>
{ }