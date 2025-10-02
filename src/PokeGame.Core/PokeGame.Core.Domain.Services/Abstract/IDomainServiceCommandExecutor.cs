
using PokeGame.Core.Domain.Services.Models;

namespace PokeGame.Core.Domain.Services.Abstract;

internal interface IDomainServiceCommandExecutor
{
    Task<TOutput> RunCommandAsync<TCommand, TInput, TOutput>(TInput input, Func<IServiceProvider, TCommand> commandBuilder) where TCommand : IDomainCommand<TInput, TOutput> where TOutput : DomainCommandResult;
    Task<TOutput> RunCommandAsync<TCommand, TOutput>(Func<IServiceProvider, TCommand> commandBuilder) where TCommand : IDomainCommand<TOutput> where TOutput : DomainCommandResult;
    Task<TOutput> RunCommandAsync<TCommand, TInput, TOutput>(TInput input) where TCommand : IDomainCommand<TInput, TOutput> where TOutput : DomainCommandResult;
    Task<TOutput> RunCommandAsync<TCommand, TOutput>() where TCommand : IDomainCommand<TOutput> where TOutput : DomainCommandResult;
}