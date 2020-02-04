using System.Collections.Generic;

namespace NetCoreAdmin.Beans
{
    public class BeanData
    {
        public IReadOnlyDictionary<string, BeanContext> Contexts { get; set; } = new Dictionary<string, BeanContext>();

        public override string ToString()
        {
            return $"Contexts: {Contexts.Keys}";
        }
    }
}
