using BT.Common.Helpers.Extensions;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using Npgsql;
using PokeGame.Core.Persistence.Configurations;
using PokeGame.Core.Persistence.Contexts;
using PokeGame.Core.Persistence.Migrations.Abstract;
using PokeGame.Core.Persistence.Migrations.Concrete;
using PokeGame.Core.Persistence.Repositories.Abstract;
using PokeGame.Core.Persistence.Repositories.Concrete;

namespace PokeGame.Core.Persistence.Extensions;

public static class PersistenceServiceCollectionExtensions
{
    public static IServiceCollection AddPokeGamePersistence(
        this IServiceCollection services,
        IConfiguration configuration,
        bool isDevelopment = true
    )
    {
        var connectionString = configuration.GetConnectionString("PostgresConnection");
        var migrationConfigSection = configuration.GetSection(DbMigrationSettings.Key);
        if (!migrationConfigSection.Exists())
        {
            throw new ArgumentNullException(DbMigrationSettings.Key);
        }

        services.ConfigureSingletonOptions<DbMigrationSettings>(migrationConfigSection);

        var connectionStringBuilder = new NpgsqlConnectionStringBuilder(connectionString);

        services
            .AddSingleton<IDatabaseMigratorHealthCheck, DatabaseMigratorHealthCheck>()
            .AddHealthChecks()
            .AddCheck<IDatabaseMigratorHealthCheck>(
                nameof(DatabaseMigratorHealthCheck)
            );

        services
            .AddSingleton<IMigrator, DatabaseMigrator>(sp => new DatabaseMigrator(
                sp.GetRequiredService<ILoggerFactory>().CreateLogger<DatabaseMigrator>(),
                sp.GetRequiredService<DbMigrationSettings>(),
                connectionStringBuilder.ConnectionString
            ))
            .AddHostedService<DatabaseMigratorHostedService>()
            .AddPooledDbContextFactory<PokeGameContext>(options =>
            {
                if (isDevelopment)
                {
                    options.EnableDetailedErrors().EnableSensitiveDataLogging();
                }
                options
                    .UseSnakeCaseNamingConvention()
                    .UseQueryTrackingBehavior(QueryTrackingBehavior.NoTracking)
                    .UseNpgsql(
                        connectionStringBuilder.ConnectionString,
                        npgOpts =>
                        {
                            npgOpts.UseQuerySplittingBehavior(QuerySplittingBehavior.SingleQuery);
                        }
                    );
            });

        services
            .AddScoped<IUserRepository, UserRepository>()
            .AddScoped<IPokedexPokemonRepository, PokedexPokemonRepository>();

        return services;
    }
}
