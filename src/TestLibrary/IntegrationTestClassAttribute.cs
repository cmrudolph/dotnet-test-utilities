using System.Diagnostics;
using System.Reflection;
using TestLibrary.Logging;
using Xunit.Sdk;

namespace TestLibrary
{
    /// <summary>
    /// Attribute that should be placed on all integration test classes to enable automatic correlation between test
    /// cases and the shared global logs. Events placed in the logs make it possible to locate a specific test case
    /// in the log (e.g. a specific test's EF queries).
    /// </summary>
    public sealed class IntegrationTestClassAttribute : BeforeAfterTestAttribute
    {
        private Stopwatch _sw;

        public override void Before(MethodInfo methodUnderTest)
        {
            _sw = Stopwatch.StartNew();

            LogFactory.ExecutionLog.WriteInformation($"TEST START -- {TestName(methodUnderTest)}");

            // Formatting intentional - this file relies on whitespace to keep queries separated
            LogFactory.SqlLog.WriteInformation($"TEST START -- {TestName(methodUnderTest)}");
            LogFactory.SqlLog.WriteInformation("");

            base.Before(methodUnderTest);
        }

        public override void After(MethodInfo methodUnderTest)
        {
            base.After(methodUnderTest);

            LogFactory.ExecutionLog.WriteInformation($"TEST END -- {TestName(methodUnderTest)} [{_sw.ElapsedMilliseconds} ms]");

            // Formatting intentional - this file relies on whitespace to keep queries separated
            LogFactory.SqlLog.WriteInformation($"TEST END -- {TestName(methodUnderTest)} [{_sw.ElapsedMilliseconds} ms]");
            LogFactory.SqlLog.WriteInformation("");
        }

        private static string TestName(MethodInfo method) => $"{method.DeclaringType?.FullName}.{method.Name}";
    }
}
