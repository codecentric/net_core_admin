using Prometheus.Client;
using Prometheus.Client.MetricsWriter.Abstractions;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace NetCoreAdmin.Metrics
{
    class MetricsWriter : IMetricsWriter, ISampleWriter, ILabelWriter
    {
        private MetricsData currentMetric;
        private List<MetricsData> data;

        public MetricsWriter()
        {
            currentMetric = new MetricsData();
            data = new List<MetricsData>();
        }

        public IEnumerable<MetricsData> Result => data;

        public Task CloseWriterAsync()
        {
            // probably end of a mettric
            return Task.CompletedTask;
        }

        public IMetricsWriter EndMetric()
        {
            // end of a metric
            data.Add(currentMetric);
            currentMetric = new MetricsData();
            return this;
        }

        public Task FlushAsync()
        {
            return Task.CompletedTask;
        }

        public IMetricsWriter StartMetric(string metricName)
        {
            currentMetric.Name = metricName;
            return this;
        }

        public ISampleWriter StartSample(string suffix = "")
        {
            return this;
        }

        public IMetricsWriter WriteHelp(string help)
        {
            // ignore help
            return this;
        }

        public IMetricsWriter WriteType(MetricType metricType)
        {
            // valid acutator types: TOTAL, TOTAL_TIME, COUNT, MAX, VALUE, UNKNOWN, ACTIVE_TASKS, DURATION

            currentMetric.BaseUnit = metricType switch
            {
                MetricType.Counter => "COUNT",
                MetricType.Gauge => "VALUE",
                MetricType.Summary => "VALUE",// DE facto mehrere tags in einem?
                MetricType.Untyped => "UNKNOWN",
                MetricType.Histogram => "VALUE",// DE facto mehrere tags in einem?
                _ => throw new ArgumentOutOfRangeException(nameof(metricType)),
            };
            return this;
        }

        #region IDisposable Support
        private bool disposedValue = false; // To detect redundant calls

        protected virtual void Dispose(bool disposing)
        {
            if (!disposedValue)
            {
                if (disposing)
                {
                    // TODO: dispose managed state (managed objects).
                }

                // TODO: free unmanaged resources (unmanaged objects) and override a finalizer below.
                // TODO: set large fields to null.

                disposedValue = true;
            }
        }

        // TODO: override a finalizer only if Dispose(bool disposing) above has code to free unmanaged resources.
        // ~MetricsWriter()
        // {
        //   // Do not change this code. Put cleanup code in Dispose(bool disposing) above.
        //   Dispose(false);
        // }

        // This code added to correctly implement the disposable pattern.
        public void Dispose()
        {
            // Do not change this code. Put cleanup code in Dispose(bool disposing) above.
            Dispose(true);
            // TODO: uncomment the following line if the finalizer is overridden above.
            // GC.SuppressFinalize(this);
        }

        public ILabelWriter StartLabels()
        {
            return this;
        }

        public ISampleWriter WriteValue(double value)
        {
            return this;
        }

        public ISampleWriter WriteTimestamp(long timestamp)
        {
            return this;
        }

        public IMetricsWriter EndSample()
        {
            return this;
        }

        public ILabelWriter WriteLabel(string name, string value)
        {
            return this;
        }

        public ISampleWriter EndLabels()
        {
            return this;
        }
        #endregion
    }
}
