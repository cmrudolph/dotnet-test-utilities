using System;
using System.Threading;

namespace TestLibrary.Logging
{
    /// <summary>
    /// Factory controlling the creation of the shared logger instances. The loggers are singletons on purpose,
    /// primarily because it is the easiest way to make things work in a test framework where we are not in full
    /// control of the execution flow.
    /// </summary>
    public static class LogFactory
    {
        private static readonly Lazy<ILog> LazyExecutionLog =
            new Lazy<ILog>(() => new PrefixedLog(new FilesystemLog("TestLog_Execution.log")), LazyThreadSafetyMode.ExecutionAndPublication);
        
        private static readonly Lazy<ILog> LazyEFLog =
            new Lazy<ILog>(() => new FilesystemLog("TestLog_EF.log"), LazyThreadSafetyMode.ExecutionAndPublication);

        public static ILog ExecutionLog => LazyExecutionLog.Value;

        public static ILog SqlLog => LazyEFLog.Value;
    }
}
