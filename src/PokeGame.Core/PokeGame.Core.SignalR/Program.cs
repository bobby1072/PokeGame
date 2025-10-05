using System.Text.Json;
using BT.Common.Api.Helpers.Extensions;
using BT.Common.Helpers;
using Microsoft.AspNetCore.Http.Timeouts;
using PokeGame.Core.Domain.Services.Extensions;
using PokeGame.Core.SignalR.Extensions;

var builder = WebApplication.CreateBuilder(args);

var localLogger = LoggingHelper.CreateLogger();

try
{
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

    builder.Services.AddLogging(opts =>
    {
        opts.ClearProviders();
        opts.AddJsonConsole(ctx =>
        {
            ctx.IncludeScopes = true;
            ctx.UseUtcTimestamp = true;
        });
    });

    builder.Services.AddResponseCompression();

    builder.Services.AddPokeGameSignalR(requestTimeoutSpan);
    
    localLogger.LogInformation(
        "About to build application with {NumberOfServices} services",
        builder.Services.Count
    );
    
    var app = builder.Build();
    
    app.UseRouting();

    app.UseResponseCompression();

    app.UseHttpsRedirection();

    app.UseAuthorization();

    app
        .UseCorrelationIdMiddleware();
    
    app
        .UseHealthGetEndpoint();

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
