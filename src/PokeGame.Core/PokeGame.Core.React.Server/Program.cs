using System.Text.Json;
using BT.Common.Api.Helpers.Extensions;
using BT.Common.Api.Helpers.Models;
using BT.Common.Helpers;
using BT.Common.Helpers.Extensions;
using Microsoft.AspNetCore.Http.Timeouts;
using Microsoft.Net.Http.Headers;

var localLogger = LoggingHelper.CreateLogger();

try
{
    var builder = WebApplication.CreateBuilder(args);
    builder.WebHost.ConfigureKestrel(server => server.AddServerHeader = false);

    builder.Configuration
        .SetBasePath(Directory.GetCurrentDirectory())
        .AddJsonFile("reactserversettings.json", optional: false, reloadOnChange: true)
        .AddEnvironmentVariables();
    
    var serviceInfo = builder.Configuration.GetSection(ServiceInfo.Key);

    if (!serviceInfo.Exists())
    {
        throw new ArgumentNullException(ServiceInfo.Key);
    }

    builder.Services.ConfigureSingletonOptions<ServiceInfo>(serviceInfo);
    
    builder.Services.AddLogging(opts =>
    {
        opts.AddJsonConsole(ctx =>
        {
            ctx.IncludeScopes = true;
            ctx.UseUtcTimestamp = true;
        });
    });
    
    var requestTimeout = builder.Configuration.GetValue<int>("RequestTimeout");

    builder.Services.AddRequestTimeouts(opts =>
    {
        opts.DefaultPolicy = new RequestTimeoutPolicy
        {
            Timeout = TimeSpan.FromSeconds(requestTimeout > 0 ? requestTimeout : 30),
        };
    });
    
    builder.Services
        .AddControllers()
        .AddJsonOptions(opts =>
        {
            opts.JsonSerializerOptions.PropertyNamingPolicy = JsonNamingPolicy.CamelCase;
        });
    
    builder.Services.AddEndpointsApiExplorer();
    builder.Services.AddResponseCompression();
    
    var app = builder.Build();

    app.UseRouting();

    app.UseResponseCompression();
    
    app.UseHttpsRedirection();

    app.UseAuthorization();

    app.UseCorrelationIdMiddleware();

    app.UseHealthGetEndpoint();
    
    #pragma warning disable ASP0014
    app.UseEndpoints(endpoint =>
    {
        endpoint.MapFallbackToFile("index.html");
    });
    #pragma warning restore ASP0014
    app.UseStaticFiles();
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
