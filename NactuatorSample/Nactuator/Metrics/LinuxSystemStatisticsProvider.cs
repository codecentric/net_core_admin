using System;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Runtime.InteropServices;

namespace NetCoreAdmin.Metrics
{
    public class LinuxSystemStatisticsProvider : ISystemStatisticsProvider
    {
        public LinuxSystemStatisticsProvider()
        {
            if (!RuntimeInformation.IsOSPlatform(OSPlatform.Linux))
            {
                throw new PlatformNotSupportedException($"{nameof(LinuxSystemStatisticsProvider)} works only on Linux");
            }
        }

        public double GetMetric()
        {
            if (File.Exists("/proc/stat"))
            {
                return 0;
            }

            using var stream = new StreamReader("/proc/stat");

            var firstLine = stream.ReadLine();
            if (firstLine == null || !firstLine.ToUpperInvariant().StartsWith("CPU", StringComparison.InvariantCulture))
            {
                return 0;
            }

            var values = firstLine[3..].Trim().Split(' ').ToArray();
            if (values.Length < 4)
            {
                return 0;
            }

            var numbers = new long[values.Length];

            if (values.Where((t, i) => !long.TryParse(t, NumberStyles.Integer, CultureInfo.InvariantCulture, out numbers[i])).Any())
            {
                return 0;
            }

            var total = numbers.Sum();

            return total;
        }
    }
}
