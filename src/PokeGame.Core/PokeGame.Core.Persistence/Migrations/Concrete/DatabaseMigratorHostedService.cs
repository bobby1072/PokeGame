using BT.Common.Polly.Extensions;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using PokeGame.Core.Persistence.Configurations;
using PokeGame.Core.Persistence.Migrations.Abstract;

namespace PokeGame.Core.Persistence.Migrations.Concrete
{
    internal sealed class DatabaseMigratorHostedService : BackgroundService
    {
        private readonly IEnumerable<IMigrator> _databaseMigrators;
        private readonly DbMigrationSettings _dbMigrationsConfiguration;
        private readonly IDatabaseMigratorHealthCheck _databaseMigratorHealthCheck;
        private readonly ILogger<DatabaseMigratorHostedService> _logger;
        public DatabaseMigratorHostedService(
            IEnumerable<IMigrator>? databaseMigrators, 
            DbMigrationSettings dbMigrationsConfiguration,
            IDatabaseMigratorHealthCheck databaseMigratorHealthCheck,
            ILogger<DatabaseMigratorHostedService> logger)
        {
            _databaseMigrators = databaseMigrators ?? new List<IMigrator>();
            _dbMigrationsConfiguration = dbMigrationsConfiguration;
            _databaseMigratorHealthCheck = databaseMigratorHealthCheck;
            _logger = logger;
        }

        protected override async Task ExecuteAsync(CancellationToken cancellationToken)
        {
            if (!_dbMigrationsConfiguration.DoMigration)
            {
                _databaseMigratorHealthCheck.SetMigrationCompleted(true);
                _logger.LogInformation("Migrations are disabled...");
                return;
            }
            
            await Task.Delay(2000, cancellationToken);
            var pipeline = _dbMigrationsConfiguration.ToPipeline();
            
            await pipeline.ExecuteAsync(async _ => await Migrate(), cancellationToken);
            
            _databaseMigratorHealthCheck.SetMigrationCompleted(true);
            _logger.LogInformation("Migrations have been successfully completed...");
        }

        private async Task Migrate()
        {
            foreach (var migrator in _databaseMigrators)
            {
                await migrator.Migrate();
            }
        }

    }
}
