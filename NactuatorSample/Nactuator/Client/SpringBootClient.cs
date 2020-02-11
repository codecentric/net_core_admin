using System;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using Nactuator.Client;

namespace Nactuator
{
    public class SpringBootClient : BackgroundService, ISpringBootClient
    {
        private readonly ILogger<SpringBootClient> logger;
        private readonly ISpringBootAdminRESTAPI restApi;
        private readonly SpringBootConfig config;
        private readonly Application app;

        public SpringBootClient(ILogger<SpringBootClient> logger, IApplicationBuilder applicationBuilder, IOptionsMonitor<SpringBootConfig> optionsAccessor, ISpringBootAdminRESTAPI restApi)
        {
            if (logger is null)
            {
                throw new ArgumentNullException(nameof(logger));
            }

            if (applicationBuilder is null)
            {
                throw new ArgumentNullException(nameof(applicationBuilder));
            }

            if (optionsAccessor is null)
            {
                throw new ArgumentNullException(nameof(optionsAccessor));
            }

            if (restApi is null)
            {
                throw new ArgumentNullException(nameof(restApi));
            }

            this.logger = logger;
            this.restApi = restApi;
            config = optionsAccessor.CurrentValue;
            app = applicationBuilder.CreateApplication();
        }

        public bool Registering { get; set; } = false;

        public async Task<string> RegisterAsync()
        {
            // most of httpClient is externalized to a different Service due to the difficulty of testing httpClient.
            var response = await restApi.PostAsync(app, config.SpringBootServerUrl).ConfigureAwait(false);
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
                    logger.LogDebug("Registering for Spring Boot Admin.");
                    await RegisterAsync().ConfigureAwait(false);
                    break;
                }
#pragma warning disable CA1031 // Do not catch general exception types
                catch (Exception e)
#pragma warning restore CA1031 // Do not catch general exception types
                {
                    logger.LogError(e, "Could not connect to Spring Boot Admin Server. Will retry in {timespan}", config.RetryTimeout);
                }

                await Task.Delay(config.RetryTimeout).ConfigureAwait(false);
            }
        }
    }
}
