using System;
using System.Collections.Concurrent;

namespace NetCoreAdmin.Metrics
{
    public interface ISimpleEventListener
    {
        event EventHandler GCCollectionEvent;

        ConcurrentDictionary<string, MetricsData> Metrics { get; }
    }
}