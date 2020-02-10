﻿using System.Collections.Generic;

namespace NetCoreAdmin.Metrics
{
    public class MetricsData
    {
        public string Name { get; set; } = string.Empty;

        public string Description { get; set; } = string.Empty;

        public string BaseUnit { get; set; } = string.Empty;

        public IEnumerable<Measurement> Measurements { get; set; } = default!;

        public IEnumerable<AvailableTag> AvailableTags { get; set; } = default!;

        public override string ToString()
        {
            return $"{Name}";
        }
    }

    public class AvailableTag
    {        
        public string Tag { get; set; } = string.Empty;

        public IEnumerable<string> Values { get; set; } = default!;

        public override string ToString()
        {
            return $"{Tag}: {string.Join(", ", Values)}";
        }
    }

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