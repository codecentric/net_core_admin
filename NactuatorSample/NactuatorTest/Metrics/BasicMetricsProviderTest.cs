using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using FluentAssertions;
using Microsoft.Extensions.Logging;
using Moq;
using NetCoreAdmin.Metrics;
using Xunit;

namespace NetCoreAdminTest.Metrics
{
    public class BasicMetricsProviderTest
    {
        private readonly ILogger<BasicMetricsProvider> loggerMock = new Mock<ILogger<BasicMetricsProvider>>().Object;

        [Fact]
        public void UsesMetricsBySimpleEventListener()
        {
            var eventListenerMock = new Mock<ISimpleEventListener>();
            ConcurrentDictionary<string, MetricsData> metrics = new ConcurrentDictionary<string, MetricsData>();
            MetricsData metric = new MetricsData();
            metrics.TryAdd("test", metric);
            eventListenerMock.Setup(x => x.Metrics).Returns(metrics);

            var systemStatisticsProviderMock = new Mock<ISystemStatisticsProvider>();

            var basicMetricsProvider = new BasicMetricsProvider(loggerMock, eventListenerMock.Object, systemStatisticsProviderMock.Object);

            basicMetricsProvider.GetMetricNames().Should().Contain("test");
            basicMetricsProvider.GetMetricByName("test").Should().Be(metric);
        }

        [Fact]
        public void AddsProcessUptime()
        {
            var eventListenerMock = new Mock<ISimpleEventListener>();
            ConcurrentDictionary<string, MetricsData> metrics = new ConcurrentDictionary<string, MetricsData>();
            eventListenerMock.Setup(x => x.Metrics).Returns(metrics);

            var systemStatisticsProviderMock = new Mock<ISystemStatisticsProvider>();

            TimeSpan uptime = new TimeSpan(0, 1, 0);
            systemStatisticsProviderMock.Setup(x => x.GetProcessUptime()).Returns(uptime);

            var basicMetricsProvider = new BasicMetricsProvider(loggerMock, eventListenerMock.Object, systemStatisticsProviderMock.Object);

            basicMetricsProvider.GetMetricNames().Should().Contain("process.uptime");
            var metric = basicMetricsProvider.GetMetricByName("process.uptime");
            metric.Measurements.First().Value.Should().Be(uptime.TotalSeconds);
        }

        [Fact]
        public void AddsProcessCpuUsageEvenIfNoMetric()
        {
            var eventListenerMock = new Mock<ISimpleEventListener>();
            ConcurrentDictionary<string, MetricsData> metrics = new ConcurrentDictionary<string, MetricsData>();
            eventListenerMock.Setup(x => x.Metrics).Returns(metrics);

            var systemStatisticsProviderMock = new Mock<ISystemStatisticsProvider>();

            var basicMetricsProvider = new BasicMetricsProvider(loggerMock, eventListenerMock.Object, systemStatisticsProviderMock.Object);

            basicMetricsProvider.GetMetricNames().Should().Contain("process.cpu.usage");
            var metric = basicMetricsProvider.GetMetricByName("process.cpu.usage");
            metric.Measurements.First().Value.Should().BeGreaterOrEqualTo(0.0001);
        }

        [Fact]
        public void AddsProcessCpuUsageUsesMEtric()
        {
            var eventListenerMock = new Mock<ISimpleEventListener>();
            ConcurrentDictionary<string, MetricsData> metrics = new ConcurrentDictionary<string, MetricsData>();
            const double cpuValue = 1.1;
            MetricsData cpuUsage = new MetricsData()
            {
                Name = "cpu-usage",
                Measurements = new List<Measurement>() { new Measurement() { Value = cpuValue } },
            };
            metrics.TryAdd("cpu-usage", cpuUsage);
            eventListenerMock.Setup(x => x.Metrics).Returns(metrics);

            var systemStatisticsProviderMock = new Mock<ISystemStatisticsProvider>();

            var basicMetricsProvider = new BasicMetricsProvider(loggerMock, eventListenerMock.Object, systemStatisticsProviderMock.Object);

            basicMetricsProvider.GetMetricNames().Should().Contain("process.cpu.usage");
            var metric = basicMetricsProvider.GetMetricByName("process.cpu.usage");
            metric.Measurements.First().Value.Should().Be(cpuValue);
        }

        [Fact]
        public void AddsSystemCpuUsage()
        {
            var eventListenerMock = new Mock<ISimpleEventListener>();
            ConcurrentDictionary<string, MetricsData> metrics = new ConcurrentDictionary<string, MetricsData>();
            eventListenerMock.Setup(x => x.Metrics).Returns(metrics);

            var systemStatisticsProviderMock = new Mock<ISystemStatisticsProvider>();
            const double systemCpuUsage = 2.2;
            systemStatisticsProviderMock.Setup(x => x.Metric).Returns(systemCpuUsage);

            var basicMetricsProvider = new BasicMetricsProvider(loggerMock, eventListenerMock.Object, systemStatisticsProviderMock.Object);

            basicMetricsProvider.GetMetricNames().Should().Contain("system.cpu.usage");
            var metric = basicMetricsProvider.GetMetricByName("system.cpu.usage");
            metric.Measurements.First().Value.Should().Be(systemCpuUsage);
        }

        [Fact]
        public void AddsSystemCpuCount()
        {
            var eventListenerMock = new Mock<ISimpleEventListener>();
            ConcurrentDictionary<string, MetricsData> metrics = new ConcurrentDictionary<string, MetricsData>();
            eventListenerMock.Setup(x => x.Metrics).Returns(metrics);

            var systemStatisticsProviderMock = new Mock<ISystemStatisticsProvider>();
            var basicMetricsProvider = new BasicMetricsProvider(loggerMock, eventListenerMock.Object, systemStatisticsProviderMock.Object);

            basicMetricsProvider.GetMetricNames().Should().Contain("system.cpu.count");
            var metric = basicMetricsProvider.GetMetricByName("system.cpu.count");
            metric.Measurements.First().Value.Should().Be(System.Environment.ProcessorCount);
        }

        [Fact]
        public void AddsJvmThreadsLive()
        {
            var eventListenerMock = new Mock<ISimpleEventListener>();
            ConcurrentDictionary<string, MetricsData> metrics = new ConcurrentDictionary<string, MetricsData>();
            eventListenerMock.Setup(x => x.Metrics).Returns(metrics);

            var systemStatisticsProviderMock = new Mock<ISystemStatisticsProvider>();
            var basicMetricsProvider = new BasicMetricsProvider(loggerMock, eventListenerMock.Object, systemStatisticsProviderMock.Object);

            basicMetricsProvider.GetMetricNames().Should().Contain("jvm.threads.live");
            var metric = basicMetricsProvider.GetMetricByName("jvm.threads.live");
            metric.Measurements.First().Value.Should().Be(Process.GetCurrentProcess().Threads.Count);
        }

        [Fact]
        public void AddsJvmThreadsDaemon()
        {
            var eventListenerMock = new Mock<ISimpleEventListener>();
            ConcurrentDictionary<string, MetricsData> metrics = new ConcurrentDictionary<string, MetricsData>();
            eventListenerMock.Setup(x => x.Metrics).Returns(metrics);

            var systemStatisticsProviderMock = new Mock<ISystemStatisticsProvider>();
            var basicMetricsProvider = new BasicMetricsProvider(loggerMock, eventListenerMock.Object, systemStatisticsProviderMock.Object);

            basicMetricsProvider.GetMetricNames().Should().Contain("jvm.threads.daemon");
            var metric = basicMetricsProvider.GetMetricByName("jvm.threads.daemon");
            metric.Measurements.First().Value.Should().Be(0);
        }

        [Fact]
        public void AddsJvmThreadsPeak()
        {
            var eventListenerMock = new Mock<ISimpleEventListener>();
            ConcurrentDictionary<string, MetricsData> metrics = new ConcurrentDictionary<string, MetricsData>();
            eventListenerMock.Setup(x => x.Metrics).Returns(metrics);

            var systemStatisticsProviderMock = new Mock<ISystemStatisticsProvider>();
            var basicMetricsProvider = new BasicMetricsProvider(loggerMock, eventListenerMock.Object, systemStatisticsProviderMock.Object);

            basicMetricsProvider.GetMetricNames().Should().Contain("jvm.threads.peak");
            var metric = basicMetricsProvider.GetMetricByName("jvm.threads.peak");
            int threadCount = Process.GetCurrentProcess().Threads.Count;
            metric.Measurements.First().Value.Should().Be(threadCount);

            // todo move this logic to get the threadcoun to systemstatsprovider, for easier and better test
        }

        [Fact]
        public void AddsJvmGcPause()
        {
            var eventListenerMock = new Mock<ISimpleEventListener>();
            ConcurrentDictionary<string, MetricsData> metrics = new ConcurrentDictionary<string, MetricsData>();
            eventListenerMock.Setup(x => x.Metrics).Returns(metrics);

            var firstEvent = new MetricsData()
            {
                Name = "% Time in GC since last GC",
                Measurements = new List<Measurement>() { new Measurement() { Statistic = "VALUE", Value = 50 } },
                AvailableTags = new List<AvailableTag>()
                {
                    new AvailableTag() { Tag = "StandardDeviation", Values = new string[] { "0" }, },
                    new AvailableTag() { Tag = "Count", Values = new string[] { "1" }, },
                    new AvailableTag() { Tag = "Min", Values = new string[] { "0" } },
                    new AvailableTag() { Tag = "Max", Values = new string[] { "0" } },
                    new AvailableTag() { Tag = "IntervalSec", Values = new string[] { "1.00" } },
                    new AvailableTag() { Tag = "Series", Values = new string[] { "Interval=1000" } },
                    new AvailableTag() { Tag = "Min", Values = new string[] { "0" } },
                },
            };

            var secondEvent = new MetricsData()
            {
                Name = "% Time in GC since last GC",
                Measurements = new List<Measurement>() { new Measurement() { Statistic = "VALUE", Value = 25 } },
                AvailableTags = new List<AvailableTag>()
                {
                    new AvailableTag() { Tag = "StandardDeviation", Values = new string[] { "0" }, },
                    new AvailableTag() { Tag = "Count", Values = new string[] { "1" }, },
                    new AvailableTag() { Tag = "Min", Values = new string[] { "0" } },
                    new AvailableTag() { Tag = "Max", Values = new string[] { "0" } },
                    new AvailableTag() { Tag = "IntervalSec", Values = new string[] { "1.00" } },
                    new AvailableTag() { Tag = "Series", Values = new string[] { "Interval=1000" } },
                    new AvailableTag() { Tag = "Min", Values = new string[] { "0" } },
                },
            };

            var systemStatisticsProviderMock = new Mock<ISystemStatisticsProvider>();
            systemStatisticsProviderMock.Setup(x => x.GetGCCount()).Returns(2);
            var basicMetricsProvider = new BasicMetricsProvider(loggerMock, eventListenerMock.Object, systemStatisticsProviderMock.Object);

            eventListenerMock.Raise(x => x.GCCollectionEvent += null, new GcTotalTimeEventArgs(firstEvent));
            eventListenerMock.Raise(x => x.GCCollectionEvent += null, new GcTotalTimeEventArgs(secondEvent));

            basicMetricsProvider.GetMetricNames().Should().Contain("jvm.gc.pause");
            var metric = basicMetricsProvider.GetMetricByName("jvm.gc.pause");

            metric.Measurements.Should().HaveCount(3);

            metric.Measurements.First(x => x.Statistic == "COUNT").Value.Should().Be(2);
            metric.Measurements.First(x => x.Statistic == "TOTAL_TIME").Value.Should().Be(0.75);
            metric.Measurements.First(x => x.Statistic == "MAX").Value.Should().Be(0.5);
        }
    }
}
