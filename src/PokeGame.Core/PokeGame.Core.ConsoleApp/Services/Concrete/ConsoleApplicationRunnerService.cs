using Microsoft.Extensions.Hosting;

namespace PokeGame.Core.ConsoleApp.Services.Concrete;

internal sealed class ConsoleApplicationRunnerService: IHostedService
{
    public Task StartAsync(CancellationToken cancellationToken)
    {
        throw new NotImplementedException();
    }
    public Task StopAsync(CancellationToken cancellationToken)
    {
        throw new NotImplementedException();
    }
}