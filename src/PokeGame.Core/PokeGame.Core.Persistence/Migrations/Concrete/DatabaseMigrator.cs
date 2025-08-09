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
        private readonly string _connectionString;
        private readonly string _startVersion;
        public DatabaseMigrator(ILogger<DatabaseMigrator> logger, DbMigrationSettings migrationSettings, string connectionString)
        {
            _logger = logger;
            _startVersion = migrationSettings.StartVersion;
            _connectionString = connectionString;
        }
        public Task Migrate()
        {
            using var connection = new NpgsqlConnection(_connectionString);
            var evolve = new Evolve(connection, msg => _logger.LogInformation(msg))
            {
                EmbeddedResourceAssemblies = new[] { typeof(DatabaseMigrator).Assembly },
                EnableClusterMode = true,
                StartVersion = new MigrationVersion(_startVersion),
                IsEraseDisabled = true,
                MetadataTableName = "migrations_changelog",
                OutOfOrder = true
            };
            evolve.Migrate();
            return Task.CompletedTask;
        }
    }
}
