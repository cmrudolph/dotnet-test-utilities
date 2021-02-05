namespace TestLibrary.Database
{
    /// <summary>
    /// Represents a DB snapshot. Snapshots are used to bring the database back to a known good state before each
    /// test method executes.
    /// </summary>
    public sealed class Snapshot
    {
        public Snapshot(string name, string database, string filePath)
        {
            Name = name;
            Database = database;
            FilePath = filePath;
        }

        public string Name { get; }

        public string Database { get; }

        public string FilePath { get; }
    }
}
