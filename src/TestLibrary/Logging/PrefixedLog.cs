using System;

namespace TestLibrary.Logging
{
    /// <summary>
    /// Wrapper around a log that prepends useful prefix information (timestamp + severity). Useful when the
    /// nature of the log is such that these extra bits of context are helpful (e.g. time between steps).
    /// </summary>
    public sealed class PrefixedLog : ILog
    {
        private readonly ILog _log;

        public PrefixedLog(ILog log)
        {
            _log = log;
        }

        public void WriteError(string format, params object[] args)
        {
            _log.WriteError(Prefix + "E | " + format, args);
        }

        public void WriteInformation(string format, params object[] args)
        {
            _log.WriteInformation(Prefix + "I | " + format, args);
        }

        public void WriteWarning(string format, params object[] args)
        {
            _log.WriteWarning(Prefix + "W | " + format, args);
        }

        private string Prefix => DateTime.Now.ToString("HH:mm:ss.fff" + " | ");
    }
}
