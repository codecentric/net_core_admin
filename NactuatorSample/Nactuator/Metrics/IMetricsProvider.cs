using System.Collections.Generic;

namespace NetCoreAdmin.Metrics
{
    public interface IMetricsProvider
    {
        MetricsData GetMetricByName(string name);

        IEnumerable<string> GetMetricNames();

        MetricsData GetMetricByNameAndTag(string metric, ActuatorTag actuatorTag);
    }
}