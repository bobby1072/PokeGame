using BT.Common.Api.Helpers.Extensions;
using BT.Common.Helpers;
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

    app.UseHttpsRedirection();

    app.UseAuthorization();

    app.UseCorrelationIdMiddleware();

    app.MapControllers();

    app.Run();
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
