namespace TestLibrary.Database
{
    /// <summary>
    /// Database settings related to connectivity/naming. This should be defined by the test project
    /// that is making use of an integration test database instance.
    /// </summary>
    public sealed class DatabaseConfiguration
    {
        /// <summary>
        /// Should be the local DB unless there is a compelling reason to target another instance.
        /// EXAMPLE: (localDb)\\MSSQLLocalDB
        /// </summary>
        public string DatabaseServer { get; set; }

        /// <summary>
        /// Arbitrary, but should be something relevant to the project.
        /// EXAMPLE: DataAccess.IntegrationTests
        /// </summary>
        public string DatabaseName { get; set; }
    }
}
