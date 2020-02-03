using Microsoft.Extensions.Diagnostics.HealthChecks;
using Microsoft.Extensions.Logging;
using System;
using System.Threading.Tasks;

namespace NetCoreAdmin
{
    public class Health : IHealth
    {
        private readonly ILogger<Health> logger;
        private readonly HealthCheckService healthCheckService;

        public Health(ILogger<Health> logger, HealthCheckService healthCheckService = null!)
        {
            this.logger = logger;
            this.healthCheckService = healthCheckService;
        }

        public async Task<bool> GetHealthAsync()
        {
            if (healthCheckService == null)
            {
                logger.LogDebug("No HealthCheckService configured, assuming Health is green. To configure this see https://docs.microsoft.com/en-us/aspnet/core/host-and-deploy/health-checks?view=aspnetcore-3.1");
                return true;
            }

            var report = await healthCheckService.CheckHealthAsync().ConfigureAwait(false);

            return report.Status switch
            {
                HealthStatus.Unhealthy => false,
                HealthStatus.Degraded => false,
                HealthStatus.Healthy => true,
                _ => throw new Exception($"unknown HealthStatus {report.Status}"),
            };
        }
    }
}
