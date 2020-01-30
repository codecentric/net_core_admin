using System.Collections.Generic;

namespace Nactuator
{
    public class PropertySources
    {
        /// <summary>
        /// see https://docs.spring.io/spring-boot/docs/2.2.2.RELEASE/actuator-api/html/#env for format and doc
        /// </summary>
        public string Name { get; set; }

        public IReadOnlyDictionary<string, PropertyValue> Properties { get; set; }

        public override string ToString()
        {
            return $"{Name} - {Properties.Count}";
        }
    }
}