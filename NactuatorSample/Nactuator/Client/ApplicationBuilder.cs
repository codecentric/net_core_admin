using Flurl;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.Options;
using System;
using System.Collections.Generic;
using System.Linq;

namespace Nactuator
{
    public class ApplicationBuilder : IApplicationBuilder
    {
        // todo make configurable, both from appsettings and code
        // todo health url should be integrated with the healthcheck package

        private readonly IWebHostEnvironment environment;
        private readonly IEnumerable<IMetadataProvider> metadataProviders;
        private readonly Uri baseUrl;
        private readonly SpringBootConfig config;

        public ApplicationBuilder(IWebHostEnvironment environment, IBaseUrlProvider baseUrlProvider, IEnumerable<IMetadataProvider> metadataProviders, IOptionsMonitor<SpringBootConfig> optionsAccessor)
        {
            this.environment = environment;
            this.metadataProviders = metadataProviders;
            if (baseUrlProvider == null)
            {
                throw new ArgumentNullException(nameof(baseUrlProvider));
            }
            baseUrl = baseUrlProvider.AppBaseUrl;

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
        /// <returns></returns>
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

            if (application.HealthUrl == null)
            {
                application.HealthUrl = GetHealthUrl();
            }

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

        /// <summary>
        /// I am not sure if this is sufficient - but for now lets be service and management base url be the same
        /// </summary>
        /// <returns></returns>
        private Uri GetServiceUrl()
        {
            return new Uri($"{baseUrl}/");
        }

        private Uri GetManagementUrl()
        {

            return new Uri(Url.Combine(baseUrl.ToString(), "/actuator")); // todo make both configurable!
        }
    }
}
