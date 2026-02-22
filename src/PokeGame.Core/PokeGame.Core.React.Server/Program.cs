using System.Reflection;
using BT.Common.Api.Helpers.Extensions;
using BT.Common.Api.Helpers.Models;
using BT.Common.Helpers;
using BT.Common.Helpers.Extensions;
using BT.Common.Services.Extensions;
using Microsoft.AspNetCore.Http.Timeouts;
using Microsoft.Net.Http.Headers;
using PokeGame.Core.React.Server.Configuration;
using PokeGame.Core.React.Server.Services.Abstract;
using PokeGame.Core.React.Server.Services.Concrete;

var localLogger = LoggingHelper.CreateLogger();

try
{
    localLogger.LogInformation("Application starting...");
    var builder = WebApplication.CreateBuilder(args);
    builder.WebHost.ConfigureKestrel(server => server.AddServerHeader = false);

    var serviceOpts = builder.CheckAndAddSingletonOptions<ServiceInfo>();

    builder.Logging.AddJsonLogging();

    var requestTimeout = builder.Configuration.GetValue<int>("RequestTimeout");

    builder.Services.AddRequestTimeouts(opts =>
    {
        opts.DefaultPolicy = new RequestTimeoutPolicy
        {
            Timeout = TimeSpan.FromSeconds(requestTimeout > 0 ? requestTimeout : 30),
        };
    });

    builder.Services
        .AddTelemetryService(
            serviceOpts.ReleaseName
        );
    
    builder.Services.AddEndpointsApiExplorer();
    builder.Services.AddResponseCompression();
    builder.Services.AddHealthChecks();
    builder.Services.AddAuthorization();

    var hstsOpts = builder.CheckAndAddSingletonOptions<HstsOptions>();
    
    builder.Services.AddHsts(opts =>
    {
        opts.ExcludedHosts.Clear();
        opts.IncludeSubDomains = hstsOpts.IncludeSubDomains;
        opts.Preload = hstsOpts.Preload;
        opts.MaxAge = TimeSpan.FromSeconds((double)hstsOpts.MaxAgeInSeconds);
    });


    var reactAppSettingsPath =
        Path.GetFullPath(Path.Combine(Directory.GetCurrentDirectory(), "wwwroot", "reactappsettings.json"));
    
    builder.Services.AddTransient<IReactAppSettingsEditor, ReactAppSettingsEditor>(sp => new ReactAppSettingsEditor(
        reactAppSettingsPath,
        sp.GetRequiredService<ILoggerFactory>().CreateLogger<ReactAppSettingsEditor>()
    ));

    builder.CheckAndAddSingletonOptions<PokeGameBackendSettings>();

    builder.Services.AddHostedService<ReactAppSettingsBackgroundEditorExecutor>();
    
    var app = builder.Build();

    app.UseResponseCompression();

    app.UseHsts();

    app.UseHttpsRedirection();

    app.UseStaticFiles();

    app.UseRouting();

    app.UseAuthorization();

    app.UseCorrelationIdMiddleware();

    app.UseHealthGetEndpoints();

#pragma warning disable ASP0014
    app.UseEndpoints(endpoint =>
    {
        endpoint.MapFallbackToFile("index.html");
    });
#pragma warning restore ASP0014
    app.UseSpa(spa =>
    {
        spa.Options.DefaultPageStaticFileOptions = new StaticFileOptions
        {
            OnPrepareResponse = context =>
            {
                var headers = context.Context.Response.GetTypedHeaders();
                if (context.File.Name.EndsWith(".html"))
                {
                    headers.CacheControl = new CacheControlHeaderValue
                    {
                        NoCache = true,
                        NoStore = true,
                        MustRevalidate = true,
                        MaxAge = TimeSpan.Zero,
                    };
                }
                else
                {
                    headers.CacheControl = new CacheControlHeaderValue
                    {
                        Public = true,
                        Private = false,
                        NoCache = false,
                        NoStore = false,
                        MaxAge = TimeSpan.FromDays(365),
                    };
                }
            },
        };
    });

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
