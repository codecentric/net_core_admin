using Prometheus;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;

namespace NetCoreAdminSample
{
    public class MetricsCollector
    {
        public async Task GetMetricsAsync()
        {
            using var stream = new MemoryStream();
            await Metrics.DefaultRegistry.CollectAndExportAsTextAsync(stream).ConfigureAwait(false);
            stream.Seek(0L, SeekOrigin.Begin);
            using var reader = new StreamReader(stream);
            var result = await reader.ReadToEndAsync().ConfigureAwait(false);
            Console.WriteLine(result);

        }
    }
}
