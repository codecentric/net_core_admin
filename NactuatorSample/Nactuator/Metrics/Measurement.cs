using System.Collections.Generic;

namespace NetCoreAdmin.Metrics
{
    public class Measurement
    {
        public string Statistic { get; set; } = string.Empty;

        public double Value { get; set; } = 0L;

        public override string ToString()
        {
            return $"{Value} {Statistic}";
        }
    }
}