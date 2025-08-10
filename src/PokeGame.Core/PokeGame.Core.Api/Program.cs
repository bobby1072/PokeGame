using System.Text.Json;
using BT.Common.Api.Helpers.Extensions;
using BT.Common.Helpers;
using Microsoft.AspNetCore.Http.Timeouts;
using PokeGame.Core.Api.Middlewares;
using PokeGame.Core.Domain.Services.Extensions;

var localLogger = LoggingHelper.CreateLogger();

try
{
    localLogger.LogInformation("Application starting...");

    var builder = WebApplication.CreateBuilder(args);
    builder.WebHost.ConfigureKestrel(options => options.AddServerHeader = false);

    builder.Services.AddPokeGameApplicationServices(builder.Configuration, builder.Environment);

    var requestTimeout = builder.Configuration.GetValue<int>("RequestTimeout");

    builder.Services.AddRequestTimeouts(opts =>
    {
        opts.DefaultPolicy = new RequestTimeoutPolicy
        {
            Timeout = TimeSpan.FromSeconds(requestTimeout > 0 ? requestTimeout : 30),
        };
    });

    builder.Services.AddLogging(opts =>
    {
        opts.AddJsonConsole(ctx =>
        {
            ctx.IncludeScopes = true;
            ctx.UseUtcTimestamp = true;
        });
    });

    builder
        .Services.AddControllers()
        .AddJsonOptions(opts =>
        {
            opts.JsonSerializerOptions.PropertyNamingPolicy = JsonNamingPolicy.CamelCase;
        });

    builder.Services.AddEndpointsApiExplorer();
    builder.Services.AddSwaggerGen();
    builder.Services.AddResponseCompression();

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
                opts.WithOrigins("http://localhost:7070").AllowCredentials();
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
        app.UseSwagger();
        app.UseSwaggerUI();
        app.UseCors(developmentCorsPolicy);
    }
    app.UseRouting();

    app.UseResponseCompression();

    app.UseHttpsRedirection();

    app.UseAuthorization();

    app.UseMiddleware<ExceptionHandlingMiddleware>().UseCorrelationIdMiddleware();

    app.MapControllers();

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
