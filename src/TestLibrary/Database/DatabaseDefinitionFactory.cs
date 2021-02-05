namespace TestLibrary.Database
{
    /// <summary>
    /// Creates the "default" database definition for tests that want to use the typical database associated with
    /// the boilerplate project. Using this definition means the DB will end up configured using resources
    /// embedded in the library project. This makes the DB setup available to N different test projects without
    /// requiring independent maintenance across said projects. The definition is split off from the integration
    /// test database to enable reuse of the database component in more complex test configurations (e.g. scenario
    /// involving multiple DBs or requiring different artifacts).
    /// </summary>
    public static class DatabaseDefinitionFactory
    {
        public static DatabaseDefinition CreateSharedDefinition()
        {
            return new DatabaseDefinition
            {
                LinkedResourceNamespace = "TestLibrary.LinkedSqlResources",
                TestScriptNamespace = "TestLibrary.TestScripts",
                OrderedResources = new[]
                {
                    "Tables.NAMEHERE",

                    "StoredProcedures.NAMEHERE",

                    "Scripts.NAMEHERE",
                },
            };
        }
    }
}
