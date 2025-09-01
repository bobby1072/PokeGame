using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using PokeGame.Core.Domain.Services.Extensions;

using var consoleHost = Host.CreateDefaultBuilder()
    .ConfigureAppConfiguration(cfg =>
    {
        cfg
            .SetBasePath(Directory.GetCurrentDirectory())
            .AddJsonFile("appsettings.json", optional: false, reloadOnChange: true)
            .AddEnvironmentVariables();
    })
    .ConfigureLogging(lg =>
    {
        lg.SetMinimumLevel(LogLevel.None);
    })
    .ConfigureServices((ctx, sc) =>
    {
        sc.AddPokeGameApplicationServices(ctx.Configuration, ctx.HostingEnvironment);
    })
    .Build();
    
await consoleHost.RunAsync();       