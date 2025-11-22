using System.Diagnostics;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using PokeGame.Core.Domain.Services.Abstract;
using PokeGame.Core.Domain.Services.Models;

namespace PokeGame.Core.Domain.Services.Concrete;

internal sealed class DomainServiceCommandExecutor: IDomainServiceCommandExecutor
{
    private readonly IServiceProvider _serviceProvider;
    private readonly ILogger<DomainServiceCommandExecutor> _logger;
    public DomainServiceCommandExecutor(IServiceProvider serviceProvider, ILogger<DomainServiceCommandExecutor> logger)
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

        var timeTaken = Stopwatch.StartNew();
        var result = await foundCommand.ExecuteAsync(input);
        timeTaken.Stop();
        
        _logger.LogInformation("Executed {CommandName} in {TimeTaken}ms", foundCommand.CommandName, timeTaken.ElapsedMilliseconds);

        return result;
    }

    public async Task<TOutput> RunCommandAsync<TCommand, TOutput>(Func<IServiceProvider, TCommand> commandBuilder)
        where TCommand : IDomainCommand<TOutput> where TOutput : DomainCommandResult
    {
        var foundCommand = commandBuilder.Invoke(_serviceProvider);
        
        _logger.LogInformation("Attempting to execute {CommandName}", foundCommand.CommandName);
        
        var timeTaken = Stopwatch.StartNew();
        var result = await foundCommand.ExecuteAsync();
        timeTaken.Stop();
        
        _logger.LogInformation("Executed {CommandName} in {TimeTaken}ms", foundCommand.CommandName, timeTaken.ElapsedMilliseconds);

        return result;
    }
    public async Task<TOutput> RunCommandAsync<TCommand, TInput, TOutput>(TInput input) where TCommand : IDomainCommand<TInput, TOutput>
        where TOutput : DomainCommandResult
    {
        var foundCommand = _serviceProvider.GetRequiredService<TCommand>();

        _logger.LogInformation("Attempting to execute {CommandName}", foundCommand.CommandName);
        
        var timeTaken = Stopwatch.StartNew();
        var result = await foundCommand.ExecuteAsync(input);
        timeTaken.Stop();
        
        _logger.LogInformation("Executed {CommandName} in {TimeTaken}ms", foundCommand.CommandName, timeTaken.ElapsedMilliseconds);

        return result;
    }

    public async Task<TOutput> RunCommandAsync<TCommand, TOutput>() where TCommand : IDomainCommand<TOutput>
        where TOutput : DomainCommandResult
    {
        var foundCommand = _serviceProvider.GetRequiredService<TCommand>();

        _logger.LogInformation("Attempting to execute {CommandName}", foundCommand.CommandName);
        
        var timeTaken = Stopwatch.StartNew();
        var result = await foundCommand.ExecuteAsync();
        timeTaken.Stop();
        
        _logger.LogInformation("Executed {CommandName} in {TimeTaken}ms", foundCommand.CommandName, timeTaken.ElapsedMilliseconds);

        return result;
    }
    
}