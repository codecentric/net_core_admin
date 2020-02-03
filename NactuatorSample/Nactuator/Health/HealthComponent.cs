using System.Collections.Generic;

namespace NetCoreAdmin.Health
{
    public class HealthComponent
    {
        public string Status { get; set; } = string.Empty;

        public IReadOnlyDictionary<string, object> Details { get; set; } = new Dictionary<string, object>();

        public IReadOnlyDictionary<string, HealthComponent> Components { get; set; } = new Dictionary<string, HealthComponent>();
    }
}