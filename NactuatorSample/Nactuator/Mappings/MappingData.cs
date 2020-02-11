using System.Collections.Generic;

namespace NetCoreAdmin.Mappings
{
    public class MappingData
    {
        public IReadOnlyDictionary<string, Application> Contexts { get; set; } = new Dictionary<string, Application>();
    }
}
