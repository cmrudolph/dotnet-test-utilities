using DbUp.Engine.Output;

namespace TestLibrary.Logging
{
    /// <summary>
    /// Wrapper around a log instance so it can present as an instance of the type required by DbUp. We want to use the
    /// same underlying log source, but need to honor the interface requirements of the library.
    /// </summary>
    public sealed class UpgradeLogAdapter : IUpgradeLog
    {
        private readonly ILog _wrapped;

        private UpgradeLogAdapter(ILog wrapped)
        {
            _wrapped = wrapped;
        }

        public static IUpgradeLog Adapt(ILog log)
        {
            return new UpgradeLogAdapter(log);
        }

        public void WriteInformation(string format, params object[] args)
        {
            _wrapped.WriteInformation(format, args);
        }

        public void WriteError(string format, params object[] args)
        {
            _wrapped.WriteError(format, args);
        }

        public void WriteWarning(string format, params object[] args)
        {
            _wrapped.WriteWarning(format, args);
        }
    }
}
