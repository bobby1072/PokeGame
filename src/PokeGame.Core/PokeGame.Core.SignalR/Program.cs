using BT.Common.Api.Helpers.Extensions;
using BT.Common.Helpers;
using Microsoft.AspNetCore.Http.Timeouts;
using PokeGame.Core.Common.Helpers;
using PokeGame.Core.Domain.Services.Extensions;
using PokeGame.Core.SignalR.Extensions;
using PokeGame.Core.SignalR.Hubs;


var localLogger = LoggingHelper.CreateLogger();

try
{
    localLogger.LogInformation("Application starting...");
    
    var builder = WebApplication.CreateBuilder(args);
    builder.WebHost.ConfigureKestrel(options => options.AddServerHeader = false);

    builder.Services.AddPokeGameApplicationServices(builder.Configuration, builder.Environment);

    var requestTimeout = builder.Configuration.GetValue<int>("RequestTimeout");
    var requestTimeoutSpan = TimeSpan.FromSeconds(requestTimeout > 0 ? requestTimeout : 30); 
    builder.Services.AddRequestTimeouts(opts =>
    {
        opts.DefaultPolicy = new RequestTimeoutPolicy
        {
            Timeout = requestTimeoutSpan,
        };
    });

    builder.Logging.AddJsonLogging();

    builder.Services.AddResponseCompression();

    builder.Services.AddPokeGameSignalR(requestTimeoutSpan);

    builder.Services.AddLocalDevelopmentCorsPolicy();
    
    localLogger.LogInformation(
        "About to build application with {NumberOfServices} services",
        builder.Services.Count
    );
    
    var app = builder.Build();

    if (app.Environment.IsDevelopment())
    {
        app.UseLocalDevelopmentCorsPolicy();
    }
    
    app.UseRouting();

    app.UseResponseCompression();

    app.UseHttpsRedirection();

    app.UseAuthorization();

    app
        .UseCorrelationIdMiddleware();
    
    app
        .UseHealthGetEndpoints();

    app
        .MapHub<PokeGameSessionHub>("/Api/SignalR/PokeGameSession");
    
    await app.RunAsync();
}
catch (Exception ex)
{
    localLogger.LogCritical(
        ex,
        "Unhandled exception in application with message: {ExMessage}",
        ex.Message
    );
}
finally
{
    localLogger.LogInformation("Application is exiting...");
}
