namespace NetCoreAdmin.Logfile
{
    public interface ILogFileLocationResolver
    {
        /// <summary>
        /// Implement this to resolve the file path of the log File at runtime. This can also return null, but that disables all log viewing functionality.
        /// This function is called everytime a log file is requested
        /// When both this and a LogFilePath are configured, this Resolver takes precedence.
        /// </summary>
        /// <returns></returns>
        public string ResolveLogFileLocation();
    }
}
