using System.Collections.Generic;

namespace NetCoreAdmin.Mappings
{
    public class RequestMappingConditions
    {
        public IEnumerable<Consume> Consumes { get; set; } = default!;

        public IEnumerable<Header> Headers { get; set; } = default!;

        public IEnumerable<string> Methods { get; set; } = default!;

        public IEnumerable<Header> Params { get; set; } = default!;

        public IEnumerable<string> Patterns { get; set; } = default!;

        public IEnumerable<Consume> Produces { get; set; } = default!;
    }
}
