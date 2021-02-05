using System;
using System.Diagnostics;
using System.IO;
using DbUp;
using Microsoft.Data.SqlClient;
using TestLibrary.Logging;

namespace TestLibrary.Database
{
    /// <summary>
    /// An integration test database instance. DB is created anew in the constructor and can then be interacted with
    /// in whatever way makes sense (directly, via production code, etc.) The DB will typically be used from within
    /// a fixture where the fixture has the responsibility for:
    ///   1. Creating the DB with the desired configuration
    ///   2. Creating a snapshot
    ///   3. Disposing of the DB
    /// </summary>
    public sealed class IntegrationTestDatabase : IDisposable
    {
        private readonly ILog _log;
        private readonly Stopwatch _sw;
        private readonly string _connectionString;
        private readonly TestScriptRetriever _testScriptRetriever;
        private readonly Snapshot _snapshot;

        private IntegrationTestDatabase(DatabaseDefinition definition, DatabaseConfiguration configuration)
        {
            _sw = Stopwatch.StartNew();

            _log = LogFactory.ExecutionLog;
            _log.WriteInformation($"Constructing integration test database: {configuration.DatabaseName}");

            DatabaseName = configuration.DatabaseName;

            var connectionStringBuilder = new SqlConnectionStringBuilder
            {
                DataSource = configuration.DatabaseServer,
                InitialCatalog = DatabaseName,
                IntegratedSecurity = true,
                ConnectTimeout = 60,

                // Disabling pooling is essential to the proper functioning of these tests. When connection pooling is
                // active, the tests begin to experience failures performing things like snapshot restores because
                // there are open connections to the DB.
                Pooling = false,
            };
            _connectionString = connectionStringBuilder.ConnectionString;
            DatabaseUtils = new DatabaseUtils(_connectionString);

            _testScriptRetriever = new TestScriptRetriever(this.GetType().Assembly, definition.TestScriptNamespace);

            string snapshotName = $"Snapshot-{DatabaseName}";
            string snapshotPath = Path.Combine(Path.GetTempPath(), snapshotName + ".ss");
            _snapshot = new Snapshot(snapshotName, DatabaseName, snapshotPath);

            // Clean starting state = make sure the old is blown away if it exists (from a previous test run)
            DatabaseUtils.DropSnapshotIfExists(_snapshot);
            DatabaseUtils.DropDatabaseIfExists();
            DatabaseUtils.CreateDatabase();

            var resources = SqlEmbeddedScriptFactory.GetEmbeddedSqlScripts(this.GetType().Assembly, definition.LinkedResourceNamespace);

            // Invoke DbUp to perform the "migration". Since the fixture always begins with an empty database, the
            // migration always runs every script in the specified order
            var upgrader = DeployChanges.To
                .SqlDatabase(_connectionString)
                .WithScripts(SqlScriptSelector.FilterAndOrderScripts(resources, definition.OrderedResources))
                .LogTo(UpgradeLogAdapter.Adapt(_log))
                .Build();

            _log.WriteInformation("Deploying database artifacts");
            var result = upgrader.PerformUpgrade();
            DeploymentSuccessful = result.Successful;

            _log.WriteInformation("Integration test database construction finished");
        }

        public static IntegrationTestDatabase Create(DatabaseDefinition definition, DatabaseConfiguration configuration)
        {
            return new IntegrationTestDatabase(definition, configuration);
        }

        public void Dispose()
        {
            _log.WriteInformation($"Disposing integration test database: {DatabaseName}");

            DatabaseUtils.DropSnapshotIfExists(_snapshot);
            DatabaseUtils.DropDatabaseIfExists();

            _log.WriteInformation($"Integration test database lifespan: {_sw.ElapsedMilliseconds} ms");
        }

        /// <summary>
        /// Provides a hook for a sanity check. Tests should fail fast if fixture setup failed.
        /// </summary>
        public bool DeploymentSuccessful { get; }

        public DatabaseUtils DatabaseUtils { get; }

        public string DatabaseName { get; }

        public string ConnectionString
        {
            get
            {
                if (!DeploymentSuccessful)
                {
                    throw new InvalidOperationException("Fixture database deployment failed -- tests cannot run");
                }

                return _connectionString;
            }
        }

        public void CreateSnapshot()
        {
            DatabaseUtils.CreateSnapshot(_snapshot);
        }

        public void RestoreSnapshot()
        {
            DatabaseUtils.RestoreSnapshot(_snapshot);
        }

        /// <summary>
        /// Executes a test script at an arbitrary time. This can be useful when a test wants to perform DB work (such
        /// as loading data into tables) and wants to use SQL scripts as the tool. These scripts need to be embedded in
        /// the test project to be discoverable.
        /// </summary>
        public void RunTestScript(string scriptName)
        {
            string content = _testScriptRetriever.ReadTestScript(scriptName);
            if (content == null)
            {
                throw new InvalidOperationException($"Failed to find test script: {scriptName}");
            }

            DatabaseUtils.RunTestScript(scriptName, content);
        }
    }
}
