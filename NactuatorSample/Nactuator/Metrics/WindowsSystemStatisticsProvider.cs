﻿using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Runtime.InteropServices;
using System.Threading.Tasks;

namespace NetCoreAdmin.Metrics
{
    public sealed class WindowsSystemStatisticsProvider : IDisposable, ISystemStatisticsProvider
    {
        private readonly PerformanceCounter processorTimePerfCounter;

        public WindowsSystemStatisticsProvider()
        {
            if (!RuntimeInformation.IsOSPlatform(OSPlatform.Windows))
            {
                throw new PlatformNotSupportedException($"{nameof(WindowsSystemStatisticsProvider)} works only on Windows");
            }

            processorTimePerfCounter = new PerformanceCounter("Processor", "% Processor Time", "_Total");

        }

        public double GetMetric()
        {
            var value = processorTimePerfCounter.NextValue();
            return (double)value / 100;
        }

        public void Dispose()
        {
            processorTimePerfCounter.Dispose();
        }
    }
}
