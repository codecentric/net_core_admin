using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using Nactuator.Client;
using System;
using System.Threading;
using System.Threading.Tasks;

namespace Nactuator
{
    public class SpringBootClient : BackgroundService, ISpringBootClient
    {
        private readonly ILogger<SpringBootClient> logger;
        private readonly IApplicationBuilder applicationBuilder;
        private readonly ISpringBootAdminRESTAPI restApi;
        private readonly SpringBootConfig config;

        public bool Registering { get; set; } = false;

        public SpringBootClient(ILogger<SpringBootClient> logger, IApplicationBuilder applicationBuilder, IOptionsMonitor<SpringBootConfig> optionsAccessor, ISpringBootAdminRESTAPI restApi)
        {
            this.logger = logger;
            this.applicationBuilder = applicationBuilder;
            this.restApi = restApi;
            if (optionsAccessor == null)
            {
                throw new ArgumentNullException(nameof(optionsAccessor));
            }
            config = optionsAccessor.CurrentValue;
        }


        public async Task<string> RegisterAsync()
        {
            // most of httpClient is externalized to a different Service due to the difficulty of testing httpClient.
            var response = await restApi.PostAsync(applicationBuilder.CreateApplication(), config.SpringBootServerUrl).ConfigureAwait(false);
            return response.Id;
        }


        protected override async Task ExecuteAsync(CancellationToken stoppingToken)
        {
            while (true)
            {
                if (stoppingToken.IsCancellationRequested)
                {
                    // todo deregister
                    Registering = false;
                    break;
                }

                Registering = true;
                try
                {
                     await RegisterAsync().ConfigureAwait(false);
                    break;
                }
#pragma warning disable CA1031 // Do not catch general exception types
                catch (Exception e)
#pragma warning restore CA1031 // Do not catch general exception types
                {
                    logger.LogError("Could not connect to Spring Boot Admin Server", e);
                    
                }

                await Task.Delay(config.RetryTimeout).ConfigureAwait(false);
            }
        }
    }
}
