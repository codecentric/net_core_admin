using Microsoft.Extensions.Diagnostics.HealthChecks;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace NetCoreAdminTest
{
    public class HealthCheckServiceStub : HealthCheckService
    {
        public override Task<HealthReport> CheckHealthAsync(Func<HealthCheckRegistration, bool> predicate, CancellationToken cancellationToken = default)
        {
            HealthReport healthResult = new HealthReport(new Dictionary<string, HealthReportEntry> { { "1", new HealthReportEntry(HealthStatus.Healthy, "MESSAGE", TimeSpan.FromSeconds(1), null, null) } }, TimeSpan.FromSeconds(2));
            return new ValueTask<HealthReport>(healthResult).AsTask();
        }
    }
}
