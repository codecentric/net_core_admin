using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace NetCoreAdmin.Metrics
{
    public class MetricsProvider : IMetricsProvider
    {
        public async Task<IEnumerable<string>> GetMetricNamesAsync()
        {
            throw new NotImplementedException();
        }

        public async Task<MetricsData> GetMetricByNameAsync(string name)
        {
            throw new NotImplementedException();
        }

        // todo query param drilldown
    }
}
