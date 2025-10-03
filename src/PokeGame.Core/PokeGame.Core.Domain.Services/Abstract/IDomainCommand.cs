using PokeGame.Core.Domain.Services.Models;

namespace PokeGame.Core.Domain.Services.Abstract;

internal interface IDomainCommand
{
    string CommandName { get; }
}

internal interface IDomainCommand<TOutput> : IDomainCommand where TOutput : DomainCommandResult
{
    Task<TOutput> ExecuteAsync(CancellationToken cancellationToken = default);
}

internal interface IDomainCommand<in TInput, TOutput> : IDomainCommand where TOutput: DomainCommandResult
{
    Task<TOutput> ExecuteAsync(TInput input, CancellationToken cancellationToken = default);
}