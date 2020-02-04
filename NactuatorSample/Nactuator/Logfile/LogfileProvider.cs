using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using Nactuator;
using System;
using System.IO;

namespace NetCoreAdmin.Logfile
{
    public class LogfileProvider : ILogfileProvider
    {
        private readonly ILogger<LogfileProvider> logger;
        private readonly ILogFileLocationResolver resolver;
        private readonly SpringBootConfig config;

        public LogfileProvider(ILogger<LogfileProvider> logger, IOptionsMonitor<SpringBootConfig> optionsMonitor, ILogFileLocationResolver resolver=null!)
        {
            if (optionsMonitor is null)
            {
                throw new ArgumentNullException(nameof(optionsMonitor));
            }

            config = optionsMonitor.CurrentValue;
            this.logger = logger;
            this.resolver = resolver;
        }

        public FileStream GetLog()
        {
            var logFilePath = GetLogFilePath();

            if (!File.Exists(logFilePath))
            {
                var msg = $"The log file at {logFilePath} does not exist. Unable to view Logs. Please check your configuration";
                logger.LogWarning(msg);
                throw new FileNotFoundException(msg);
            }

            var fs = new FileStream(logFilePath, FileMode.Open, FileAccess.Read, FileShare.ReadWrite);

            return fs;
        }

        private string GetLogFilePath()
        {
            var resolved = resolver?.ResolveLogFileLocation();

            if (resolved != null) {
                logger.LogDebug("Resolved log file to {filePath}", resolved);
                return resolved;
            }

            logger.LogDebug("No ILogFileLocationResolver configured, using configured LogFilePath");

            if (string.IsNullOrWhiteSpace(config.LogFilePath))
            {
                throw new FileNotFoundException("No ILogFileLocationResolver configured and Configuration Setting LogFilePath is blank. Please choose a valid file through one of the options.");
            }

            return config.LogFilePath;


        }
    }
}
