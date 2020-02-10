using Prometheus.Client;
using System;
using System.Collections.Generic;
using System.IO;
using System.Threading.Tasks;

namespace NetCoreAdmin.Metrics
{
    public class PrometheusMetricsProvider : IMetricsProvider
    {
        public async Task GetMetricsAsync()
        {
            var hist = Prometheus.Client.Metrics.CreateHistogram("my_histogram", "help text", buckets: new[] { 0, 0.2, 0.4, 0.6, 0.8, 0.9 });
            hist.Observe(0.4);

            var summary = Prometheus.Client.Metrics.CreateSummary("mySummary", "help text");
            summary.Observe(1);
            summary.Observe(2);
            summary.Observe(3);

            var registry = Prometheus.Client.Metrics.DefaultCollectorRegistry;
            await registry.CollectToAsync(new MetricsWriter());

            using var memStream = new MemoryStream();
            await ScrapeHandler.ProcessAsync(Prometheus.Client.Metrics.DefaultCollectorRegistry, memStream);
            memStream.Seek(0, SeekOrigin.Begin);

            var rd = new StreamReader(memStream);
            var txt = await rd.ReadToEndAsync();
            // Console.WriteLine(txt);
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
