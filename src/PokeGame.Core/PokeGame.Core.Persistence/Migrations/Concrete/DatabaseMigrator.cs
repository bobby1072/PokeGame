using BT.Common.Polly.Extensions;
using EvolveDb;
using EvolveDb.Migration;
using Microsoft.Extensions.Logging;
using Npgsql;
using PokeGame.Core.Persistence.Configurations;
using PokeGame.Core.Persistence.Migrations.Abstract;

namespace PokeGame.Core.Persistence.Migrations.Concrete
{
    internal sealed class DatabaseMigrator : IMigrator
    {
        private readonly ILogger<DatabaseMigrator> _logger;
        private readonly DbMigrationSettings _settings;
        private readonly string _connectionString;
        public DatabaseMigrator(ILogger<DatabaseMigrator> logger, DbMigrationSettings migrationSettings, string connectionString)
        {
            _logger = logger;
            _connectionString = connectionString;
            _settings = migrationSettings;
        }
        public Task Migrate()
        {
            using var connection = new NpgsqlConnection(_connectionString);
            var evolve = new Evolve(connection, msg => _logger.LogInformation(msg))
            {
                EmbeddedResourceAssemblies = [typeof(DatabaseMigrator).Assembly],
                EnableClusterMode = true,
                StartVersion = new MigrationVersion(_settings.StartVersion),
                IsEraseDisabled = true,
                MetadataTableName = "migrations_changelog",
                OutOfOrder = true
            };

            _settings.ToPipeline().Execute(() => evolve.Migrate());

            return Task.CompletedTask;
        }
    }
}
