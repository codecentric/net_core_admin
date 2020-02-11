using System.Collections.Generic;

namespace Nactuator
{
    /// <summary>
    /// see https://docs.spring.io/spring-boot/docs/2.2.2.RELEASE/actuator-api/html/#env for format and doc
    /// </summary>
    public class EnvironmentData
    {
        public EnvironmentData(IReadOnlyList<string> activeProfiles, IReadOnlyCollection<PropertySources> propertySources)
        {
            ActiveProfiles = activeProfiles;
            PropertySources = propertySources;
        }

        public IReadOnlyList<string> ActiveProfiles { get; }

        public IReadOnlyCollection<PropertySources> PropertySources { get; }

        public override string ToString()
        {
            return $"{ActiveProfiles[0]} - sources: {PropertySources.Count}";
        }
    }
}