using PokeGame.Core.ConsoleApp.Services.Abstract;

namespace PokeGame.Core.ConsoleApp.Services.Concrete;

internal sealed class ConsoleApplicationOrchestratorService: IConsoleApplicationOrchestratorService
{
    public Task StartConsoleApplicationAsync(Action stopApplicationAction)
    {
        return Task.CompletedTask;
    }
}