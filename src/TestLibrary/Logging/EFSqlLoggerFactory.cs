using Microsoft.Extensions.Logging;

namespace TestLibrary.Logging
{
    /// <summary>
    /// Logger factory that constructs the loggers used to capture EF commands and push them to the SQL log.
    /// </summary>
    public sealed class EFSqlLoggerFactory : ILoggerFactory
    {
        private readonly ILog _log;

        public EFSqlLoggerFactory(ILog log)
        {
            _log = log;
        }

        public void Dispose()
        {
        }

        public ILogger CreateLogger(string categoryName)
        {
            return new EfSqlLogger(_log);
        }

        public void AddProvider(ILoggerProvider provider)
        {
        }
    }
}
