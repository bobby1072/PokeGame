using BT.Common.Polly.Extensions;
using Microsoft.Extensions.Hosting;
using PokeGame.Core.Persistence.Configurations;
using PokeGame.Core.Persistence.Migrations.Abstract;

namespace PokeGame.Core.Persistence.Migrations.Concrete
{
    internal sealed class DatabaseMigratorHostedService : BackgroundService
    {
        private readonly IEnumerable<IMigrator> _databaseMigrators;
        private readonly DbMigrationSettings _dbMigrationsConfiguration;
        private readonly IDatabaseMigratorHealthCheck _databaseMigratorHealthCheck;
        public DatabaseMigratorHostedService(
            IEnumerable<IMigrator>? databaseMigrators, 
            DbMigrationSettings dbMigrationsConfiguration,
            IDatabaseMigratorHealthCheck databaseMigratorHealthCheck)
        {
            _databaseMigrators = databaseMigrators ?? new List<IMigrator>();
            _dbMigrationsConfiguration = dbMigrationsConfiguration;
            _databaseMigratorHealthCheck = databaseMigratorHealthCheck;
        }

        protected override async Task ExecuteAsync(CancellationToken cancellationToken)
        {
            if (!_dbMigrationsConfiguration.DoMigration)
            {
                return;
            }
            
            await Task.Delay(2000, cancellationToken);
            var pipeline = _dbMigrationsConfiguration.ToPipeline();
            
            await pipeline.ExecuteAsync(async _ => await Migrate(), cancellationToken);
            
            _databaseMigratorHealthCheck.SetMigrationCompleted(true);
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
