using System.Collections.Generic;

namespace NetCoreAdmin.Mappings
{
    public class ServletFilter
    {
        public IReadOnlyList<object> ServletNameMappings { get; set; } = default!;

        public IReadOnlyList<string> UrlPatternMappings { get; set; } = default!;

        public string Name { get; set; } = default!;

        public string ClassName { get; set; } = default!;
    }
}
