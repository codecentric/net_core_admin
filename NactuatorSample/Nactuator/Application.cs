using System.Collections.Generic;

namespace Nactuator
{
    /// <summary>
    /// Source: https://github.com/codecentric/spring-boot-admin/blob/master/spring-boot-admin-client/src/main/java/de/codecentric/boot/admin/client/registration/Application.java
    /// </summary>
    public class Application
    {
        /// <summary>
        /// Name to register with. Defaults to IWebHostEnvironment.ApplicationName
        /// </summary>
        public string Name { get; set; }

	    /// Client-health-URL to register with. Inferred at runtime, can be overridden in case
	    /// the reachable URL is different (e.g. Docker). Must be unique all services registry.
        public string HealthUrl { get; set; } // todo implement
        public string ManagementUrl { get; set; }
        public string ServiceUrl { get; set; }

        public IReadOnlyDictionary<string, string> Metadata { get; set; }
    }
}