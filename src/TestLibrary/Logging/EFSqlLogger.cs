using System;
using Microsoft.Extensions.Logging;

namespace TestLibrary.Logging
{
    /// <summary>
    /// Logger implementation intended to selectively capture EF commands and push them to the SQL log.
    /// </summary>
    public sealed class EfSqlLogger : ILogger
    {
        private readonly ILog _sqlLog;

        public EfSqlLogger(ILog log)
        {
            _sqlLog = log;
        }

        public void Log<TState>(LogLevel logLevel, EventId eventId, TState state, Exception exception, Func<TState, Exception, string> formatter)
        {
            string message = formatter(state, exception);
            if (ShouldLogBasedOnContent(message))
            {
                _sqlLog.WriteInformation(message + Environment.NewLine);
            }
        }

        public bool IsEnabled(LogLevel logLevel)
        {
            return true;
        }

        public IDisposable BeginScope<TState>(TState state)
        {
            throw new NotImplementedException();
        }

        private static bool ShouldLogBasedOnContent(string message)
        {
            // Filter out EF noise that is not the generated commands
            if (message.StartsWith("Executing DbCommand "))
            {
                return true;
            }

            return false;
        }
    }
}
