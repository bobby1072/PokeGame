using BT.Common.Api.Helpers.Extensions;
using BT.Common.Helpers;
using Microsoft.Net.Http.Headers;
using PokeGame.Core.Common.Configurations;

var localLogger = LoggingHelper.CreateLogger();

try
{
    var builder = WebApplication.CreateBuilder(args);
    builder.WebHost.ConfigureKestrel(server => server.AddServerHeader = false);

    var serviceInfo = builder.Configuration.GetSection(ServiceInfo.Key);

    if (!serviceInfo.Exists())
    {
        throw new ArgumentNullException(ServiceInfo.Key);
    }

    builder.Services.AddLogging(opts =>
    {
        opts.AddJsonConsole(ctx =>
        {
            ctx.IncludeScopes = true;
            ctx.UseUtcTimestamp = true;
        });
    });
    builder.Services.AddControllers();
    builder.Services.AddEndpointsApiExplorer();

    var app = builder.Build();

    app.UseRouting();
    
    app.UseHttpsRedirection();

    app.UseAuthorization();

    app.UseCorrelationIdMiddleware();

    app.MapControllers();
    
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
