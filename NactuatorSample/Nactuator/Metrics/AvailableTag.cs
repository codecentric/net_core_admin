using System.Collections.Generic;

namespace NetCoreAdmin.Metrics
{
    public class AvailableTag
    {
        public string Tag { get; set; } = string.Empty;

        public IEnumerable<string> Values { get; set; } = default!;

        public override string ToString()
        {
            return $"{Tag}: {string.Join(", ", Values)}";
        }
    }
}