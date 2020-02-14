using System;
using System.Collections.Concurrent;

namespace NetCoreAdmin.Metrics
{
    public interface ISimpleEventListener
    {
        ConcurrentDictionary<string, MetricsData> Metrics { get; }

        event EventHandler GCCollectionEvent;
    }
}