using System;
using System.Collections.Generic;
using System.Linq;
using Flurl;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.Options;

namespace Nactuator
{
    public class ApplicationBuilder : IApplicationBuilder
    {
        // todo health url should be integrated with the healthcheck package
        private readonly IWebHostEnvironment environment;
        private readonly IEnumerable<IMetadataProvider> metadataProviders;
        private readonly SpringBootConfig config;

        public ApplicationBuilder(IWebHostEnvironment environment, IEnumerable<IMetadataProvider> metadataProviders, IOptionsMonitor<SpringBootConfig> optionsAccessor)
        {
            this.environment = environment;
            this.metadataProviders = metadataProviders;

            if (optionsAccessor == null)
            {
                throw new ArgumentNullException(nameof(optionsAccessor));
            }

            config = optionsAccessor.CurrentValue;
        }

        /// <summary>
        /// Relevant SBA classes: https://github.com/codecentric/spring-boot-admin/blob/master/spring-boot-admin-client/src/main/java/de/codecentric/boot/admin/client/config/InstanceProperties.java
        /// https://github.com/codecentric/spring-boot-admin/blob/master/spring-boot-admin-client/src/main/java/de/codecentric/boot/admin/client/registration/DefaultApplicationFactory.java
        /// </summary>
        /// <returns>An Application</returns>
        public virtual Application CreateApplication()
        {
            var application = config.Application;

            if (string.IsNullOrWhiteSpace(application.Name))
            {
                application.Name = environment.ApplicationName;
            }

            if (application.ManagementUrl == null)
            {
                application.ManagementUrl = GetManagementUrl();
            }

            if (application.ServiceUrl == null)
            {
                application.ServiceUrl = GetServiceUrl();
            }

            application.HealthUrl = GetHealthUrl(); // since we need a speical data structure we only use our own health endpoint

            application.Metadata = GetMetadata();

            return application;
        }

        private Uri GetHealthUrl()
        {
            return new Uri(Url.Combine(GetManagementUrl().ToString(), "/health"));
        }

        private IReadOnlyDictionary<string, string> GetMetadata()
        {
            try
            {
                var data = metadataProviders
                            .Select(x => x.GetMetadata())
                            .SelectMany(dict => dict.ToList())
                            .ToDictionary(kvp => kvp.Key, kvp => kvp.Value);

                foreach (var kvp in config.Application.Metadata)
                {
                    data.Add(kvp.Key, kvp.Value);
                }

                return data;
            }
            catch (ArgumentException e)
            {
                throw new DuplicateKeyException("Metadata cannot contain duplicate keys" + e.Message, e);
            }
        }

        private Uri GetServiceUrl()
        {
            return config.Application.ServiceUrl;
        }

        private Uri GetManagementUrl()
        {
            return new Uri(Url.Combine(GetServiceUrl().ToString(), "/actuator"));
        }
    }
}
