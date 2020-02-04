using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using Microsoft.Net.Http.Headers;
using Nactuator;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace NetCoreAdmin.Logfile
{
    public class LogfileProvider : ILogfileProvider
    {
        private readonly ILogger<LogfileProvider> logger;
        private readonly SpringBootConfig config;

        public LogfileProvider(ILogger<LogfileProvider> logger, IOptionsMonitor<SpringBootConfig> optionsMonitor)
        {
            if (optionsMonitor is null)
            {
                throw new ArgumentNullException(nameof(optionsMonitor));
            }

            config = optionsMonitor.CurrentValue;
            this.logger = logger;
        }

        public FileStream GetLog(long? startByte, long? stopByte)
        {
           if (!File.Exists(config.LogFilePath)) {
                var msg = $"The log file at {config.LogFilePath} does not exist. Unable to view Logs. Please check your configuration";
                logger.LogWarning(msg);
                throw new FileNotFoundException(msg);
            }

            var fs = new FileStream(config.LogFilePath, FileMode.Open);

            if (startByte != null)
            {
                fs.Position = startByte.Value;
            }

            if (stopByte != null)
            {
                fs.SetLength(stopByte.Value);
            } 
            return fs;
        }
    }
}
