using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

namespace NetCoreAdmin.Metrics
{
    public class BasicMetricsProvider : IMetricsProvider
    {
        private readonly ISimpleEventListener eventListener;
        private readonly ISystemStatisticsProvider systemStatisticsProvider;
        private readonly Dictionary<string, Func<MetricsData>> resolvers;

        public BasicMetricsProvider(ISimpleEventListener eventListener, ISystemStatisticsProvider systemStatisticsProvider)
        {
            // list of providers especially for SBA who expects this names
            resolvers = new Dictionary<string, Func<MetricsData>>() {
                {"process.uptime", GetProcessUptime },
                {"process.cpu.usage", GetProcessCPUUsage },
                {"system.cpu.usage", GetSystemCPUUsage },
                {"system.cpu.count", GetCPUCount },
                {"jvm.threads.live", GetLiveThreads },
                {"jvm.threads.daemon", GetDaemonThreads },
                {"jvm.threads.peak", GetPeakThreads }
            };

            this.eventListener = eventListener ?? throw new ArgumentNullException(nameof(eventListener));
            this.systemStatisticsProvider = systemStatisticsProvider;
        }

        private int peakThreads = 0;

        private MetricsData GetPeakThreads()
        {
            var threadCount = Process.GetCurrentProcess().Threads.Count;

            if (threadCount > peakThreads)
            {
                peakThreads = threadCount;
            }

            var result = new List<Measurement>()
            {
                new Measurement
                {
                    Statistic = "VALUE", Value = peakThreads
                }
            };

            return new MetricsData()
            {
                Name = "jvm.threads.peak",
                BaseUnit = "threads",
                Description = "The peak live thread count since the .Net Core runtime started or peak was reset",
                Measurements = result
            };
        }

        private MetricsData GetDaemonThreads()
        {

            var result = new List<Measurement>()
            {
                new Measurement
                {
                    Statistic = "VALUE", Value = 0
                }
            };

            return new MetricsData()
            {
                Name = "jvm.threads.daemon",
                BaseUnit = "threads",
                Description = "The current number of live daemon threads - always 0 since .Net does not have the concept",
                Measurements = result
            };
        }

        private MetricsData GetLiveThreads()
        {

            var threads = Process.GetCurrentProcess().Threads;
            var result = new List<Measurement>()
            {
                new Measurement
                {
                    Statistic = "VALUE", Value =threads.Count
                }
            };

            return new MetricsData()
            {
                Name = "jvm.threads.live",
                BaseUnit = "threads",
                Description = "The current number of live threads. This are system threads, not managed threads",
                Measurements = result
            };
        }

        private MetricsData GetCPUCount()
        {
            return new MetricsData()
            {
                Name = "system.cpu.count",
                BaseUnit = null!,
                Description = "The number of processors available to the .Net Core Runtime",
                Measurements = new List<Measurement>() { new Measurement { Statistic = "VALUE", Value = System.Environment.ProcessorCount } }
            };
        }

        private MetricsData GetSystemCPUUsage()
        {
            var result = new List<Measurement>() { new Measurement() { Statistic = "VALUE", Value = systemStatisticsProvider.GetMetric() } };

            return new MetricsData()
            {
                Name = "system.cpu.usage",
                BaseUnit = null!,
                Description = "The \"recent cpu usage\" for the whole system",
                Measurements = result
            };
        }

        private MetricsData GetProcessCPUUsage()
        {
            // SBA does not show the value at all if its 0, so use a very small value here.
            var originalMetrics = eventListener.Metrics.GetValueOrDefault("cpu-usage");
            var originalValue = originalMetrics?.Measurements?.FirstOrDefault()?.Value;
            var value = originalValue ?? 0.0001;
            if (Math.Abs(value) < 0.0001)
            {
                value = 0.0001;
            }

            var fallBack = new List<Measurement>() { new Measurement() { Statistic = "VALUE", Value = value } };

            return new MetricsData()
            {
                Name = "process.cpu.usage",
                BaseUnit = null!,
                Description = "The recent cpu usage for the NET Core process",
                Measurements = fallBack
            };
        }

        public async Task<MetricsData> GetMetricByNameAsync(string name)
        {
            if (resolvers.TryGetValue(name, out var resultFunc))
            {
                return resultFunc();
            }

            if (eventListener.Metrics.TryGetValue(name, out var result))
            {
                return result;
            }

            throw new KeyNotFoundException(name);
        }

        public async Task<IEnumerable<string>> GetMetricNamesAsync()
        {
            List<string> metrics = eventListener.Metrics.Keys.ToList();
            metrics.AddRange(resolvers.Keys);
            return metrics;
        }

        private MetricsData GetProcessUptime()
        {
            var process = Process.GetCurrentProcess();
            var uptime = DateTime.UtcNow - process.StartTime.ToUniversalTime();
            return new MetricsData()
            {
                Name = "process.uptime",
                BaseUnit = "seconds",
                Description = "The uptime of the process",
                Measurements = new List<Measurement>() { new Measurement() { Statistic = "VALUE", Value = uptime.TotalSeconds } }
            };
        }
    }
}
