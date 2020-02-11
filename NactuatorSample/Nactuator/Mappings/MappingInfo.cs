using System.Collections.Generic;

namespace NetCoreAdmin.Mappings
{
    public class MappingInfo
    {
        public DispatcherServlets DispatcherServlets { get; set; } = default!;

        public IReadOnlyList<ServletFilter> ServletFilters { get; set; } = default!;

        public IReadOnlyList<Servlet> Servlets { get; set; } = default!;
    }
}
