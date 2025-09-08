namespace PokeGame.Core.ConsoleApp.Services.Abstract;

internal interface IConsoleApplicationOrchestratorService
{
    Task StartConsoleApplicationAsync(Action stopApplicationAction);
}