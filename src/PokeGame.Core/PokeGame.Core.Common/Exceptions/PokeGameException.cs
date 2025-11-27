namespace PokeGame.Core.Common.Exceptions;

public abstract class PokeGameException: Exception
{
    protected PokeGameException(string message, Exception? innerException) : base(message, innerException) { }
    protected PokeGameException(string message) : base(message) { }
}