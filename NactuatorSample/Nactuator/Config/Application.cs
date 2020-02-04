using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

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
        [Required]
        [MinLength(1)]
        public string Name { get; set; } = default!;

        /// <summary>
        /// URL of the management Endpoints. If empty, defaults to ServiceUrl/actuator
        /// </summary>
        [Url]
        public Uri ManagementUrl { get; set; } = default!;

        /// <summary>
        /// Absolute base url of this Server, e.g. http//localhost:5000
        /// Must not be Emoty
        /// </summary>
        [Required]
        [Url]
        [MinLength(1)]
        public Uri ServiceUrl { get; set; } = default!;

        ///
        /// Client-health-URL to register with. Inferred at runtime
        ///
        public Uri HealthUrl { get; set; } = default!;

        /// <summary>
        /// Additional metadata. Can be supplied by Configuration and dedicated IMetadataProviders.
        /// </summary>
        public IReadOnlyDictionary<string, string> Metadata { get; set; } = new Dictionary<string, string>();
    }
}