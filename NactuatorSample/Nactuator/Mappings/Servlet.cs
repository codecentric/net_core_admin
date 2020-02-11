using System.Collections.Generic;

namespace NetCoreAdmin.Mappings
{
    public class Servlet
    {
        public IReadOnlyList<string> Mappings { get; set; } = default!;

        public string Name { get; set; } = default!;

        public string ClassName { get; set; } = default!;
    }
}
