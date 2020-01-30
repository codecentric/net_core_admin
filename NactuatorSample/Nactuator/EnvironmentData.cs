using System.Collections.Generic;

namespace Nactuator
{
    public class EnvironmentData
    {
        public IReadOnlyList<string> activeProfiles { get; set; }

        public IReadOnlyCollection<PropertySources> propertySources { get; set; }

        public override string ToString()
        {
            return $"{activeProfiles[0]} - sources: {propertySources.Count}";
        }
    }
}