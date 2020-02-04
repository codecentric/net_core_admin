using Microsoft.Extensions.Diagnostics.HealthChecks;
using Microsoft.Extensions.Logging;
using NetCoreAdmin.Health;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace NetCoreAdmin
{
    public class HealthProvider: IHealthProvider
    {
        private readonly ILogger<HealthProvider> logger;
        private readonly HealthCheckService healthCheckService;

        public HealthProvider(ILogger<HealthProvider> logger, HealthCheckService healthCheckService = null!)
        {
            this.logger = logger;
            this.healthCheckService = healthCheckService;
        }

        public async Task<HealthData> GetHealthAsync()
        {
            if (healthCheckService == null)
            {
                logger.LogDebug("No HealthCheckService configured, assuming Health is green. To configure this see https://docs.microsoft.com/en-us/aspnet/core/host-and-deploy/health-checks?view=aspnetcore-3.1");
                return new HealthData() {Status = "Healthy" };
            }

            var report = await healthCheckService.CheckHealthAsync().ConfigureAwait(false);

            return FormatHealthReport(report);
        }

        private HealthData FormatHealthReport(HealthReport report)
        {
            var healthData = new HealthData()
            {
                Status = report.Status.ToString(),
                Components = report.Entries.ToDictionary(k => k.Key, v => new HealthComponent()
                {
                    Details = new Dictionary<string, object>() {
                    {"Description", v.Value.Description },
                    {"Exception", v.Value.Exception },
                    {"Duration", v.Value.Duration },
                    {"Data", v.Value.Data},
                    {"Tags", v.Value.Tags }
                },
                    Status = v.Value.Status.ToString()
                })
            };

            return healthData;
        }
    }
}
