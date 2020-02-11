using System.Collections.Generic;

namespace Nactuator
{
    /// <summary>
    /// see https://docs.spring.io/spring-boot/docs/2.2.2.RELEASE/actuator-api/html/#env for format and doc
    /// </summary>
    public class PropertySources
    {
        public PropertySources(string name, IReadOnlyDictionary<string, PropertyValue> properties)
        {
            Name = name;
            Properties = properties;
        }

        public string Name { get; }

        public IReadOnlyDictionary<string, PropertyValue> Properties { get; }

        public override string ToString()
        {
            return $"{Name} - {Properties.Count}";
        }
    }
}