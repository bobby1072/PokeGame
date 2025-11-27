using System.Net;
using Microsoft.Extensions.Logging;

namespace PokeGame.Core.Common.Exceptions;

public sealed class PokeGameApiServerException: PokeGameApiException
{
    public PokeGameApiServerException(HttpStatusCode statusCode, LogLevel overrideLogLevel,string message): base(statusCode, overrideLogLevel, message) {}    
    public PokeGameApiServerException(string message): base(HttpStatusCode.InternalServerError, LogLevel.Error, message) {}
    public PokeGameApiServerException(string message, Exception innerException): base(HttpStatusCode.InternalServerError, LogLevel.Error, message) {}
}