namespace TestLibrary.Database
{
    /// <summary>
    /// A definition used to create an integration test database instance. These details have been separated from the
    /// database implementation to enable reuse (if desired) with varied database configurations.
    /// </summary>
    public sealed class DatabaseDefinition
    {
        /// <summary>
        /// Must specify the namespace where the database project's SQL resources are linked to.
        /// EXAMPLE: TestLibrary.LinkedSqlResources
        /// </summary>
        public string LinkedResourceNamespace { get; set; }

        /// <summary>
        /// Must specify the namespace where ad-hoc test scripts are located within the test project.
        /// EXAMPLE: TestLibrary.TestScripts
        /// </summary>
        public string TestScriptNamespace { get; set; }

        /// <summary>
        /// Must specify all the desired SQL resource in the proper order. Resources not called out here will not be
        /// deployed even if they exist in the database project. This opt-in model is desirable for a few reasons:
        ///   1. Ordering must be specified. We cannot infer a reasonable order
        ///   2. One might not want every DB artifact to be deployed for testing
        /// EXAMPLE:
        ///   Tables.NAME
        ///   Scripts.NAME
        /// </summary>
        public string[] OrderedResources { get; set; }
    }
}
