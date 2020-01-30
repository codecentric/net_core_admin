using Microsoft.AspNetCore.Hosting;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Nactuator
{
    public class SpringBootClient
    {
        private readonly IWebHostEnvironment environment;
        private readonly IEnumerable<IMetadataProvider> metadataProviders;
        private readonly string baseUrl;

        // todo create application
        // todo post to SBA
        // todo make configurable
        // todo retry
        // todo deregister

        // todo health url should be integrated with the healthcheck package

        public SpringBootClient(IWebHostEnvironment environment, IBaseUrlProvider baseUrlProvider, IEnumerable<IMetadataProvider> metadataProviders)
        {
            this.environment = environment;
            this.metadataProviders = metadataProviders;
            baseUrl = baseUrlProvider.AppBaseUrl;
        }

        /// <summary>
        /// Relevant SBA classes: https://github.com/codecentric/spring-boot-admin/blob/master/spring-boot-admin-client/src/main/java/de/codecentric/boot/admin/client/config/InstanceProperties.java
        /// https://github.com/codecentric/spring-boot-admin/blob/master/spring-boot-admin-client/src/main/java/de/codecentric/boot/admin/client/registration/DefaultApplicationFactory.java
        /// </summary>
        /// <returns></returns>
        public Application CreateApplication() {
            return new Application() {
                Name = environment.ApplicationName,
                ManagementUrl = GetManagementUrl(),
                ServiceUrl = GetServiceUrl(),
                Metadata = GetMetadata(),
                HealthUrl = GetHealthUrl()
            };
        }

        private string GetHealthUrl()
        {
            return GetManagementUrl() + "/health";
        }

        private IReadOnlyDictionary<string, string> GetMetadata()
        {

            try
            {
                var data = metadataProviders
                            .Select(x => x.GetMetadata())
                            .SelectMany(dict => dict.ToList())
                            .ToDictionary(kvp => kvp.Key, kvp => kvp.Value);
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
        private string GetServiceUrl()
        {
            return $"{baseUrl}/";
        }

        private string GetManagementUrl()
        {
            return baseUrl + "/actuator"; // todo make both configurable!
        }

        public string Register()
        {
            return "";
        }
    }
}
