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
        /// Configuration how this Application should register itself
        /// </summary>
        public Application Application { get; set; } = null!;
    }
}
