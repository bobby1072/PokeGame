using PokeGame.Core.ConsoleApp.Services.Abstract;

namespace PokeGame.Core.ConsoleApp.Services.Concrete;

internal sealed class ConsoleApplicationOrchestratorService: IConsoleApplicationOrchestratorService
{
    private readonly IServiceProvider _serviceProvider;
    public ConsoleApplicationOrchestratorService(IServiceProvider serviceProvider)
    {
        _serviceProvider = serviceProvider;
    }
    
    public Task StartConsoleApplicationAsync(Action stopApplicationAction)
    {
        return Task.CompletedTask;
    }
}