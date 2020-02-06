using System.Collections.Generic;
using System.Threading.Tasks;

namespace NetCoreAdmin.Metrics
{
    public interface IMetricsProvider
    {
        Task<MetricsData> GetMetricByNameAsync(string name);
        Task<IEnumerable<string>> GetMetricNamesAsync();
    }
}