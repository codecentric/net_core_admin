using System;

namespace NetCoreAdmin.Metrics
{
    public class GcTotalTimeEventArgs : EventArgs
    {
        public GcTotalTimeEventArgs(MetricsData metricsData)
            : base()
        {
            MetricsData = metricsData;
        }

        public MetricsData MetricsData { get; }
    }
}