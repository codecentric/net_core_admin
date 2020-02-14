using System;
using System.Diagnostics;

namespace NetCoreAdmin.Metrics
{
    public interface ISystemStatisticsProvider
    {
        double Metric { get; }

        TimeSpan GetProcessUptime()
        {
            var process = Process.GetCurrentProcess();
            var uptime = DateTime.UtcNow - process.StartTime.ToUniversalTime();
            return uptime;
        }

        double GetGCCount()
        {
            // todo move to systemstats?
            int count = 0;
            for (int i = 0; i < GC.MaxGeneration; i++)
            {
                count += GC.CollectionCount(i);
            }

            return count;
        }
    }
}