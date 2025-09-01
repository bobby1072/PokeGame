using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Diagnostics.HealthChecks;
using Microsoft.Extensions.Hosting;
using PokeGame.Core.ConsoleApp.Helpers;
using PokeGame.Core.ConsoleApp.Services.Abstract;
using PokeGame.Core.Domain.Services.Pokedex.Abstract;

namespace PokeGame.Core.ConsoleApp.Services.Concrete;

internal sealed class ConsoleApplicationRunnerService: IHostedService
{
    private readonly IHostApplicationLifetime _appLifetime;
    private readonly IPokedexDataMigratorHealthCheck _pokedexDataMigratorHealthCheck;
    private readonly IServiceScopeFactory _serviceScopeFactory;
    public ConsoleApplicationRunnerService(IHostApplicationLifetime appLifetime, 
        IPokedexDataMigratorHealthCheck pokedexDataMigratorHealthCheck, 
        IServiceScopeFactory serviceScopeFactory)
    {
        _appLifetime = appLifetime;
        _pokedexDataMigratorHealthCheck = pokedexDataMigratorHealthCheck;
        _serviceScopeFactory = serviceScopeFactory;
    }
    public async Task StartAsync(CancellationToken cancellationToken)
    {
        while ((await _pokedexDataMigratorHealthCheck.CheckHealthAsync(new HealthCheckContext(), cancellationToken)).Status !=
               HealthStatus.Healthy)
        {
            await Task.Delay(TimeSpan.FromSeconds(0.1), cancellationToken);
        }
        
        Console.WriteLine($"{ConsoleHelper.GetConsoleNewLine()}Application starting...{ConsoleHelper.GetConsoleNewLine()}");
        
        await using var scope = _serviceScopeFactory.CreateAsyncScope();
        var orchestratorService = scope.ServiceProvider.GetRequiredService<IConsoleApplicationOrchestratorService>();
        
        await orchestratorService.StartConsoleApplicationAsync(_appLifetime.StopApplication);
    }

    public Task StopAsync(CancellationToken cancellationToken)
    {   
        Console.WriteLine($"{ConsoleHelper.GetConsoleNewLine()}Application exiting...{ConsoleHelper.GetConsoleNewLine()}");
        
        return Task.CompletedTask;
    }
}