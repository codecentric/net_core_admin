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
            this.activeProfiles = activeProfiles;
            this.propertySources = propertySources;
        }

        public IReadOnlyList<string> activeProfiles { get;  }

        public IReadOnlyCollection<PropertySources> propertySources { get;}

        public override string ToString()
        {
            return $"{activeProfiles[0]} - sources: {propertySources.Count}";
        }
    }
}