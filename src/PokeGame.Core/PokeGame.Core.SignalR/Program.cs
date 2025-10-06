using BT.Common.Api.Helpers.Extensions;
using BT.Common.Helpers;
using Microsoft.AspNetCore.Http.Timeouts;
using PokeGame.Core.Domain.Services.Extensions;
using PokeGame.Core.SignalR.Extensions;
using PokeGame.Core.SignalR.Hubs;

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
    
    const string developmentCorsPolicy = "DevelopmentCorsPolicy";

    builder.Services.AddCors(p =>
    {
        p.AddPolicy(
            developmentCorsPolicy,
            opts =>
            {
                opts.AllowAnyOrigin().AllowAnyMethod().AllowAnyHeader();

                opts.WithOrigins("http://localhost:3000").AllowCredentials();
                opts.WithOrigins("http://localhost:8080").AllowCredentials();
                opts.WithOrigins("https://localhost:7070").AllowCredentials();
            }
        );
    });
    
    
    localLogger.LogInformation(
        "About to build application with {NumberOfServices} services",
        builder.Services.Count
    );
    
    var app = builder.Build();

    if (app.Environment.IsDevelopment())
    {
        app.UseCors(developmentCorsPolicy);
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
