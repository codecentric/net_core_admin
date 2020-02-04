using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
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
        private SpringBootConfig config;

        public LogfileProvider(ILogger<LogfileProvider> logger, IOptionsMonitor<SpringBootConfig> optionsMonitor)
        {
            if (optionsMonitor is null)
            {
                throw new ArgumentNullException(nameof(optionsMonitor));
            }

            config = optionsMonitor.CurrentValue;
            this.logger = logger;
        }

        public async Task<string> GetLogAsync(long? startByte, long? stopByte)
        {
            if (!File.Exists(config.LogFilePath)) {
                var msg = $"The log file at {config.LogFilePath} does not exist. Unable to view Logs. Please check your configuration";
                logger.LogWarning(msg);
                return msg;
            }

            using var fs = new FileStream(config.LogFilePath, FileMode.Open);

            if (startByte != null)
            {
                fs.Position = startByte.Value;
            }

            using var sr = new StreamReader(fs, Encoding.UTF8);

            string data = await sr.ReadToEndAsync().ConfigureAwait(false);

            return data;

        }
    }
}
