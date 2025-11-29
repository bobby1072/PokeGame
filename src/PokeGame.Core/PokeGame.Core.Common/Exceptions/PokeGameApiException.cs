using System.Net;
using Microsoft.Extensions.Logging;

namespace PokeGame.Core.Common.Exceptions;

public abstract class PokeGameApiException: PokeGameException
{
    public HttpStatusCode StatusCode { get; }
    public LogLevel LogLevel { get; }

    public PokeGameApiException(HttpStatusCode statusCode, LogLevel logLevel, string message, Exception? innerException = null) : base(message, innerException)
    {
        StatusCode = statusCode;
        LogLevel = logLevel;
    }
}