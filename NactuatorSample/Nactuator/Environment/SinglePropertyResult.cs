using Nactuator;
using System.Collections.Generic;

namespace NetCoreAdmin.Controllers
{
    public class SinglePropertyResult
    {
        public SinglePropertyResult(PropertyResult prop, IReadOnlyList<string> activeProfiles, IReadOnlyCollection<PropertySources> propertySources)
        {
            ActiveProfiles = activeProfiles;
            PropertySources = propertySources;
            @Property = prop;
        }

        public PropertyResult @Property { get; set; }

        public IReadOnlyList<string> ActiveProfiles { get; }

        public IReadOnlyCollection<PropertySources> PropertySources { get; }

    }
}