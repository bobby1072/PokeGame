namespace PokeGame.Core.Domain.Services.Models;

internal record DomainCommandResult
{ }

internal sealed record DomainCommandResult<T> : DomainCommandResult
{
    public required T CommandResult { get; init; }
}