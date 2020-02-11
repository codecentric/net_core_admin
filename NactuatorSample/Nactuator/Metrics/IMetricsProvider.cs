using System.Collections.Generic;
using System.Threading.Tasks;

namespace NetCoreAdmin.Metrics
{
    public interface IMetricsProvider
    {
        MetricsData GetMetricByName(string name);

        IEnumerable<string> GetMetricNames();
    }
}