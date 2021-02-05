using Microsoft.Data.SqlClient;
using System.Diagnostics;
using System.Text;
using DbUp;
using TestLibrary.Logging;

namespace TestLibrary.Database
{
    public sealed class DatabaseUtils
    {
        private readonly string _connectionString;
        private readonly string _masterConnectionString;
        private readonly string _databaseName;

        public DatabaseUtils(string connectionString)
        {
            var builder = new SqlConnectionStringBuilder(connectionString)
            {
                InitialCatalog = "master"
            };

            _connectionString = connectionString;
            _masterConnectionString = builder.ConnectionString;
            _databaseName = new SqlConnectionStringBuilder(connectionString).InitialCatalog;
        }

        public void CreateDatabase()
        {
            Stopwatch sw = Stopwatch.StartNew();
            LogFactory.ExecutionLog.WriteInformation($"Creating database: {_databaseName}");

            EnsureDatabase.For.SqlDatabase(_connectionString);

            LogFactory.ExecutionLog.WriteInformation($"Created database: {_databaseName} [{sw.ElapsedMilliseconds} ms]");
        }

        public void DropDatabaseIfExists()
        {
            Stopwatch sw = Stopwatch.StartNew();
            LogFactory.ExecutionLog.WriteInformation($"Dropping database: {_databaseName}");

            string command = $"IF EXISTS(SELECT * FROM sys.databases WHERE name='{_databaseName}') DROP DATABASE [{_databaseName}] ";

            RunCommand(_masterConnectionString, command);

            LogFactory.ExecutionLog.WriteInformation($"Dropped database: {_databaseName} [{sw.ElapsedMilliseconds} ms]");
        }

        /// <summary>
        /// Executes a test script at an arbitrary time. This can be useful when a test wants to perform DB work (such
        /// as loading data into tables) and wants to use SQL scripts as the tool. These scripts need to be embedded in
        /// the test project to be discoverable.
        /// </summary>
        public void RunTestScript(string scriptName, string scriptContent)
        {
            Stopwatch sw = Stopwatch.StartNew();
            LogFactory.ExecutionLog.WriteInformation($"Running test script: {scriptName}");

            RunCommand(_connectionString, scriptContent);

            LogFactory.ExecutionLog.WriteInformation($"Ran test script: {scriptName} [{sw.ElapsedMilliseconds} ms]");
        }

        public void CreateSnapshot(Snapshot snapshot)
        {
            Stopwatch sw = Stopwatch.StartNew();
            LogFactory.ExecutionLog.WriteInformation("Creating snapshot");

            var sb = new StringBuilder();
            sb.Append($"CREATE DATABASE [{snapshot.Name}] ON ");
            sb.Append($@"( NAME = '{snapshot.Database}', FILENAME = '{snapshot.FilePath}' ) ");
            sb.Append($"AS SNAPSHOT OF [{snapshot.Database}]; ");
            sb.Append($"USE [{snapshot.Database}]; ");

            RunCommand(_masterConnectionString, sb.ToString());

            LogFactory.ExecutionLog.WriteInformation($"Created snapshot [{sw.ElapsedMilliseconds} ms]");
        }

        public void RestoreSnapshot(Snapshot snapshot)
        {
            Stopwatch sw = Stopwatch.StartNew();
            LogFactory.ExecutionLog.WriteInformation("Restoring snapshot");

            var sb = new StringBuilder();
            sb.Append($"RESTORE DATABASE [{snapshot.Database}] FROM ");
            sb.Append($"DATABASE_SNAPSHOT = '{snapshot.Name}'; ");
            sb.Append($"USE [{snapshot.Database}]; ");

            RunLoggedCommand(_masterConnectionString, sb.ToString());

            LogFactory.ExecutionLog.WriteInformation($"Restored snapshot [{sw.ElapsedMilliseconds} ms]");
        }

        public void DropSnapshotIfExists(Snapshot snapshot)
        {
            Stopwatch sw = Stopwatch.StartNew();
            LogFactory.ExecutionLog.WriteInformation("Dropping snapshot");

            var sb = new StringBuilder();
            sb.Append($"IF EXISTS(SELECT * FROM sys.databases WHERE name='{snapshot.Name}') ");
            sb.Append($"DROP DATABASE [{snapshot.Name}] ");

            RunLoggedCommand(_masterConnectionString, sb.ToString());

            LogFactory.ExecutionLog.WriteInformation($"Dropped snapshot [{sw.ElapsedMilliseconds} ms]");
        }

        private static void RunLoggedCommand(string connectionString, string commandText)
        {
            RunCommand(connectionString, commandText, true);
        }

        private static void RunCommand(string connectionString, string commandText, bool logged = false)
        {
            if (logged)
            {
                LogFactory.ExecutionLog.WriteInformation($"SQL: {commandText}");
            }

            using (var conn = new SqlConnection(connectionString))
            {
                conn.Open();

                using (var cmd = conn.CreateCommand())
                {
                    cmd.CommandText = commandText;
                    cmd.ExecuteNonQuery();
                }
            }
        }
    }
}
