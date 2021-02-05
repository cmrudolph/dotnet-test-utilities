namespace TestLibrary.Logging
{
    /// <summary>
    /// Logging interface intended to be usable throughout the test project (primarily in the 'infrastructure' pieces
    /// to provide visibility into execution and to aid in troubleshooting).
    /// </summary>
    public interface ILog
    {
        void WriteInformation(string format, params object[] args);

        void WriteError(string format, params object[] args);

        void WriteWarning(string format, params object[] args);
    }
}
