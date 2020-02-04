using System.Collections.Generic;

namespace NetCoreAdmin.Health
{
    public class HealthData
    {
        public string Status { get; set; } = string.Empty;

        public IReadOnlyDictionary<string, HealthComponent> Components { get; set; } = new Dictionary<string, HealthComponent>();

        public override string ToString()
        {
            return $"{Status}";
        }
    }
}
