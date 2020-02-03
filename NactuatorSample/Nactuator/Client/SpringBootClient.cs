using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Options;
using Nactuator.Client;
using System;
using System.Threading;
using System.Threading.Tasks;

namespace Nactuator
{
    public class SpringBootClient : BackgroundService, ISpringBootClient
    {
        private readonly IApplicationBuilder applicationBuilder;
        private readonly ISpringBootAdminRESTAPI restApi;
        private readonly SpringBootConfig config;

        // todo retry
        // todo deregister

        public SpringBootClient(IApplicationBuilder applicationBuilder, IOptionsMonitor<SpringBootConfig> optionsAccessor, ISpringBootAdminRESTAPI restApi)
        {
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
            await RegisterAsync().ConfigureAwait(false);
        }
    }
}
