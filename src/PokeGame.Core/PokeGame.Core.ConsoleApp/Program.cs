using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using PokeGame.Core.ConsoleApp.Services.Abstract;
using PokeGame.Core.ConsoleApp.Services.Concrete;
using PokeGame.Core.Domain.Services.Extensions;

using var consoleHost = Host.CreateDefaultBuilder()
    .ConfigureAppConfiguration(cfg =>
    {
        cfg
            .SetBasePath(Directory.GetCurrentDirectory())
            .AddJsonFile("appsettings.json", optional: false, reloadOnChange: true);
    })
    .ConfigureServices((ctx, sc) =>
    {
        sc
            .AddPokeGameApplicationServices(ctx.Configuration, ctx.HostingEnvironment)
            .AddScoped<IConsoleApplicationOrchestratorService, ConsoleApplicationOrchestratorService>()
            .AddHostedService<ConsoleApplicationRunnerService>();
    })
    .ConfigureLogging(lg =>
    {
        lg.SetMinimumLevel(LogLevel.None);
    })
    .Build();
    
await consoleHost.RunAsync();