using System.Collections.Generic;

namespace NetCoreAdmin.Metrics
{
    public class AvailableTag
    {
        public string Tag { get; set; } = string.Empty;

        public Dictionary<string, double> Values { get; internal set; } = new Dictionary<string, double>();

        public override string ToString()
        {
            return $"{Tag}: {string.Join(", ", Values)}";
        }
    }
}