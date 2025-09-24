using BT.Common.OperationTimer.Proto;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using PokeGame.Core.Domain.Services.Abstract;
using PokeGame.Core.Domain.Services.Models;

namespace PokeGame.Core.Domain.Services.Concrete;

internal sealed class ScopedDomainServiceCommandExecutor: IScopedDomainServiceCommandExecutor
{
    private readonly IServiceProvider _serviceProvider;
    private readonly ILogger<ScopedDomainServiceCommandExecutor> _logger;
    public ScopedDomainServiceCommandExecutor(IServiceProvider serviceProvider, ILogger<ScopedDomainServiceCommandExecutor> logger)
    {
        _serviceProvider = serviceProvider;
        _logger = logger;
    }

    public async Task<TOutput> RunCommandAsync<TCommand, TInput, TOutput>(TInput input,
        Func<IServiceProvider, TCommand> commandBuilder) where TCommand : IDomainCommand<TInput, TOutput>
        where TOutput : DomainCommandResult
    {
        var foundCommand = commandBuilder.Invoke(_serviceProvider);
        
        _logger.LogInformation("Attempting to execute {CommandName}", foundCommand.CommandName);
        
        var (timeTaken, result) = await OperationTimerUtils.TimeWithResultsAsync(() => foundCommand.ExecuteAsync(input));
        
        _logger.LogInformation("Executed {CommandName} in {TimeTaken}ms", foundCommand.CommandName, timeTaken.Milliseconds);

        return result;
    }

    public async Task<TOutput> RunCommandAsync<TCommand, TOutput>(Func<IServiceProvider, TCommand> commandBuilder)
        where TCommand : IDomainCommand<TOutput> where TOutput : DomainCommandResult
    {
        var foundCommand = commandBuilder.Invoke(_serviceProvider);
        
        _logger.LogInformation("Attempting to execute {CommandName}", foundCommand.CommandName);
        
        var (timeTaken, result) = await OperationTimerUtils.TimeWithResultsAsync(() => foundCommand.ExecuteAsync());
        
        _logger.LogInformation("Executed {CommandName} in {TimeTaken}ms", foundCommand.CommandName, timeTaken.Milliseconds);

        return result;
    }
    public async Task<TOutput> RunCommandAsync<TCommand, TInput, TOutput>(TInput input) where TCommand : IDomainCommand<TInput, TOutput>
        where TOutput : DomainCommandResult
    {
        var foundCommand = _serviceProvider.GetRequiredService<TCommand>();

        _logger.LogInformation("Attempting to execute {CommandName}", foundCommand.CommandName);
        
        var (timeTaken, result) = await OperationTimerUtils.TimeWithResultsAsync(() => foundCommand.ExecuteAsync(input));
        
        _logger.LogInformation("Executed {CommandName} in {TimeTaken}ms", foundCommand.CommandName, timeTaken.Milliseconds);

        return result;
    }

    public async Task<TOutput> RunCommandAsync<TCommand, TOutput>() where TCommand : IDomainCommand<TOutput>
        where TOutput : DomainCommandResult
    {
        var foundCommand = _serviceProvider.GetRequiredService<TCommand>();

        _logger.LogInformation("Attempting to execute {CommandName}", foundCommand.CommandName);
        
        var (timeTaken, result) = await OperationTimerUtils.TimeWithResultsAsync(() => foundCommand.ExecuteAsync());
        
        _logger.LogInformation("Executed {CommandName} in {TimeTaken}ms", foundCommand.CommandName, timeTaken.Milliseconds);

        return result;
    }
    
}