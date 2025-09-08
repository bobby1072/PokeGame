namespace PokeGame.Core.Domain.Services.Abstract;

internal interface IDomainCommand
{
    string CommandName { get; }
}

internal interface IDomainCommand<in TInput> : IDomainCommand
{
    Task ExecuteAsync(TInput input, CancellationToken cancellationToken = default);
}

internal interface IDomainCommand<in TInput, TOutput> : IDomainCommand
{
    Task<TOutput> ExecuteAsync(TInput id, CancellationToken cancellationToken = default);
}