using System.Collections.Generic;

namespace NetCoreAdmin.Metrics
{
    public class AvailableTag
    {
        public string Tag { get; set; } = string.Empty;

#pragma warning disable CA2227 // Collection properties should be read only
        public Dictionary<string, double> Values { get; set; } = new Dictionary<string, double>();
#pragma warning restore CA2227 // Collection properties should be read only

        public override string ToString()
        {
            return $"{Tag}: {string.Join(", ", Values)}";
        }
    }
}