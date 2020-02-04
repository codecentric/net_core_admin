using System;

namespace Nactuator
{
    public class SpringBootConfig
    {
        /// <summary>
        /// The url of the Spring Boot Server where this client should register itself
        /// </summary>
        public Uri SpringBootServerUrl { get; set; } = null!;

        /// <summary>
        /// How long do we wait between attempts to contact the Spring Boot Admin Server? Can be TimeSpan Syntax, see https://docs.microsoft.com/en-us/dotnet/standard/base-types/standard-timespan-format-strings
        /// </summary>
        public TimeSpan RetryTimeout { get; set; } = TimeSpan.FromMinutes(1);

        /// <summary>
        /// Configuration how this Application should register itself
        /// </summary>
        public Application Application { get; set; } = new Application();

        /// <summary>
        /// The path to the current log. If empty or invalid, log file viewing is disabled
        /// </summary>
        public string LogFilePath { get; set; } = string.Empty;
    }
}
