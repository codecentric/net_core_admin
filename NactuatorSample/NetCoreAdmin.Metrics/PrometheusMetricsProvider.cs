using Prometheus;
using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.IO;
using System.Reflection;
using System.Threading.Tasks;

namespace NetCoreAdmin.Metrics
{
    public class PrometheusMetricsProvider : IMetricsProvider
    {
        public async Task GetMetricsAsync()
        {
            var custom = Prometheus.Metrics.WithCustomRegistry(Prometheus.Metrics.DefaultRegistry);
            using var stream = new MemoryStream();
            await Prometheus.Metrics.DefaultRegistry.CollectAndExportAsTextAsync(stream).ConfigureAwait(false);
            stream.Seek(0L, SeekOrigin.Begin);
            using var reader = new StreamReader(stream);
            var result = await reader.ReadToEndAsync().ConfigureAwait(false);

            var field = typeof(Prometheus.CollectorRegistry).GetField("_collectors", BindingFlags.NonPublic | BindingFlags.Instance);
            var collectors = (ConcurrentDictionary<string, Collector>)field.GetValue(Prometheus.Metrics.DefaultRegistry);
            foreach (var kvp in collectors)
            {
                Console.WriteLine(kvp.Key);
                Console.WriteLine(kvp.Value.Name);
                Console.WriteLine(kvp.Value.LabelNames);
            }
            // todo get method CollectAndSerializeAsync
            // implment Iserializer
            // -> PROFIT!
        }

        public async Task<MetricsData> GetMetricByNameAsync(string name)
        {
            await GetMetricsAsync();
            throw new NotImplementedException();
        }

        public Task<IEnumerable<string>> GetMetricNamesAsync()
        {
            throw new NotImplementedException();
        }
    }
}
