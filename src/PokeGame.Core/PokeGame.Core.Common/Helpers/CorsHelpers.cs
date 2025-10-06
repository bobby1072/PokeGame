using Microsoft.AspNetCore.Builder;
using Microsoft.Extensions.DependencyInjection;

namespace PokeGame.Core.Common.Helpers;

public static class CorsHelpers
{
    private const string developmentCorsPolicy = "DevelopmentCorsPolicy";
    public static IServiceCollection AddLocalDevelopmentCorsPolicy(this IServiceCollection services)
    {
        services.AddCors(p =>
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

        return services;
    }

    public static IApplicationBuilder UseLocalDevelopmentCorsPolicy(this IApplicationBuilder app)
    {
        app.UseCors(developmentCorsPolicy);
        
        return app;
    }
}