using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using Microsoft.Extensions.Logging;

namespace NetCoreAdmin.Metrics
{
    public class BasicMetricsProvider : IMetricsProvider
    {
        private readonly ISimpleEventListener eventListener;
        private readonly ILogger<BasicMetricsProvider> logger;
        private readonly Dictionary<string, Func<MetricsData>> resolvers;
        private readonly ISystemStatisticsProvider systemStatisticsProvider;
        private double maxGcCycleTime;
        private int peakThreads = 0;
        private double totalGcTime;

        public BasicMetricsProvider(ILogger<BasicMetricsProvider> logger, ISimpleEventListener eventListener, ISystemStatisticsProvider systemStatisticsProvider)
        {
            // list of providers especially for SBA who expects this names
            resolvers = new Dictionary<string, Func<MetricsData>>()
            {
                { "process.uptime", GetProcessUptime },
                { "process.cpu.usage", GetProcessCPUUsage },
                { "system.cpu.usage", GetSystemCPUUsage },
                { "system.cpu.count", GetCPUCount },
                { "jvm.threads.live", GetLiveThreads },
                { "jvm.threads.daemon", GetDaemonThreads },
                { "jvm.threads.peak", GetPeakThreads },
                { "jvm.gc.pause", GetGCPause },
                { "jvm.memory.max", GetMemoryMax }, // todo can these three better obtained from event sources?
                { "jvm.memory.used", GetMemoryUsed },
                { "jvm.memory.committed", GetMemoryComitted },
            };

            this.eventListener = eventListener ?? throw new ArgumentNullException(nameof(eventListener));
            this.systemStatisticsProvider = systemStatisticsProvider ?? throw new ArgumentNullException(nameof(systemStatisticsProvider));
            this.logger = logger;
            this.eventListener.GCCollectionEvent += EventListener_GCCollectionEvent;
        }

        public MetricsData GetMetricByName(string name)
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

        public MetricsData GetMetricByNameAndTag(string metric, ActuatorTag actuatorTag)
        {
            if (actuatorTag is null)
            {
                throw new ArgumentNullException(nameof(actuatorTag));
            }

            var metricData = GetMetricByName(metric);
            var area = metricData.AvailableTags.FirstOrDefault(x => x.Tag == actuatorTag.Tag);
            if (area == null)
            {
                throw new UnknownTagErrorException(metric, actuatorTag);
            }

            if (!area.Values.ContainsKey(actuatorTag.Value))
            {
                throw new UnknownTagErrorException(metric, actuatorTag);
            }

            var tagData = area.Values.FirstOrDefault(x => x.Key == actuatorTag.Value);

            return new MetricsData()
            {
                Name = tagData.Key,
                BaseUnit = null!,
                Description = string.Empty,
                Measurements = new List<Measurement>() { new Measurement() { Statistic = tagData.Key, Value = tagData.Value } },
            };
        }

        public IEnumerable<string> GetMetricNames()
        {
            List<string> metrics = eventListener.Metrics.Keys.ToList();
            metrics.AddRange(resolvers.Keys);
            return metrics;
        }

        private void EventListener_GCCollectionEvent(object? sender, EventArgs e)
        {
            var metricsData = ((GcTotalTimeEventArgs)e).MetricsData;
            double gcTime = GetGCTimeFrom(metricsData);
            totalGcTime += gcTime;
            if (gcTime > maxGcCycleTime)
            {
                maxGcCycleTime = gcTime;
            }
        }

        private MetricsData GetCPUCount()
        {
            return new MetricsData()
            {
                Name = "system.cpu.count",
                BaseUnit = null!,
                Description = "The number of processors available to the .Net Core Runtime",
                Measurements = new List<Measurement>() { new Measurement { Statistic = "VALUE", Value = System.Environment.ProcessorCount } },
            };
        }

        private MetricsData GetDaemonThreads()
        {
            // todo if SBA would support this it would be nice to show threadpool threads
            var result = new List<Measurement>()
            {
                new Measurement
                {
                    Statistic = "VALUE",
                    Value = 0,
                },
            };

            return new MetricsData()
            {
                Name = "jvm.threads.daemon",
                BaseUnit = "threads",
                Description = "The current number of live daemon threads - always 0 since .Net does not have the concept",
                Measurements = result,
            };
        }

        private MetricsData GetGCPause()
        {
            var data = new List<Measurement>()
            {
                new Measurement()
                {
                    Statistic = "COUNT", Value = systemStatisticsProvider.GetGCCount(), // num of gc cycles
                },
                new Measurement()
                {
                    Statistic = "TOTAL_TIME", Value = totalGcTime, // total time in seconds
                },
                new Measurement()
                {
                    Statistic = "MAX", Value = maxGcCycleTime, // longest time in gc in seconds
                },
            };

            return new MetricsData()
            {
                Name = "jvm.gc.pause",
                BaseUnit = "seconds",
                Description = "Time spent in GC pause",
                Measurements = data,
            };
        }

        private double GetGCTimeFrom(MetricsData metric)
        {
            var interval = metric.AvailableTags
                .Where(x => x.Tag == "IntervalSec")
                .Select(x => x.Values.First().Value)
                .Single();

            var percent = metric.Measurements.First().Value;

            if (Math.Abs(percent) < 0.00001)
            {
                return 0;
            }

            var result = interval * (percent * 0.01);
            logger.LogDebug("spent {seconds} seconds in garbage collection (gc)", result);
            return result;
        }

        private MetricsData GetLiveThreads()
        {
            var threads = Process.GetCurrentProcess().Threads;
            var result = new List<Measurement>()
            {
                new Measurement
                {
                    Statistic = "VALUE",
                    Value = threads.Count,
                },
            };

            return new MetricsData()
            {
                Name = "jvm.threads.live",
                BaseUnit = "threads",
                Description = "The current number of live threads. This are system threads, not managed threads",
                Measurements = result,
            };
        }

        private MetricsData GetMemoryComitted()
        {
            // likely bogus, I'm not sure these data even exist
            var commited = GC.GetTotalMemory(false);
            return new MetricsData()
            {
                Name = "jvm.memory.committed",
                BaseUnit = "bytes",
                Description = "The amount of committed memory",
                Measurements = new List<Measurement>()
                {
                    new Measurement
                    {
                        Statistic = "VALUE",
                        Value = commited,
                    },
                },
                AvailableTags = new List<AvailableTag>()
                {
                    new AvailableTag()
                    {
                        Tag = "area",
                        Values = new Dictionary<string, double>()
                        {
                            { "heap", commited },
                            { "nonheap", commited },
                        },
                    },
                },
            };
        }

        private MetricsData GetMemoryMax()
        {
            long totalAvailableMemory = GC.GetGCMemoryInfo().TotalAvailableMemoryBytes;
            return new MetricsData()
            {
                Name = "jvm.memory.max",
                BaseUnit = "bytes",
                Description = "The maximum amount of memory in bytes that can be used for memory management",
                Measurements = new List<Measurement>()
                {
                    new Measurement
                    {
                        Statistic = "VALUE",
                        Value = totalAvailableMemory,
                    },
                },
                AvailableTags = new List<AvailableTag>()
                {
                    new AvailableTag()
                    {
                        Tag = "area",
                        Values = new Dictionary<string, double>()
                        {
                            { "heap", totalAvailableMemory },  // todo probably wrong
                            { "nonheap", totalAvailableMemory }, // todo probably wrong
                        },
                    },
                },
            };
        }

        private MetricsData GetMemoryUsed()
        {
            var heapSize = GC.GetGCMemoryInfo().HeapSizeBytes;
            var totalSize = GC.GetTotalMemory(false);
            var nonHeap = totalSize - heapSize;
            return new MetricsData()
            {
                Name = "jvm.memory.used",
                BaseUnit = "bytes",
                Description = "The amount of used memory",
                Measurements = new List<Measurement>()
                {
                    new Measurement
                    {
                        Statistic = "VALUE",
                        Value = totalSize,
                    },
                },
                AvailableTags = new List<AvailableTag>()
                {
                    new AvailableTag()
                    {
                        Tag = "area",
                        Values = new Dictionary<string, double>()
                        {
                            { "heap", heapSize },
                            { "nonheap", nonHeap },
                        },
                    },
                },
            };
        }

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
                    Statistic = "VALUE",
                    Value = peakThreads,
                },
            };

            return new MetricsData()
            {
                Name = "jvm.threads.peak",
                BaseUnit = "threads",
                Description = "The peak live thread count since the .Net Core runtime started or peak was reset",
                Measurements = result,
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
                Measurements = fallBack,
            };
        }

        private MetricsData GetProcessUptime()
        {
            return new MetricsData()
            {
                Name = "process.uptime",
                BaseUnit = "seconds",
                Description = "The uptime of the process",
                Measurements = new List<Measurement>() { new Measurement() { Statistic = "VALUE", Value = systemStatisticsProvider.GetProcessUptime().TotalSeconds } },
            };
        }

        private MetricsData GetSystemCPUUsage()
        {
            var result = new List<Measurement>() { new Measurement() { Statistic = "VALUE", Value = systemStatisticsProvider.Metric } };

            return new MetricsData()
            {
                Name = "system.cpu.usage",
                BaseUnit = null!,
                Description = "The \"recent cpu usage\" for the whole system",
                Measurements = result,
            };
        }
    }
}
