using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace NetCoreAdmin.Metrics
{
    public class ActuatorTag
    {
        public ActuatorTag(string tag, string value)
        {
            if (string.IsNullOrEmpty(tag))
            {
                throw new ArgumentException("message", nameof(tag));
            }

            if (string.IsNullOrEmpty(value))
            {
                throw new ArgumentException("message", nameof(value));
            }

            Tag = tag;
            Value = value;
        }

        public string Tag { get; }

        public string Value { get; }

        public override string ToString()
        {
            return $"{Tag}:{Value}";
        }
    }
}
