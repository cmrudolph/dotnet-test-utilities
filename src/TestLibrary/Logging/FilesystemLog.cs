using System.IO;

namespace TestLibrary.Logging
{
    /// <summary>
    /// Log that provides insight into fixture operations and database deployment steps. This can be inspected after
    /// test execution to help diagnose cases where the fixture encounters errors during setup. Thread safety is
    /// unnecessary when not using parallel execution, but it makes the class more robust for other use cases.
    /// </summary>
    public sealed class FilesystemLog : ILog
    {
        private readonly StreamWriter _sw;

        public FilesystemLog(string filename)
        {
            _sw = new StreamWriter(filename)
            {
                AutoFlush = true
            };
        }

        public void WriteError(string format, params object[] args)
        {
            lock (_sw)
            {
                _sw.WriteLine(format, args);
            }
        }

        public void WriteInformation(string format, params object[] args)
        {
            lock (_sw)
            {
                _sw.WriteLine(format, args);
            }
        }

        public void WriteWarning(string format, params object[] args)
        {
            lock (_sw)
            {
                _sw.WriteLine(format, args);
            }
        }
    }
}
